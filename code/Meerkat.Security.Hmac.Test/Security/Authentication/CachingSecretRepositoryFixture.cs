#if !NETCOREAPP
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
        public void CacheMiss()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var repository = new Mock<ISecretRepository>(MockBehavior.Strict);
            var service = new CachingSecretRepository(repository.Object, cache.Object, new TimeSpan(1));

            repository.Setup(x => x.ClientSecret("A")).Returns("B");

            cache.Setup(x => x.Get("A", It.IsAny<string>())).Returns((string) null);
            cache.Setup(x => x.Set("A", "B", It.IsAny<DateTimeOffset>(), It.IsAny<string>()));
 
            var candidate = service.ClientSecret("A");

            Assert.That(candidate, Is.EqualTo("B"), "Secret differs");

            cache.Verify(x => x.Set("A", "B", It.IsAny<DateTimeOffset>(), It.IsAny<string>()));
        }

        [Test]
        public void CacheHit()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var repository = new Mock<ISecretRepository>(MockBehavior.Strict);
            var service = new CachingSecretRepository(repository.Object, cache.Object, new TimeSpan(1));

            cache.Setup(x => x.Get("A", It.IsAny<string>())).Returns("B");

            var candidate = service.ClientSecret("A");

            Assert.That(candidate, Is.EqualTo("B"), "Secret differs");
        }

        [Test]
        public void EmptySecretNotCached()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var repository = new Mock<ISecretRepository>(MockBehavior.Strict);
            var service = new CachingSecretRepository(repository.Object, cache.Object, new TimeSpan(1));

            repository.Setup(x => x.ClientSecret("A")).Returns(string.Empty);

            cache.Setup(x => x.Get("A", It.IsAny<string>())).Returns((string) null);

            var candidate = service.ClientSecret("A");

            Assert.That(candidate, Is.EqualTo(string.Empty), "Secret differs");
        }
    }
}
#endif