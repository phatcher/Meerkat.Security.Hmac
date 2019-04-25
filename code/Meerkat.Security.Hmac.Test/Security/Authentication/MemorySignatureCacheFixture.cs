#if NETCOREAPP
using System;

using Meerkat.Security.Authentication;

using Microsoft.Extensions.Caching.Memory;

using Moq;

using NUnit.Framework;

namespace Meerkat.Hmac.Test.Security.Authentication
{
    [TestFixture]
    public class MemorySignatureCacheFixture
    {
        [Test]
        public void CacheMiss()
        {
            var cache = new Mock<IMemoryCache>(MockBehavior.Strict);
            var service = new MemorySignatureCache(cache.Object);

            object value = null;
            cache.Setup(x => x.TryGetValue("hmac:A", out value)).Returns(false);
 
            var candidate = service.Contains("A");

            Assert.That(candidate, Is.False, "Cache entry differs");
        }

        [Test]
        public void CacheHit()
        {
            var cache = new Mock<IMemoryCache>(MockBehavior.Strict);
            var service = new MemorySignatureCache(cache.Object);

            object value = "B";
            cache.Setup(x => x.TryGetValue("hmac:A", out value)).Returns(true);

            var candidate = service.Contains("A");

            Assert.That(candidate, Is.True, "Cache entry differs");
        }

        [Test]
        public void Add()
        {
            var cache = new Mock<IMemoryCache>(MockBehavior.Strict);
            var service = new MemorySignatureCache(cache.Object);

            var dt = DateTimeOffset.UtcNow;

            var entry = new Mock<ICacheEntry>();
            cache.Setup(x => x.CreateEntry("hmac:A")).Returns(entry.Object);

            service.Add("A", dt);
        }
    }
}
#endif