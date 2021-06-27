using BankCQRS.Application.Contracts.Persistence;
using BankCQRS.Application.Models.APIConta;
using BankCQRS.Application.Models.MQ;
using BankCQRS.Domain.Entities;
using BankCQRS.Domain.Enums;
using BankCQRS.Infra.Consumer.APIConta;
using BankCQRS.Infra.Consumer.MessageQueues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BankCQRS.Domain.Enums.TransactionStatusEnum;
using static BankCQRS.Domain.Enums.TransactionTypeEnum;

namespace BankCQRS.Infra.Consumer.Consumers
{
    public class TransactionConsumer : BackgroundService
    {
        private readonly MQSettings _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly IModel _model;
        private APIContaClient _apiConta;
        private readonly ILogger<TransactionConsumer> _logger;

        public TransactionConsumer(IOptions<MQSettings> option, IServiceProvider serviceProvider, ILogger<TransactionConsumer> logger)
        {
            _configuration = option.Value;
            _serviceProvider = serviceProvider;
            _model = new QueueFactory(_configuration).CreateTransactionQueue();
            _apiConta = new APIContaClient(_configuration);
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_model);

            consumer.Received += async (sender, eventArgs) =>
            {
                var transactionMessage = new Transaction();
                try
                {
                    var contentArray = eventArgs.Body.ToArray();
                    var contentString = Encoding.UTF8.GetString(contentArray);
                    transactionMessage = JsonConvert.DeserializeObject<Transaction>(contentString);
                    transactionMessage.Status = (int)TransactionStatus.Processing;
                    await UpdateTransactionAsync(transactionMessage);
                    _logger.LogInformation($"Processing message: {contentString}");

                    var accountOrigin = await _apiConta.GetAccountByNumberAsync(transactionMessage.AccountOrigin);
                    var accountDestination = await _apiConta.GetAccountByNumberAsync(transactionMessage.AccountDestination);
                    if (accountOrigin == null || accountDestination == null)
                    {
                        await RetryQueue(eventArgs, transactionMessage, contentString);
                    }
                    else if (accountOrigin.Id == 0 || accountDestination.Id == 0)
                    {
                        transactionMessage.Status = (int)TransactionStatus.Error;
                        transactionMessage.Message = "Account not found.";
                        await UpdateTransactionAsync(transactionMessage);
                        _model.BasicAck(eventArgs.DeliveryTag, false);
                        _logger.LogInformation($"{transactionMessage.Message}: {contentString}");
                    }
                    else
                    {
                        var transferOrigin = new BalanceAdjustment()
                        {
                            TransactionId = transactionMessage.Id,
                            AccountNumber = transactionMessage.AccountOrigin,
                            Value = transactionMessage.Value,
                            Type = Enumerations.GetEnumDescription(TransactionType.Debit)
                        };
                        var transferDestination = new BalanceAdjustment()
                        {
                            TransactionId = transactionMessage.Id,
                            AccountNumber = transactionMessage.AccountDestination,
                            Value = transactionMessage.Value,
                            Type = Enumerations.GetEnumDescription(TransactionType.Credit)
                        };

                        var originResponse = await _apiConta.PostTransferAsync(transferOrigin);
                        if (originResponse.Response == "Success")
                        {
                            var processed = false;
                            while (!processed)
                            {
                                var destinationResponse = await _apiConta.PostTransferAsync(transferDestination);
                                if (destinationResponse.Response == "Success")
                                {
                                    transactionMessage.Status = (int)TransactionStatus.Confirmed;
                                    await UpdateTransactionAsync(transactionMessage);
                                    processed = true;
                                    _model.BasicAck(eventArgs.DeliveryTag, false);
                                    _logger.LogInformation($"Message processed: {contentString}");
                                }
                            }
                        }
                        else if (originResponse.Response == "Not enough balance")
                        {
                            transactionMessage.Status = (int)TransactionStatus.Error;
                            transactionMessage.Message = originResponse.Response;
                            await UpdateTransactionAsync(transactionMessage);
                            _model.BasicAck(eventArgs.DeliveryTag, false);
                            _logger.LogInformation($"{transactionMessage.Message}: {contentString}");
                        }
                        else
                        {
                            await RetryQueue(eventArgs, transactionMessage, contentString);
                        }
                    }
                }
                catch (Exception e)
                {
                    if (transactionMessage != null && transactionMessage.Id != Guid.Empty)
                    {
                        transactionMessage.Status = (int)TransactionStatus.Error;
                        transactionMessage.Message = e.Message;
                        await UpdateTransactionAsync(transactionMessage);
                    }
                    _logger.LogError($"Error in transaction consumer with exception: {e.Message}");
                    _model.BasicReject(eventArgs.DeliveryTag, false);
                }
            };
            _model.BasicConsume(_configuration.TransactionQueue, false, consumer);
        }

        private async Task RetryQueue(BasicDeliverEventArgs eventArgs, Transaction transaction, string contentString)
        {
            var retryHandler = new RetryHandler();
            var attempts = retryHandler.GetRetryAttempts(eventArgs.BasicProperties);
            if (attempts < _configuration.RetryAttempts)
            {
                attempts++;
                var properties = _model.CreateBasicProperties();
                properties.Headers = retryHandler.CopyMessageHeaders(eventArgs.BasicProperties.Headers);
                retryHandler.SetRetryAttempts(properties, attempts);
                _model.BasicPublish(eventArgs.Exchange, eventArgs.RoutingKey, properties, eventArgs.Body);
                _model.BasicAck(eventArgs.DeliveryTag, false);
            }
            else
            {
                transaction.Status = (int)TransactionStatus.Error;
                transaction.Message = "APIConta not reachable.";
                await UpdateTransactionAsync(transaction);
                _model.BasicAck(eventArgs.DeliveryTag, false);
                _logger.LogInformation($"{transaction.Message}: {contentString}");
            }
        }

        private async Task UpdateTransactionAsync(Transaction transaction)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var transactionRepository = scope.ServiceProvider.GetRequiredService<ITransactionRepository>();
                var transactionToUpdate = await transactionRepository.GetByIdAsync(transaction.Id);
                if (transactionToUpdate != null)
                {
                    transactionToUpdate.Status = transaction.Status;
                    transactionToUpdate.Message = transaction.Message;
                    await transactionRepository.UpdateAsync(transactionToUpdate);
                }
            }
        }
    }
}
