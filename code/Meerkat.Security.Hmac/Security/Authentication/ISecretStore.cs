namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Responsible for assigning secrets to clients
    /// </summary>
    public interface ISecretStore
    {
        /// <summary>
        /// Assign a secret to a client.
        /// </summary>
        /// <param name="clientId">Client to use</param>
        /// <param name="secret">Secret to use</param>
        void Assign(string clientId, string secret);
    }
}