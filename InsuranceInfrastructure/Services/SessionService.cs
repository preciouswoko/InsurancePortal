using InsuranceCore.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace InsuranceInfrastructure.Services
{
    public class SessionService : ISessionService
    {
        private readonly IMemoryCache _cache;

        public SessionService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void Set<T>(string key, T value)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
            };

            _cache.Set(key, value, cacheEntryOptions);
        }

        public T Get<T>(string key)
        {
            if (_cache.TryGetValue(key, out T value))
            {
                return value;
            }

            return default;
        }
        public void Clear(string key)
        {
            _cache.Remove(key);
        }
    }
}
