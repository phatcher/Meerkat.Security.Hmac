﻿#if !NETSTANDARD
using System;

using Meerkat.Caching;

namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Caching layer over a <see cref="ISecretRepository"/> using <see cref="ICache"/>
    /// <para>
    /// Useful for avoiding a database call on every secret retrieve.
    /// </para>
    /// </summary>
    public class CachingSecretRepository : ISecretRepository
    {
        private const string Region = "clientsecret";

        private readonly ISecretRepository repository;
        private readonly ICache cache;
        private readonly TimeSpan duration;

        /// <summary>
        /// Creates a new instance of the <see cref="CachingSecretRepository"/> class.
        /// </summary>
        /// <param name="repository">Underlying repository with the secrets</param>
        /// <param name="cache">Cache to use</param>
        /// <param name="duration">Duration to cache after acquisition</param>
        public CachingSecretRepository(ISecretRepository repository, ICache cache, TimeSpan duration)
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
            return (string)cache.Get(Key(clientId), Region);
        }

        private void Cache(string clientId, string secret, DateTimeOffset absoluteExpiration)
        {
            cache.Set(Key(clientId), secret, absoluteExpiration, Region);
        }

        private string Key(string clientId)
        {
            return clientId;
        }
    }
}
#endif