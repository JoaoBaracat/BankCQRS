using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Features.Transactions.Commands.CreateTransaction
{
    public class CreateTransactionCommand : IRequest<Guid>
    {
        public string AccountOrigin { get; set; }
        public string AccountDestination { get; set; }
        public decimal Value { get; set; }
        //public int Status { get; set; }
        //public string Message { get; set; }
    }
}
