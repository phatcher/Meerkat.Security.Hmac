using System;

using Meerkat.Security.Authentication.Hmac;
using NUnit.Framework;

namespace Meerkat.Hmac.Test.Security.Authentication.Hmac
{
    [TestFixture]
    public class HmacSignatureCalculatorFixture
    {
        [TestCase("SHA256", "UbV2r53a8OFVHVkx0EKdfnJD0H9KxwJ7hD6JOLqpQvQ=")]
        [TestCase("SHA384", "1NeO+Qrd89pPpt911r2jPaP6sm4Xm/tVgwN1uefS/O3bOLzefItwY+Jo5Yu63mss")]
        [TestCase("SHA512", "tgEJ/lVdygPQziNvvIUmSCCwnC2l5pxYHzAX2iQw+gqwDZDNJk24OlVuMl7d6mWHqQewOKUOafRhbVT+/INO6g==")]
        public void ComputeSignature(string scheme, string expected)
        {
            var calculator = new HmacSignatureCalculator();

            var secret = "1234";
            var source = "ABCD";

            var candidate = calculator.Signature(secret, source, scheme);

            Assert.That(candidate, Is.EqualTo(expected), "Signature differs");
        }

        [TestCase("FOO")]
        public void UnknownSchemes(string scheme)
        {
            var calculator = new HmacSignatureCalculator();

            var secret = "1234";
            var source = "ABCD";

            var ex = Assert.Throws<NotSupportedException>(() => calculator.Signature(secret, source, scheme));
        }

        [TestCase("MD5")]
        [TestCase("SHA1")]
        public void DisallowedSchemes(string scheme)
        {
            var calculator = new HmacSignatureCalculator();

            var secret = "1234";
            var source = "ABCD";

            var ex = Assert.Throws<NotSupportedException>(() => calculator.Signature(secret, source, scheme));
        }
    }
}