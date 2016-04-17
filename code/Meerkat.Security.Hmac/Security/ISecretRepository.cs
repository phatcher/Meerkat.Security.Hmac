namespace Meerkat.Security
{
    /// <summary>
    /// Repository for client secrets.
    /// </summary>
    public interface ISecretRepository
    {
        /// <summary>
        /// Obtain the secret for the user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        string ClientSecret(string userName);
    }
}