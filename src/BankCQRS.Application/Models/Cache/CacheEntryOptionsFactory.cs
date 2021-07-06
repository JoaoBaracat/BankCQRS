using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace BankCQRS.Application.Models.Cache
{
    public class CacheEntryOptionsFactory
    {
        private readonly MemoryCacheSettings _memoryCacheSettings;

        public CacheEntryOptionsFactory(MemoryCacheSettings memoryCacheSettings)
        {
            _memoryCacheSettings = memoryCacheSettings;
        }
        public MemoryCacheEntryOptions CacheEntryOptions()
        {
            return new MemoryCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_memoryCacheSettings.AbsoluteExpirationRelativeToNow),
                SlidingExpiration = TimeSpan.FromSeconds(_memoryCacheSettings.SlidingExpiration)
            };
        }
    }
}
