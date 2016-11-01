using System.Collections.Concurrent;

namespace Meerkat.Security
{
    /// <summary>
    /// Simple store for client secrets
    /// </summary>
    public class SecretStore : ISecretRepository, ISecretStore
    {
        private readonly ConcurrentDictionary<string, string> secrets;

        /// <summary>
        /// Creates a new instance of the <see cref="SecretStore"/> class.
        /// </summary>
        public SecretStore()
        {
            secrets = new ConcurrentDictionary<string, string>();
        }

        /// <copydoc cref="ISecretStore.Assign" />
        public void Assign(string clientId, string secret)
        {
            secrets[clientId] = secret;
        }

        /// <copydoc cref="ISecretRepository.ClientSecret" />
        public string ClientSecret(string clientId)
        {
            string secret;
            secrets.TryGetValue(clientId, out secret);

            return secret;
        }
    }
}