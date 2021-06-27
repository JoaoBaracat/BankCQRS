using BankCQRS.Application.Contracts;
using BankCQRS.Domain.Common;
using BankCQRS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BankCQRS.Infra.Data
{
    public class BankCQRSDbContext : DbContext
    {
        //private readonly ILoggedInUserService _loggedInUserService;

        public BankCQRSDbContext(DbContextOptions<BankCQRSDbContext> options)
           : base(options)
        {
        }

        //public BankCQRSDbContext(DbContextOptions<BankCQRSDbContext> options, ILoggedInUserService loggedInUserService)
        //    : base(options)
        //{
        //    _loggedInUserService = loggedInUserService;
        //}

        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BankCQRSDbContext).Assembly);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedDate = DateTime.Now;
                        //entry.Entity.CreatedBy = _loggedInUserService?.UserId??"";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedDate = DateTime.Now;
                        //entry.Entity.LastModifiedBy = _loggedInUserService?.UserId ?? "";
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
