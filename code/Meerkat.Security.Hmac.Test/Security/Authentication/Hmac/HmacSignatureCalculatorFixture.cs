using Meerkat.Security.Authentication.Hmac;
using NUnit.Framework;

namespace Meerkat.Test.Security.Authentication.Hmac
{
    [TestFixture]
    public class HmacSignatureCalculatorFixture
    {
        [Test]
        public void ComputeSignature()
        {
            var calculator = new HmacSignatureCalculator();

            var secret = "1234";
            var source = "ABCD";

            var candidate = calculator.Signature(secret, source);

            Assert.That(candidate, Is.EqualTo("UbV2r53a8OFVHVkx0EKdfnJD0H9KxwJ7hD6JOLqpQvQ="));
        }
    }
}
