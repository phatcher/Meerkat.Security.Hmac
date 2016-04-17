using System.Collections.Concurrent;

namespace Meerkat.Security
{
    /// <summary>
    /// Simple store for client secrets
    /// </summary>
    public class SecretStore : ISecretRepository, ISecretStore
    {
        private readonly ConcurrentDictionary<string, string> secrets;

        public SecretStore()
        {
            secrets = new ConcurrentDictionary<string, string>();
        }

        public void Assign(string clientId, string secret)
        {
            secrets[clientId] = secret;
        }

        public string ClientSecret(string clientId)
        {
            string secret;
            secrets.TryGetValue(clientId, out secret);

            return secret;
        }
    }
}