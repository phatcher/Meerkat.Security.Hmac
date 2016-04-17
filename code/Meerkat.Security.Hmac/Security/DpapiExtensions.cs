using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Meerkat.Security
{
    public static class DpapiExtensions
    {
        public static SecureString ToSecureString(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var secure = new SecureString();

            return secure;            
        }

        public static string Unwrap(this SecureString value)
        {
            if (value == null)
            {
                return null;
            }

            var ptr = Marshal.SecureStringToCoTaskMemAnsi(value);
            try
            {
                return Marshal.PtrToStringUni(ptr);
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(ptr);
            }
        }

        /// <summary>
        /// Encrypts the contents of a string
        /// </summary>
        /// <param name="value"></param>
        /// <param name="scope"></param>
        /// <param name="entropy"></param>
        /// <returns>Base64 encoded encrypted values</returns>
        public static string Encrypt(this string value, DataProtectionScope scope = DataProtectionScope.CurrentUser, byte[] entropy = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var data = Encoding.Unicode.GetBytes(value);
            var encrypted = ProtectedData.Protect(data, entropy, scope);

            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Encrypts the contents of a <see cref="SecureString"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="scope"></param>
        /// <param name="entropy"></param>
        /// <returns>Base64 encoded encrypted values</returns>
        public static string Encrypt(this SecureString value, DataProtectionScope scope = DataProtectionScope.CurrentUser, byte[] entropy = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var ptr = Marshal.SecureStringToCoTaskMemAnsi(value);
            try
            {
                var buffer = new char[value.Length];
                Marshal.Copy(ptr, buffer, 0, value.Length);

                var data = Encoding.Unicode.GetBytes(buffer);
                var encrypted = ProtectedData.Protect(data, entropy, scope);

                return Convert.ToBase64String(encrypted);
            }
            finally
            {
                Marshal.ZeroFreeCoTaskMemUnicode(ptr);
            }
        }

        /// <summary>
        /// Decrypt a base64 encoded string and returns the decrypted data as a <see cref="SecureString"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="scope"></param>
        /// <param name="entropy"></param>
        /// <returns></returns>
        public static SecureString DecryptSecure(this string value, DataProtectionScope scope = DataProtectionScope.CurrentUser, byte[] entropy = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            // Flip back from Base64
            var data = Convert.FromBase64String(value);

            var decrypted = ProtectedData.Unprotect(data, entropy, scope);
            var chars = Encoding.Unicode.GetChars(decrypted);
            var secure = new SecureString();
            foreach (var c in chars)
            {
                secure.AppendChar(c);
            }

            secure.MakeReadOnly();
            return secure;
        }
    }
}