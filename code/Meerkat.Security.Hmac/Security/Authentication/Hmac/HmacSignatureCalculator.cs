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

        // TODO: Inject this via either constructor 
        private const string HmacScheme = "SHA256";

        /// <copydoc cref="ISignatureCalculator.Signature" />
        public string Signature(string secret, string value)
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
           
            using (var hmac = HmacProvider(secretBytes, HmacScheme))
            {
                var hash = hmac.ComputeHash(valueBytes);
                signature = Convert.ToBase64String(hash);
            }

            if (Logger.IsDebugEnabled())
            {
                Logger.DebugFormat("Signature {0} : {1}", HmacScheme, signature);
            }

            return signature;
        }

        private HMAC HmacProvider(byte[] secret, string value)
        {
            switch (value)
            {
                case "MD5":
                case "SHA1":
                    throw new NotSupportedException(string.Format("Hash '{0}' is not secure and is not supported", value));

                case "SHA384":
                    return new HMACSHA384(secret);

                case "SHA512":
                    return new HMACSHA512(secret);

                default:
                    return new HMACSHA256(secret);
            }
        }
    }
}