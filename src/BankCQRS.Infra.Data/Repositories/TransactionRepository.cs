using BankCQRS.Application.Contracts.Persistence;
using BankCQRS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Infra.Data.Repositories
{
    public class TransactionRepository : BaseRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(BankCQRSDbContext dbContext) : base(dbContext)
        {
        }
    }
}
