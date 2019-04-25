#if !NETSTANDARD
using System;

using Meerkat.Caching;

namespace Meerkat.Security.Authentication
{
    public class SignatureCache : ISignatureCache
    {
        private const string CacheRegion = "hmac";

        private readonly ICache cache;

        public SignatureCache(ICache cache)
        {
            this.cache = cache;
        }

        public bool Contains(string signature)
        {
            return cache.Contains(signature, CacheRegion);
        }

        public void Add(string signature, DateTimeOffset absoluteExpiration)
        {
            cache.Set(signature, "hmac signature", absoluteExpiration, CacheRegion);
        }
    }
}
#endif