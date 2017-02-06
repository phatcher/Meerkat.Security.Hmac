namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Repository for client secrets.
    /// </summary>
    public interface ISecretRepository
    {
        /// <summary>
        /// Obtain the secret for the client.
        /// </summary>
        /// <param name="clientId">ClientId to use</param>
        /// <returns>The secret associated with client, if not present may be null or empty.</returns>
        string ClientSecret(string clientId);
    }
}