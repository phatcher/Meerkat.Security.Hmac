using System;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Meerkat.Net.Http
{
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Get the content's MD5 hash as a Base64 encoded string if present.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string Md5Base64(this HttpContent content)
        {
            var md5 = content == null || content.Headers.ContentMD5 == null
                        ? string.Empty
                        : Convert.ToBase64String(content.Headers.ContentMD5);

            return md5;
        }

        /// <summary>
        /// Checks the validity of the contents MD5 hash.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<bool> IsMd5Valid(this HttpContent content)
        {
            if (content == null)
            {
                // No content so we are correct.
                return true;
            }

            var hashHeader = content.Headers.ContentMD5;
            if (hashHeader == null)
            {
                // TOOD: Should we always require one if we have non-null content?
                // No MD5 hash so true
                return true;
            }

            var hash = await content.ComputeMd5Hash();

            return hash.SequenceEqual(hashHeader);
        }

        /// <summary>
        /// Compute and assign the MD5 hash of the content.
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        public static async Task AssignMd5Hash(this HttpContent httpContent)
        {
            var hash = await httpContent.ComputeMd5Hash();

            httpContent.Headers.ContentMD5 = hash;
        }

        /// <summary>
        /// Compute the MD5 hash of the content.
        /// </summary>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        public static async Task<byte[]> ComputeMd5Hash(this HttpContent httpContent)
        {
            using (var md5 = MD5.Create())
            {
                var content = await httpContent.ReadAsByteArrayAsync();
                var hash = md5.ComputeHash(content);
                return hash;
            }
        }
    }
}