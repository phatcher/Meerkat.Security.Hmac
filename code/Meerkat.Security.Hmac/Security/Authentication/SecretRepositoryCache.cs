#if NETSTANDARD
using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Caching.Distributed;

namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Caching layer over a <see cref="ISecretRepository"/> using <see cref="IDistributedCache"/>
    /// <para>
    /// Useful for avoiding a database call on every secret retrieve.
    /// </para>
    /// </summary>
    public class SecretRepositoryCache : ISecretRepository
    {
        private const string CacheRegion = "clientsecret";

        private readonly ISecretRepository repository;
        private readonly IDistributedCache cache;
        private readonly TimeSpan duration;
        //private readonly IDataProtector protector;

        /// <summary>
        /// Creates a new instance of the <see cref="SecretRepositoryCache"/> class.
        /// </summary>
        /// <param name="repository">Underlying repository with the secrets</param>
        /// <param name="cache">Cache to use</param>
        /// <param name="duration">Duration to cache after acquisition</param>
        public SecretRepositoryCache(ISecretRepository repository, IDistributedCache cache, TimeSpan duration)
        {
            this.repository = repository;
            this.cache = cache;
            this.duration = duration;
            // TODO: Need to investigate how the scoping of this works
            //this.protector = dataProtectionProvider.CreateProtector("clientsecret");
        }

        /// <summary>
        /// Retrieves the secret from the cache, if not present calls the underlying <see cref="ISecretRepository"/> to acquire and caches the result.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        /// <remarks>We don't do any locking internally, it's a case of last one wins but all that would happen is that the cache duration would be increased slightly.</remarks>
        public string ClientSecret(string clientId)
        {
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
            var result = cache.GetString(Key(clientId));
            return result;
            //return string.IsNullOrEmpty(result) ? result : protector.Unprotect(result);
        }

        private void Cache(string clientId, string secret, DateTimeOffset absoluteExpiration)
        {
            var key = Key(clientId);
            var data = secret;//protector.Protect(secret);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = absoluteExpiration
            };
            Task.Run(() => cache.SetStringAsync(key, data, options));
        }

        private string Key(string clientId)
        {
            return $"{CacheRegion}:{clientId}";
        }
    }
}
#endif
