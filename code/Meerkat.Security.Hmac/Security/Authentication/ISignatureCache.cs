using System;

namespace Meerkat.Security.Authentication
{
    /// <summary>
    /// Cache for message signatures to avoid replay attacks
    /// </summary>
    public interface ISignatureCache
    {
        /// <summary>
        /// Check if the cache contains the signature
        /// </summary>
        /// <param name="signature">Signature to check</param>
        /// <returns>true if present, otherwise false</returns>
        bool Contains(string signature);

        /// <summary>
        /// Add a signature to the cache.
        /// </summary>
        /// <param name="signature">Signature to add</param>
        /// <param name="absoluteExpiration">The absolute expiration date for the cache entry</param>
        void Add(string signature, DateTimeOffset absoluteExpiration);
    }
}