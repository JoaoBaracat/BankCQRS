using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Models.MQ
{
    public class APIContaSettings
    {
        public string Url { get; set; }
        public string GetEndPoint { get; set; }
        public string PostEndPoint { get; set; }
    }
}
