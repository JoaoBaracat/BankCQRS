using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Features.Transactions.Queries.GetTransactionDetail
{
    public class GetTransactionDetailQuery : IRequest<TransactionDetailVm>
    {
        public Guid Id { get; set; }
    }
}
