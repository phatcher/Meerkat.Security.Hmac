namespace Meerkat.Security
{
    /// <summary>
    /// Repository for client secrets.
    /// </summary>
    public interface ISecretRepository
    {
        /// <summary>
        /// Obtain the secret for the client.
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        string ClientSecret(string clientId);
    }
}