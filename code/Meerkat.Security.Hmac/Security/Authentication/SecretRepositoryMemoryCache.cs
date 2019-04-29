#if NETSTANDARD
using System;

using Meerkat.Caching;

using Microsoft.Extensions.Caching.Memory;

namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Caching layer over a <see cref="ISecretRepository"/> using <see cref="IMemoryCache"/>
    /// <para>
    /// Useful for avoiding a database call on every secret retrieve.
    /// </para>
    /// </summary>
    public class SecretRepositoryMemoryCache : ISecretRepository
    {
        private const string CacheRegion = "clientsecret";

        private readonly ISecretRepository repository;
        private readonly IMemoryCache cache;
        private readonly TimeSpan duration;

        /// <summary>
        /// Creates a new instance of the <see cref="SecretRepositoryMemoryCache"/> class.
        /// </summary>
        /// <param name="repository">Underlying repository with the secrets</param>
        /// <param name="cache">Cache to use</param>
        /// <param name="duration">Duration to cache after acquisition</param>
        public SecretRepositoryMemoryCache(ISecretRepository repository, IMemoryCache cache, TimeSpan duration)
        {
            this.repository = repository;
            this.cache = cache;
            this.duration = duration;
        }

        /// <summary>
        /// Retrieves the secret from the cache, if not present calls the underlying <see cref="ISecretRepository"/> to acquire and caches the result.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// <remarks>We don't do any locking internally, it's a case of last one wins but all that would happen is that the cache duration would be increased slightly.</remarks>
        public string ClientSecret(string clientId)
        {
            // TODO: Flip to GetOrCreateAtomic once ready
            var secret = Get(clientId);
            if (secret == null)
            {
                secret = repository.ClientSecret(clientId);
                if (!string.IsNullOrEmpty(secret))
                {
                    Cache(clientId, secret, DateTimeOffset.UtcNow.Add(duration));
                }
            }

            return secret;
        }

        private string Get(string clientId)
        {
            var result = cache.Get<string>(Key(clientId));
            return result;
        }

        private void Cache(string clientId, string secret, DateTimeOffset absoluteExpiration)
        {
            var key = Key(clientId);
            var entry = cache.CreateEntry(key);
            entry.AbsoluteExpiration = absoluteExpiration;
            entry.Value = secret;
        }

        private string Key(string clientId)
        {
            return CacheRegion.CacheKey(clientId);
        }
    }
}
#endif
