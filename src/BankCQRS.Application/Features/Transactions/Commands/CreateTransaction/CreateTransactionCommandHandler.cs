using AutoMapper;
using BankCQRS.Application.Contracts.Consumer;
using BankCQRS.Application.Contracts.Persistence;
using BankCQRS.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankCQRS.Application.Features.Transactions.Commands.CreateTransaction
{
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Guid>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateTransactionCommandHandler> _logger;
        private readonly ITransactionSendQueue _transactionSendQueue;

        public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IMapper mapper, ILogger<CreateTransactionCommandHandler> logger, ITransactionSendQueue transactionSendQueue)
        {
            _transactionRepository = transactionRepository;
            _mapper = mapper;
            _logger = logger;
            _transactionSendQueue = transactionSendQueue;
        }

        public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateTransactionCommandValidator(_transactionRepository);
            var validationResult = await validator.ValidateAsync(request);

            if (validationResult.Errors.Count > 0)
                throw new Exceptions.ValidationException(validationResult);

            var @transaction = _mapper.Map<Transaction>(request);

            @transaction = await _transactionRepository.AddAsync(@transaction);

            //Sending email notification to admin address
            //var email = new Email() { To = "joaodobingo@gmail.com", Body = $"A new event was created: {request}", Subject = "A new event was created" };

            try
            {
                //await _emailService.SendEmail(email);
                _transactionSendQueue.SendQueue(JsonConvert.SerializeObject(@transaction));
            }
            catch (Exception ex)
            {
                //this shouldn't stop the API from doing else so this can be logged
                _logger.LogError($"Mailing about event {@transaction.Id} failed due to an error with the mail service: {ex.Message}");
            }

            return @transaction.Id;
        }

    }
}
