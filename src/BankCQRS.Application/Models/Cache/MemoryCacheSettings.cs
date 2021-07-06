using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Models.Cache
{
    public class MemoryCacheSettings
    {
        public int AbsoluteExpirationRelativeToNow { get; set; }
        public int SlidingExpiration { get; set; }
    }
}
