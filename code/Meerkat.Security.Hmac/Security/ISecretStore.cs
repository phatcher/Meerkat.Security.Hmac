namespace Meerkat.Security
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISecretStore
    {
        /// <summary>
        /// Assign a secret to a client.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="secret"></param>
        void Assign(string clientId, string secret);
    }
}