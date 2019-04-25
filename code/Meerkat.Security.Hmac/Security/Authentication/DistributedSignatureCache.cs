#if NETSTANDARD
using System;

using Microsoft.Extensions.Caching.Distributed;

namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// A <see cref="ISignatureCache"/> implemented via <see cref="IDistributedCache"/>
    /// </summary>
    public class DistributedSignatureCache : ISignatureCache
    {
        private const string CacheRegion = "hmac";

        private readonly IDistributedCache cache;

        public DistributedSignatureCache(IDistributedCache cache)
        {
            this.cache = cache;
        }

        public bool Contains(string signature)
        {
            var key = Key(signature);
            var result = cache.Get(key);

            return result != null;
        }

        public void Add(string signature, DateTimeOffset absoluteExpiration)
        {
            var key = Key(signature);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };
            cache.SetString(key, "hmac", options);
        }

        private string Key(string signature)
        {
            return $"{CacheRegion}:{signature}";
        }
    }
}
#endif