using BankCQRS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Domain.Entities
{
    public class Transaction : AuditableEntity
    {
        public Guid Id { get; set; }
        public string AccountOrigin { get; set; }
        public string AccountDestination { get; set; }
        public decimal Value { get; set; }
        public int Status { get; set; }
        public string Message { get; set; }
    }
}
