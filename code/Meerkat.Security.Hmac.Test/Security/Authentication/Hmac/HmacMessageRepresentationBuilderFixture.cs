using System;
using System.Net.Http;
using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;
using NUnit.Framework;

namespace Meerkat.Test.Security.Authentication.Hmac
{
    [TestFixture]
    public class HmacMessageRepresentationBuilderFixture
    {
        [Test]
        public void GetMessage()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://secured.com/data/test", UriKind.Absolute);
            request.Method = HttpMethod.Get;
            request.Headers.Date = DateTimeOffset.UtcNow;
            request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");
            request.Headers.Add(HmacAuthentication.CustomHeaders, "custom");
            request.Headers.Add("custom", "A");

            var builder = new HmacMessageRepresentationBuilder();

            var candidate = builder.BuildRequestRepresentation(request);

            Assert.That(candidate, Is.Not.Null);
        }

        [Test]
        public void MissingDateReturnsNull()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://secured.com/data/test", UriKind.Absolute);
            request.Method = HttpMethod.Get;
            //request.Headers.Date = DateTimeOffset.UtcNow;
            request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            var builder = new HmacMessageRepresentationBuilder();

            var candidate = builder.BuildRequestRepresentation(request);

            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void MissingClientIdReturnsNull()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://secured.com/data/test", UriKind.Absolute);
            request.Method = HttpMethod.Get;
            request.Headers.Date = DateTimeOffset.UtcNow;
            //request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            var builder = new HmacMessageRepresentationBuilder();

            var candidate = builder.BuildRequestRepresentation(request);

            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void MissingCustomHeaderReturnsNull()
        {
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri("http://secured.com/data/test", UriKind.Absolute);
            request.Method = HttpMethod.Get;
            request.Headers.Date = DateTimeOffset.UtcNow;
            request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");
            request.Headers.Add(HmacAuthentication.CustomHeaders, "custom");

            var builder = new HmacMessageRepresentationBuilder();

            var candidate = builder.BuildRequestRepresentation(request);

            Assert.That(candidate, Is.Null);
        }
    }
}