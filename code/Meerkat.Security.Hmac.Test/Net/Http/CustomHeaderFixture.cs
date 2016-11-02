using System.Net.Http;
using Meerkat.Net.Http;

using Meerkat.Security;

using NUnit.Framework;

namespace Meerkat.Test.Net.Http
{
    [TestFixture]
    public class CustomHeaderFixture
    {
        [Test]
        public void EmptyIfNoCustomHeaders()
        {
            var message = new HttpRequestMessage();

            var candidate = message.Headers.CustomHeadersRepresentation(HmacAuthentication.CustomHeaders);

            Assert.That(candidate, Is.EqualTo(string.Empty));
        }

        [Test]
        public void NullIfMissingHeader()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add(HmacAuthentication.CustomHeaders, "test");

            var candidate = message.Headers.CustomHeadersRepresentation(HmacAuthentication.CustomHeaders);

            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void SingleHeader()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add(HmacAuthentication.CustomHeaders, "test");
            message.Headers.Add("test", "A");

            var candidate = message.Headers.CustomHeadersRepresentation(HmacAuthentication.CustomHeaders);

            Assert.That(candidate, Is.EqualTo("A"));
        }

        [Test]
        public void MultipleHeaders()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add(HmacAuthentication.CustomHeaders, "test token");
            message.Headers.Add("test", "A");
            message.Headers.Add("token", "B");

            var candidate = message.Headers.CustomHeadersRepresentation(HmacAuthentication.CustomHeaders);

            Assert.That(candidate, Is.EqualTo("A\nB"));
        }

        [Test]
        public void MultipleHeaderValues()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add(HmacAuthentication.CustomHeaders, "test token");
            message.Headers.Add("test", "A");
            message.Headers.Add("test", "B");
            message.Headers.Add("token", "C");
            message.Headers.Add("token", "D");

            var candidate = message.Headers.CustomHeadersRepresentation(HmacAuthentication.CustomHeaders);

            Assert.That(candidate, Is.EqualTo("A, B\nC, D"));
        }
    }
}