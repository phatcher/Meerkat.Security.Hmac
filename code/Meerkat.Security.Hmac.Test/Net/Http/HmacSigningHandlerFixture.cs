using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Meerkat.Net.Http;
using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

using Moq;

using NUnit.Framework;

namespace Meerkat.Hmac.Test.Net.Http
{
    [TestFixture]
    public class HmacSigningHandlerFixture
    {
        private Mock<ISecretRepository> secretRepository;
        private Mock<IMessageRepresentationBuilder> representationBuilder;
        private Mock<ISignatureCalculator> calculator;
        private HmacSigningHandler handler;

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
                Assert.That(headers.Authorization, Is.Null, "Authorization present");
                Assert.That(headers.Date, Is.Null, "Date set");
            });
        }

        [Test]
        public async Task NoSecret()
        {
            // Arrange
            var message = new HttpRequestMessage(HttpMethod.Get, "https://foo.com");
            var headers = message.Headers;

            headers.Add(HmacAuthentication.ClientIdHeader, "1234");

            var invoker = new HttpMessageInvoker(handler);

            secretRepository.Setup(x => x.ClientSecret("1234")).Returns((string) null);

            // Act
            await invoker.SendAsync(message, new CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(headers.Authorization, Is.Null, "Authorization present");
                Assert.That(headers.Date, Is.Null, "Date set");
            });
        }

        [Test]
        public async Task InvalidSignature()
        {
            // Arrange
            var message = new HttpRequestMessage(HttpMethod.Get, "https://foo.com");
            var headers = message.Headers;

            headers.Add(HmacAuthentication.ClientIdHeader, "1234");

            var invoker = new HttpMessageInvoker(handler);

            secretRepository.Setup(x => x.ClientSecret("1234")).Returns("ABCD");
            representationBuilder.Setup(x => x.BuildRequestRepresentation(message)).Returns("REPR");
            calculator.Setup(x => x.Signature("ABCD", "REPR", "SHA256")).Returns((string) null);

            // Act
            await invoker.SendAsync(message, new CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(headers.Authorization, Is.Null, "Authorization present");
                Assert.That(headers.Date, Is.Not.Null, "Date not set");
            });
        }

        [Test]
        public async Task HmacSigned()
        {
            // Arrange
            var message = new HttpRequestMessage(HttpMethod.Get, "https://foo.com");
            var headers = message.Headers;

            headers.Add(HmacAuthentication.ClientIdHeader, "1234");

            var invoker = new HttpMessageInvoker(handler);

            secretRepository.Setup(x => x.ClientSecret("1234")).Returns("ABCD");
            representationBuilder.Setup(x => x.BuildRequestRepresentation(message)).Returns("REPR");
            calculator.Setup(x => x.Signature("ABCD", "REPR", "SHA256")).Returns("SIGNED");

            // Act
            await invoker.SendAsync(message, new CancellationToken());

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(headers.Authorization?.ToString(), Is.EqualTo(HmacAuthentication.AuthenticationScheme + " SIGNED"), "Authorization not present");
                Assert.That(headers.Date, Is.Not.Null, "Date not set");
            });
        }

        [SetUp]
        public void Setup()
        {
            secretRepository = new Mock<ISecretRepository>(MockBehavior.Strict);
            representationBuilder = new Mock<IMessageRepresentationBuilder>(MockBehavior.Strict);
            calculator = new Mock<ISignatureCalculator>(MockBehavior.Strict);

            handler = new HmacSigningHandler(secretRepository.Object, representationBuilder.Object, calculator.Object, "SHA256")
            {
                InnerHandler = new StubResponseHandler()
            };
        }
    }
}
