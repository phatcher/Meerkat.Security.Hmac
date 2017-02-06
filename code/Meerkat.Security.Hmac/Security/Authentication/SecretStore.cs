using System.Collections.Concurrent;

namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Simple store for client secrets, useful for client side where we might not have a datatabase.
    /// </summary>
    /// <remarks>
    /// Not "secure" in that the secrets are held in memory in clear.
    /// </remarks>
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
            if (string.IsNullOrEmpty(secret))
            {
                string value;
                secrets.TryRemove(clientId, out value);
            }
            else
            {
                secrets[clientId] = secret;
            }
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