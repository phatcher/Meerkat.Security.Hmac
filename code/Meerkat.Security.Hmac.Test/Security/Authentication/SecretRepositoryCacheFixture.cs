#if NETCOREAPP
using System;
using System.Text;

using Meerkat.Security.Authentication;

using Microsoft.Extensions.Caching.Distributed;

using Moq;

using NUnit.Framework;

namespace Meerkat.Hmac.Test.Security.Authentication
{
    [TestFixture]
    public class SecretRepositoryCacheFixture
    {
        [Test]
        public void CacheMiss()
        {
            var cache = new Mock<IDistributedCache>(MockBehavior.Strict);
            var repository = new Mock<ISecretRepository>(MockBehavior.Strict);
            var service = new SecretRepositoryCache(repository.Object, cache.Object, new TimeSpan(1));

            // NB Have to use the underlying methods not the helpful extensions
            cache.Setup(x => x.Get("clientsecret:A")).Returns((byte[]) null);
            cache.Setup(x => x.Set("clientsecret:A", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()));

            repository.Setup(x => x.ClientSecret("A")).Returns("B");

            var candidate = service.ClientSecret("A");

            Assert.That(candidate, Is.EqualTo("B"), "Secret differs");
        }

        [Test]
        public void CacheHit()
        {
            var cache = new Mock<IDistributedCache>(MockBehavior.Strict);
            var repository = new Mock<ISecretRepository>(MockBehavior.Strict);
            var service = new SecretRepositoryCache(repository.Object, cache.Object, new TimeSpan(1));

            // NB Have to use the underlying methods not the helpful extensions
            cache.Setup(x => x.Get("clientsecret:A")).Returns(Encoding.UTF8.GetBytes("B"));

            var candidate = service.ClientSecret("A");

            Assert.That(candidate, Is.EqualTo("B"), "Secret differs");
        }

        [Test]
        public void EmptySecretNotCached()
        {
            var cache = new Mock<IDistributedCache>(MockBehavior.Strict);
            var repository = new Mock<ISecretRepository>(MockBehavior.Strict);
            var service = new SecretRepositoryCache(repository.Object, cache.Object, new TimeSpan(1));

            // NB Have to use the underlying methods not the helpful extensions
            cache.Setup(x => x.Get("clientsecret:A")).Returns((byte[]) null);
            repository.Setup(x => x.ClientSecret("A")).Returns(string.Empty);

            var candidate = service.ClientSecret("A");

            Assert.That(candidate, Is.EqualTo(string.Empty), "Secret differs");
        }
    }
}
#endif
