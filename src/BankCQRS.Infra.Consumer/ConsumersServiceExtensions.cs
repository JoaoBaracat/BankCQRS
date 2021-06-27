using BankCQRS.Application.Contracts.Consumer;
using BankCQRS.Application.Models.MQ;
using BankCQRS.Infra.Consumer.Consumers;
using BankCQRS.Infra.Consumer.MessageQueues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Infra.Consumer
{
    public static class ConsumersServiceExtensions
    {
        public static void AddConsumerService(this IServiceCollection services, IConfiguration configuration)
        {
            //consumer
            services.AddHostedService<TransactionConsumer>();

            //sender
            services.Configure<MQSettings>(configuration.GetSection("MQSettings"));
            services.AddScoped<ITransactionSendQueue, TransactionSendQueue>();
        }
    }
}
