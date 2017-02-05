using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Meerkat.Caching;
using Meerkat.Net.Http;
using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;
using Moq;
using NUnit.Framework;

namespace Meerkat.Test.Security.Authentication.Hmac
{
    [TestFixture]
    public class HmacSignatureValidatorFixture
    {
        private Mock<ISignatureCalculator> signatureCalculator;
        private Mock<IMessageRepresentationBuilder> representationBuilder;
        private Mock<ISecretRepository> secretRepository;
        private Mock<ICache> cache;
        private HmacSignatureValidator validator;

        [Test]
        public async Task NoAuthentication()
        {
            var request = new HttpRequestMessage();

            var candidate = await validator.IsValid(request);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [Test]
        public void ValidityPeriod()
        {
            Assert.That(validator.ValidityPeriod, Is.EqualTo(5));
        }

        [Test]
        public async Task NonHmacAuthenticationScheme()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue("test");

            var candidate = await validator.IsValid(request);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [Test]
        public async Task InvalidDate()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme);
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow.AddMinutes(-10));

            var candidate = await validator.IsValid(request);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [Test]
        public async Task NoClientId()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme);
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);

            var candidate = await validator.IsValid(request);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [Test]
        public async Task NoClientSecret()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme);
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            var candidate = await validator.IsValid(request);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [Test]
        public async Task InvalidMd5Hash()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme);
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            request.Content = new StringContent("ABCD");
            request.Content.Headers.ContentMD5 = new byte[] { 0x1, 0x2 };

            secretRepository.Setup(x => x.ClientSecret("test")).Returns("1234");

            var candidate = await validator.IsValid(request);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [Test]
        public async Task InvalidRepresentation()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme, "PQRS");
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            request.Content = new StringContent("ABCD");
            await request.Content.AssignMd5Hash();

            secretRepository.Setup(x => x.ClientSecret("test")).Returns("1234");

            string representation = null;
            representationBuilder.Setup(x => x.BuildRequestRepresentation(request)).Returns(representation);

            var candidate = await validator.IsValid(request);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [Test]
        public async Task ValidSignature()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme, "PQRS");
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            request.Content = new StringContent("ABCD");
            await request.Content.AssignMd5Hash();

            secretRepository.Setup(x => x.ClientSecret("test")).Returns("1234");

            representationBuilder.Setup(x => x.BuildRequestRepresentation(request)).Returns("ABCD");
            signatureCalculator.Setup(x => x.Signature(It.IsAny<string>(), It.IsAny<string>())).Returns("PQRS");

            var candidate = await validator.IsValid(request);

            cache.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<DateTimeOffset>(), "hmac"));

            Assert.That(candidate, Is.EqualTo(true));
        }

        [Test]
        public async Task InvalidSignature()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme, "PQRS");
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            request.Content = new StringContent("ABCD");
            await request.Content.AssignMd5Hash();

            secretRepository.Setup(x => x.ClientSecret("test")).Returns("1234");

            representationBuilder.Setup(x => x.BuildRequestRepresentation(request)).Returns("ABCD");
            signatureCalculator.Setup(x => x.Signature(It.IsAny<string>(), It.IsAny<string>())).Returns("WXYZ");

            var candidate = await validator.IsValid(request);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [Test]
        public async Task ReplayedSignature()
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme, "PQRS");
            request.Headers.Date = new DateTimeOffset(DateTime.UtcNow);
            request.Headers.Add(HmacAuthentication.ClientIdHeader, "test");

            request.Content = new StringContent("ABCD");
            await request.Content.AssignMd5Hash();

            secretRepository.Setup(x => x.ClientSecret("test")).Returns("1234");

            representationBuilder.Setup(x => x.BuildRequestRepresentation(request)).Returns("ABCD");
            signatureCalculator.Setup(x => x.Signature(It.IsAny<string>(), It.IsAny<string>())).Returns("PQRS");
            cache.Setup(x => x.Contains(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var candidate = await validator.IsValid(request);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [SetUp]
        public void Setup()
        {
            signatureCalculator = new Mock<ISignatureCalculator>();
            representationBuilder = new Mock<IMessageRepresentationBuilder>();
            secretRepository = new Mock<ISecretRepository>();
            cache = new Mock<ICache>();

            validator = new HmacSignatureValidator(signatureCalculator.Object, representationBuilder.Object, secretRepository.Object, cache.Object, 5, 5);
        }
    }
}