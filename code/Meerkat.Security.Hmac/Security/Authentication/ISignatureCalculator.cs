namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Calculates a message signature 
    /// </summary>
    public interface ISignatureCalculator
    {
        /// <summary>
        /// Given a secret and a value, compute a signature
        /// </summary>
        /// <param name="secret">Secret to use</param>
        /// <param name="value">Value to use</param>
        /// <returns>Computed signature from the value and the secret</returns>
        string Signature(string secret, string value);
    }
}
