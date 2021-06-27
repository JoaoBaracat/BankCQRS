﻿using BankCQRS.Application.Contracts.Consumer;
using BankCQRS.Application.Models.MQ;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Infra.Consumer.MessageQueues
{
    public class TransactionSendQueue : ITransactionSendQueue
    {
        private MQSettings _configuration;
        private IModel _model;

        public TransactionSendQueue(IOptions<MQSettings> option)
        {
            _configuration = option.Value;
        }

        public void SendQueue(string message)
        {
            _model = new QueueFactory(_configuration).CreateTransactionQueue();
            //Setup properties
            var properties = _model.CreateBasicProperties();
            //Serialize
            byte[] messageBuffer = Encoding.Default.GetBytes(message);
            //Send message
            _model.BasicPublish("", _configuration.TransactionQueue, properties, messageBuffer);
        }

    }
}
