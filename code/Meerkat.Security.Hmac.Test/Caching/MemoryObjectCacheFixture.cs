using System;
using System.Threading;
using Meerkat.Caching;

using NUnit.Framework;

namespace Meerkat.Test.Caching
{
    [TestFixture]
    public class MemoryObjectCacheFixture
    {
        private MemoryObjectCache cache;

        [Test]
        public void AddItem()
        {
            cache.AddOrGetExisting("A", "A", DateTimeOffset.UtcNow.AddMinutes(1));

        }

        [Test]
        public void Retrieve()
        {
            cache.AddOrGetExisting("B", "A", DateTimeOffset.UtcNow.AddMinutes(1));

            var candidate = cache.Contains("B");

            Assert.That(candidate, Is.EqualTo(true));
        }

        [Test]
        public void RetrieveNonExistent()
        {
            var candidate = cache.Contains("C");

            Assert.That(candidate, Is.EqualTo(false));
        }

        [Test]
        public void RetrieveExpired()
        {
            cache.AddOrGetExisting("D", "D", DateTimeOffset.UtcNow.AddSeconds(1));

            // Need to wait 20s for the cache to expire
            // See http://stackoverflow.com/questions/1434284/when-does-asp-net-remove-expired-cache-items
            Thread.Sleep(21000);

            var candidate = cache.Contains("D");

            Assert.That(candidate, Is.EqualTo(false));
        }

        [SetUp]
        public void Setup()
        {
            cache = new MemoryObjectCache();
        }
    }
}