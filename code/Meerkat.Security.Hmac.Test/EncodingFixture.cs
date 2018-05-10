using System.Net;
using NUnit.Framework;

namespace Meerkat.Hmac.Test
{
    [TestFixture]
    public class EncodingFixture
    {
        [TestCase("/odata/$metadata", "/odata/%24metadata")]
        public void UrlDecode(string clientUrl, string serverUrl)
        {
            var expected = WebUtility.UrlDecode(clientUrl);
            var candidate = WebUtility.UrlDecode(serverUrl);

            Assert.AreEqual(expected, candidate);
        }
    }
}