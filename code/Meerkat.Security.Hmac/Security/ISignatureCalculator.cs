namespace Meerkat.Security
{
    /// <summary>
    /// Calculates a message signature 
    /// </summary>
    public interface ISignatureCalculator
    {
        /// <summary>
        /// Given a secet and a value, compute a signature
        /// </summary>
        /// <param name="secret"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        string Signature(string secret, string value);
    }
}
