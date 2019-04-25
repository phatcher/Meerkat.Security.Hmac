#if !NETCOREAPP

using System;

using Meerkat.Caching;
using Meerkat.Security.Authentication;

using Moq;

using NUnit.Framework;

namespace Meerkat.Hmac.Test.Security.Authentication
{
    [TestFixture]
    public class SignatureCacheFixture
    {
        [Test]
        public void CacheMiss()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var service = new SignatureCache(cache.Object);

            cache.Setup(x => x.Contains("A", "hmac")).Returns(false);
 
            var candidate = service.Contains("A");

            Assert.That(candidate, Is.False, "Cache entry differs");
        }

        [Test]
        public void CacheHit()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var service = new SignatureCache(cache.Object);

            cache.Setup(x => x.Contains("A", "hmac")).Returns(true);

            var candidate = service.Contains("A");

            Assert.That(candidate, Is.True, "Cache entry differs");
        }

        [Test]
        public void Add()
        {
            var cache = new Mock<ICache>(MockBehavior.Strict);
            var service = new SignatureCache(cache.Object);

            var dt = DateTimeOffset.UtcNow;

            cache.Setup(x => x.Set("A", "hmac signature", dt, "hmac"));

            service.Add("A", dt);
        }
    }
}
#endif