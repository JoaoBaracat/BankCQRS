using BankCQRS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Contracts.Persistence
{
    public interface ITransactionRepository : IAsyncRepository<Transaction>
    {
    }
}
