﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Models.APIConta
{
    public class Account
    {
        public int Id { get; set; }
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
    }
}
