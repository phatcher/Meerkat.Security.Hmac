#if NETSTANDARD
using System;

using Meerkat.Caching;

using Microsoft.Extensions.Caching.Distributed;

namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Caching layer over a <see cref="ISecretRepository"/> using <see cref="IDistributedCache"/>
    /// </summary>
    /// <remarks>Data is stored in the cache in plaintext, for encryption use <see cref="SecretRepositoryCache" /> with a suitable configured Polly CachePolicy.</remarks>
    public class SecretRepositoryDistributedCache : ISecretRepository
    {
        private const string CacheRegion = "clientsecret";

        private readonly ISecretRepository repository;
        private readonly IDistributedCache cache;
        private readonly TimeSpan duration;

        /// <summary>
        /// Creates a new instance of the <see cref="SecretRepositoryDistributedCache"/> class.
        /// </summary>
        /// <param name="repository">Underlying repository with the secrets</param>
        /// <param name="cache">Cache to use</param>
        /// <param name="duration">Duration to cache after acquisition</param>
        public SecretRepositoryDistributedCache(ISecretRepository repository, IDistributedCache cache, TimeSpan duration)
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
            var key = CacheRegion.CacheKey(clientId);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = duration
            };
            return cache.GetOrCreate(key, () => repository.ClientSecret(clientId), options);
        }
    }
}
#endif
