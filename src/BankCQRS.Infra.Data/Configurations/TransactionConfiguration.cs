using BankCQRS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Infra.Data.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AccountOrigin).IsRequired()
                .HasMaxLength(20)
                .HasColumnType("VARCHAR(20)");

            builder.Property(x => x.AccountDestination).IsRequired()
                .HasMaxLength(20)
                .HasColumnType("VARCHAR(20)");

            builder.Property(x => x.Value).IsRequired();

            builder.ToTable("Transactions");

        }
    }
}
