using System;
using Meerkat.Caching;
using Meerkat.Security.Authentication;
using Moq;
using NUnit.Framework;

namespace Meerkat.Hmac.Test.Security.Authentication
{
    [TestFixture]
    public class CachingSecretRepositoryFixture
    {
        [Test]
        public void EmptyCache()
        {
            var cache = new Mock<ICache>();
            var repository = new Mock<ISecretRepository>();
            var service = new CachingSecretRepository(repository.Object, cache.Object, new TimeSpan(1));

            repository.Setup(x => x.ClientSecret("A")).Returns("B");

            var candidate = service.ClientSecret("A");

            Assert.That(candidate, Is.EqualTo("B"), "Secret differs");

            cache.Verify(x => x.Set("A", "B", It.IsAny<DateTimeOffset>(), It.IsAny<string>()));
        }

        [Test]
        public void CacheHit()
        {
            var cache = new Mock<ICache>();
            var repository = new Mock<ISecretRepository>();
            var service = new CachingSecretRepository(repository.Object, cache.Object, new TimeSpan(1));

            cache.Setup(x => x.Get("A", It.IsAny<string>())).Returns("B");
            repository.Setup(x => x.ClientSecret("A")).Returns("B");

            var candidate = service.ClientSecret("A");

            Assert.That(candidate, Is.EqualTo("B"), "Secret differs");

            cache.Verify(x => x.Set("A", "B", It.IsAny<DateTimeOffset>(), It.IsAny<string>()), Times.Never);
            repository.Verify(x => x.ClientSecret(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void EmptySecretNotCached()
        {
            var cache = new Mock<ICache>();
            var repository = new Mock<ISecretRepository>();
            var service = new CachingSecretRepository(repository.Object, cache.Object, new TimeSpan(1));

            repository.Setup(x => x.ClientSecret("A")).Returns(string.Empty);

            var candidate = service.ClientSecret("A");

            Assert.That(candidate, Is.EqualTo(string.Empty), "Secret differs");

            cache.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTimeOffset>(), It.IsAny<string>()), Times.Never);
        }
    }
}