using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

using Meerkat.Logging;

namespace Meerkat.Security.Authentication.Hmac
{
    /// <summary>
    /// Computes a HMAC signature.
    /// </summary>
    public class HmacSignatureCalculator : ISignatureCalculator
    {
        private static readonly ILog Logger = LogProvider.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <copydoc cref="ISignatureCalculator.Signature" />
        public string Signature(string secret, string value, string scheme = "SHA256")
        {
            if (Logger.IsDebugEnabled())
            {
                Logger.DebugFormat("Source {0}: '{1}'", secret.Substring(0, 2) + "...", value);
            }

            var secretBytes = Encoding.Unicode.GetBytes(secret);
            var valueBytes = Encoding.Unicode.GetBytes(value);
            string signature;

            if (Logger.IsDebugEnabled())
            {
                Logger.DebugFormat("Value '{0}'", BitConverter.ToString(valueBytes));
            }
           
            using (var hmac = HmacProvider(secretBytes, scheme))
            {
                var hash = hmac.ComputeHash(valueBytes);
                signature = Convert.ToBase64String(hash);
            }

            if (Logger.IsDebugEnabled())
            {
                Logger.DebugFormat("Signature {0} : {1}", scheme, signature);
            }

            return signature;
        }

        private HMAC HmacProvider(byte[] secret, string value)
        {
            switch (value)
            {
                case "MD5":
                case "SHA1":
                    throw new NotSupportedException($"Hash '{value}' is not secure and is not supported");

                case "SHA256":
                    return new HMACSHA256(secret);

                case "SHA384":
                    return new HMACSHA384(secret);

                case "SHA512":
                    return new HMACSHA512(secret);

                default:
                    throw new NotSupportedException($"Hash '{value}' is unknown and is not supported");
            }
        }
    }
}