using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Meerkat.Net.Http;
using Meerkat.Security.Authentication.Hmac;

using NUnit.Framework;

namespace Meerkat.Hmac.Test.Net.Http
{
    [TestFixture]
    public class HmacClientHandlerFixture
    {
        private HmacClientHandler handler;
        private HmacClient client;

        [Test]
        public async Task NoClientId()
        {
            // Arrange
            var message = new HttpRequestMessage(HttpMethod.Get, "https://foo.com");
            var headers = message.Headers;

            var invoker = new HttpMessageInvoker(handler);

            // Act
            await invoker.SendAsync(message, new CancellationToken());

            // Asset
            Assert.Multiple(() =>
            {
                Assert.That(GetHeader(headers, HmacAuthentication.ClientIdHeader), Is.Null, "ClientId differs");
                Assert.That(GetHeader(headers, HmacAuthentication.NonceHeader), Is.Null, "Nonce differs");
            });
        }

        [Test]
        public async Task ClientId()
        {
            // Arrange
            var message = new HttpRequestMessage(HttpMethod.Get, "https://foo.com");
            var headers = message.Headers;

            var invoker = new HttpMessageInvoker(handler);

            client.ClientId = "1234";

            // Act
            await invoker.SendAsync(message, new CancellationToken());

            // Asset
            Assert.Multiple(() =>
            {
                Assert.That(GetHeader(headers, HmacAuthentication.ClientIdHeader), Is.EqualTo("1234"), "ClientId differs");
                Assert.That(GetHeader(headers, HmacAuthentication.NonceHeader), Is.Not.Null, "Nonce differs");
            });
        }

        [Test]
        public async Task AuthorizationPresent()
        {
            // Arrange
            var message = new HttpRequestMessage(HttpMethod.Get, "https://foo.com");
            var headers = message.Headers;

            headers.Authorization = new AuthenticationHeaderValue("Bearer", "JWT");

            var invoker = new HttpMessageInvoker(handler);

            client.ClientId = "1234";

            // Act
            await invoker.SendAsync(message, new CancellationToken());

            // Asset
            Assert.Multiple(() =>
            {
                Assert.That(GetHeader(headers, HmacAuthentication.ClientIdHeader), Is.Null, "ClientId differs");
                Assert.That(GetHeader(headers, HmacAuthentication.NonceHeader), Is.Null, "Nonce differs");
            });
        }

        [SetUp]
        public void Setup()
        {
            client = new HmacClient();

            handler = new HmacClientHandler(client)
            {
                InnerHandler = new StubResponseHandler()
            };
        }

        private string GetHeader(HttpRequestHeaders headers, string name)
        {
            if (headers.TryGetValues(name, out var values))
            {
                return values.First();
            }

            return null;
        }
    }
}
