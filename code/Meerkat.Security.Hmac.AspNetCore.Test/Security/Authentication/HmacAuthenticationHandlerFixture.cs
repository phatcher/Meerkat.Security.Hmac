
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

using NUnit.Framework;

namespace Meerkat.Hmac.Test.Security.Authentication
{
    [TestFixture]
    public class HmacAuthenticationHandlerFixture
    {
        private Mock<ILogger<HmacAuthenticationHandler>> logger;
        private Mock<IHmacAuthenticator> authenticator;
        private HmacAuthenticationHandler handler;

        [Test]
        public async Task NoAuthorizationHeader()
        {
            // Arrange
            var context = new DefaultHttpContext();

            await InitializeHandler(context);

            // Act
            var result = await handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False, "Succeeded differs");
            Assert.That(result.Failure, Is.Null, "Failure differs");
        }

        [Test]
        public async Task NotHmac()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = "Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==";

            await InitializeHandler(context);

            // Act
            var result = await handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False, "Succeeded differs");
            Assert.That(result.Failure, Is.Null, "Failure differs");
        }

        [Test]
        public async Task NoHmacParameter()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = HmacAuthentication.AuthenticationScheme;

            await InitializeHandler(context);

            // Act
            var result = await handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False, "Succeeded differs");
            Assert.That(result.Failure, Is.Not.Null, "Failure differs");
            Assert.That(result.Failure.Message, Is.EqualTo("Missing credentials"));
        }

        [Test]
        [Ignore("Need to work on the test expectations")]
        public async Task InvalidSignature()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Headers["Authorization"] = HmacAuthentication.AuthenticationScheme + " Foo";
            var request = new DefaultHttpRequest(context)
            {
                Method = "GET"
            };

            authenticator.Setup(x => x.Authenticate(It.IsAny<System.Net.Http.HttpRequestMessage>())).ReturnsAsync(new ClaimsIdentity());

            await InitializeHandler(context);

            // Act
            var result = await handler.AuthenticateAsync();

            Assert.That(result.Succeeded, Is.False, "Succeeded differs");
            Assert.That(result.Failure, Is.Not.Null, "Failure differs");
            Assert.That(result.Failure.Message, Is.EqualTo("Invalid signature"));
        }

        private async Task InitializeHandler(HttpContext context)
        {
            await handler.InitializeAsync(new AuthenticationScheme(HmacAuthentication.AuthenticationScheme, HmacAuthentication.AuthenticationScheme, typeof(HmacAuthenticationHandler)), context);
        }

        [SetUp]
        public void OnSetup()
        {
            logger = new Mock<ILogger<HmacAuthenticationHandler>>();
            var loggerFactory = new Mock<ILoggerFactory>(MockBehavior.Strict);
            loggerFactory.Setup(x => x.CreateLogger("Meerkat.Security.Authentication.HmacAuthenticationHandler")).Returns(logger.Object);

            var encoder = new Mock<UrlEncoder>();
            var clock = new Mock<ISystemClock>();

            var options = new HmacAuthenticationOptions();
            var om = new Mock<IOptionsMonitor<HmacAuthenticationOptions>>(MockBehavior.Strict);
            om.Setup(x => x.Get(HmacAuthentication.AuthenticationScheme)).Returns(options);

            authenticator = new Mock<IHmacAuthenticator>(MockBehavior.Strict);

            handler = new HmacAuthenticationHandler(authenticator.Object, om.Object, loggerFactory.Object, encoder.Object, clock.Object);
        }
    }
}