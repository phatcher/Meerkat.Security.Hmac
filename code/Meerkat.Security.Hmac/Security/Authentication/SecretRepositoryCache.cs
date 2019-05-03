#if NETSTANDARD
using System.Threading.Tasks;

using Meerkat.Caching;

using Polly.Caching;

namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Caching layer over a <see cref="ISecretRepository"/> using a <see cref="CachePolicy"/>
    /// </summary>
    public class SecretRepositoryCache : ISecretRepository
    {
        private const string CacheRegion = "clientsecret";

        private readonly ISecretRepository repository;
        private readonly CachePolicy cachePolicy;

        /// <summary>
        /// Creates a new instance of the <see cref="SecretRepositoryCache"/> class.
        /// </summary>
        /// <param name="repository">Underlying repository with the secrets</param>
        /// <param name="cachePolicy">Cache to use</param>
        public SecretRepositoryCache(ISecretRepository repository, CachePolicy cachePolicy)
        {
            this.repository = repository;
            this.cachePolicy = cachePolicy;
        }

        /// <summary>
        /// Retrieves the secret from the cache, if not present calls the underlying <see cref="ISecretRepository"/> to acquire and caches the result.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public string ClientSecret(string clientId)
        {
            var key = CacheRegion.CacheKey(clientId);

            return cachePolicy.Cache(key, () => Task.FromResult(repository.ClientSecret(clientId))).GetAwaiter().GetResult();
        }
    }
}
#endif
