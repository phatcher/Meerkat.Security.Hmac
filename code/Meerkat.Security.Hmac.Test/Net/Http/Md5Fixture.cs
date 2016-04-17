using System;
using System.Net.Http;
using System.Threading.Tasks;
using Meerkat.Net.Http;
using NUnit.Framework;

namespace Meerkat.Test.Net.Http
{
    [TestFixture]
    public class Md5Fixture
    {
        [Test]
        public async Task AssignM5Header()
        {
            var content = new StringContent("Test");
            
            var expected = await content.ComputeMd5Hash();

            await content.AssignMd5Hash();

            var candidate = content.Headers.ContentMD5;

            Assert.That(expected, Is.EqualTo(candidate));
        }

        [Test]
        public void ToBase64Null()
        {
            HttpContent content = null;

            var candidate = content.Md5Base64();

            Assert.That(candidate, Is.EqualTo(string.Empty));
        }

        [Test]
        public void ToBase64NoHeader()
        {
            HttpContent content = new StringContent("Test");

            var candidate = content.Md5Base64();

            Assert.That(candidate, Is.EqualTo(string.Empty));
        }

        [Test]
        public async Task ToBase64()
        {
            var content = new StringContent("Test");

            var hash = await content.ComputeMd5Hash();
            var expected = Convert.ToBase64String(hash);

            await content.AssignMd5Hash();

            var candidate = content.Md5Base64();

            Assert.That(expected, Is.EqualTo(candidate));
        }

        [Test]
        public async Task Md5ValidWithNoContent()
        {
            HttpContent content = null;

            var candidate = await content.IsMd5Valid();

            Assert.That(candidate, Is.EqualTo(true));
        }

        [Test]
        public async Task Md5ValidWithNoHeader()
        {
            var content = new StringContent("Test");

            var candidate = await content.IsMd5Valid();

            Assert.That(candidate, Is.EqualTo(true));
        }

        [Test]
        public async Task Md5Valid()
        {
            var content = new StringContent("Test");
            await content.AssignMd5Hash();

            var candidate = await content.IsMd5Valid();

            Assert.That(candidate, Is.EqualTo(true));
        }

        [Test]
        public async Task Md5Invalid()
        {
            var content = new StringContent("Test");
            await content.AssignMd5Hash();
            
            content.Headers.ContentMD5 = new byte[] { 0x1, 0x2 };

            var candidate = await content.IsMd5Valid();

            Assert.That(candidate, Is.EqualTo(false));
        }
    }
}