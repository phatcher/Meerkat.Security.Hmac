#if NETCOREAPP
using System;
using System.Text;

using Meerkat.Security.Authentication;

using Microsoft.Extensions.Caching.Distributed;

using Moq;

using NUnit.Framework;

namespace Meerkat.Hmac.Test.Security.Authentication
{
    public class DistributedSignatureCacheFixture
    {
        [Test]
        public void CacheMiss()
        {
            var cache = new Mock<IDistributedCache>(MockBehavior.Strict);
            var service = new DistributedSignatureCache(cache.Object);

            cache.Setup(x => x.Get("hmac:A")).Returns((byte[]) null);
 
            var candidate = service.Contains("A");

            Assert.That(candidate, Is.False, "Cache entry differs");
        }

        [Test]
        public void CacheHit()
        {
            var cache = new Mock<IDistributedCache>(MockBehavior.Strict);
            var service = new DistributedSignatureCache(cache.Object);

            cache.Setup(x => x.Get("hmac:A")).Returns(new byte[1]);

            var candidate = service.Contains("A");

            Assert.That(candidate, Is.True, "Cache entry differs");
        }

        [Test]
        public void Add()
        {
            var cache = new Mock<IDistributedCache>(MockBehavior.Strict);
            var service = new DistributedSignatureCache(cache.Object);

            var dt = DateTimeOffset.UtcNow;

            cache.Setup(x => x.Set("hmac:A", It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>()));

            service.Add("A", dt);
        }
    }
}
#endif