#if NETSTANDARD
using System;

using Microsoft.Extensions.Caching.Memory;

namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// A <see cref="ISignatureCache"/> implemented via <see cref="IMemoryCache"/>
    /// </summary>
    public class MemorySignatureCache : ISignatureCache
    {
        private const string CacheRegion = "hmac";

        private readonly IMemoryCache cache;

        public MemorySignatureCache(IMemoryCache cache)
        {
            this.cache = cache;
        }

        public bool Contains(string signature)
        {
            var key = Key(signature);
            return cache.TryGetValue(key, out var value);
        }

        public void Add(string signature, DateTimeOffset absoluteExpiration)
        {
            var key = Key(signature);
            var entry = cache.CreateEntry(key);
            entry.AbsoluteExpiration = absoluteExpiration;
            entry.Value = "hmac";
        }

        private string Key(string signature)
        {
            return $"{CacheRegion}:{signature}";
        }
    }
}
#endif