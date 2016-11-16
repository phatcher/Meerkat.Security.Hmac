using System.Net.Http;
using System.Threading.Tasks;

using Meerkat.Security;

using Moq;

using NUnit.Framework;

namespace Meerkat.Test.Security
{
    [TestFixture]
    public class HmacAuthenticatorFixture
    {
        [Test]
        public async Task ValidSignature()
        {
            // Arrange
            var validator = new Mock<ISignatureValidator>();
            var provider = new RequestClaimsProvider("name");

            var authenticator = new HmacAuthenticator(validator.Object, provider, "name", "role");

            var message = new HttpRequestMessage();
            message.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            validator.Setup(x => x.IsValid(It.IsAny<HttpRequestMessage>())).ReturnsAsync(true);

            // Act
            var candidate = await authenticator.Authenticate(message);

            // Assert
            Assert.That(candidate, Is.Not.Null);
            Assert.That(candidate.IsAuthenticated, Is.EqualTo(true));
            Assert.That(candidate.AuthenticationType, Is.EqualTo(HmacAuthentication.AuthenticationScheme));
            Assert.That(candidate.Name, Is.EqualTo("test"));
        }

        [Test]
        public async Task HmacNotPresent()
        {
            // Arrange
            var validator = new Mock<ISignatureValidator>();
            var provider = new RequestClaimsProvider("name");

            var authenticator = new HmacAuthenticator(validator.Object, provider, "name", "role");

            var message = new HttpRequestMessage();
            message.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            validator.Setup(x => x.IsValid(It.IsAny<HttpRequestMessage>())).ReturnsAsync(false);

            // Act
            var candidate = await authenticator.Authenticate(message);

            // Assert
            Assert.That(candidate, Is.Null);
        }
    }
}
