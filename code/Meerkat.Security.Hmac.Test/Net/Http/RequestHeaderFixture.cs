using System;
using System.Linq;
using System.Net.Http;

using Meerkat.Net.Http;

using NUnit.Framework;

namespace Meerkat.Hmac.Test.Net.Http
{
    [TestFixture]
    public class RequestHeaderFixture
    {
        [Test]
        public void NotPresentHeaderReturnsDefault()
        {
            var message = new HttpRequestMessage();

            var candidate = message.Headers.GetValues<string>("test").FirstOrDefault();

            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void NotPresentIntegerHeaderReturnsDefault()
        {
            var message = new HttpRequestMessage();

            var candidate = message.Headers.GetValues<int>("test").FirstOrDefault();

            Assert.That(candidate, Is.EqualTo(0));
        }

        [Test]
        public void HeaderPresent()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add("test", "A");

            var candidate = message.Headers.GetValues<string>("test").FirstOrDefault();

            Assert.That(candidate, Is.EqualTo("A"));
        }

        [Test]
        public void FirstValuePresent()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add("test", new [] { "A", "B", "C" });

            var candidate = message.Headers.GetValues<string>("test").FirstOrDefault();

            Assert.That(candidate, Is.EqualTo("A"));
        }

        [Test]
        public void MultipleValuesPresent()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add("test", new[] { "A", "B", "C" });

            var candidate = message.Headers.GetValues<string>("test").ToList();

            Assert.Multiple(() =>
            {
                Assert.That(candidate.Count, Is.EqualTo(3));
                Assert.That(candidate[0], Is.EqualTo("A"));
                Assert.That(candidate[1], Is.EqualTo("B"));
                Assert.That(candidate[2], Is.EqualTo("C"));
            });
        }

        [Test]
        public void IntegerHeader()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add("test", "1");

            var candidate = message.Headers.GetValues<int>("test").FirstOrDefault();

            Assert.That(candidate, Is.EqualTo(1));
        }

        [Test]
        public void ConversionErrorThrowsException()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add("test", "A");

            try
            {
                message.Headers.GetValues<int>("test").FirstOrDefault();
            }
            catch (FormatException ex)
            {
                Assert.That(ex.Message, Is.EqualTo("Input string was not in a correct format."));
                return;
            }

            throw new Exception("Expected FormatException");
        }

        [Test]
        public void NoTimestampReturnsNull()
        {
            var message = new HttpRequestMessage();

            var candidate = message.Headers.MessageDate();

            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void MessageTimestamp()
        {            
            var message = new HttpRequestMessage();

            var date = DateTime.UtcNow;

            message.Headers.Date = date;

            var candidate = message.Headers.MessageDate();

            Assert.That(candidate, Is.EqualTo(date.ToString("r")));
        }

        [Test]
        public void MessageDateIsInvalidWhenNotPresent()
        {
            var message = new HttpRequestMessage();

            var candidate = message.IsMessageDateValid(5);

            Assert.That(candidate, Is.EqualTo(false));
        }

        [TestCase(0, true)]
        [TestCase(4, true)]
        [TestCase(-4, true)]
        [TestCase(6, false)]
        [TestCase(-6, false)]
        public void MessageDateValidity(int minutes, bool expected)
        {
            var message = new HttpRequestMessage();

            var date = DateTime.UtcNow.AddMinutes(minutes);

            message.Headers.Date = date;

            var candidate = message.IsMessageDateValid(5);

            Assert.That(candidate, Is.EqualTo(expected));
        }
    }
}