using System;
using System.Net.Http;
using Meerkat.Net.Http;

using NUnit.Framework;

namespace Meerkat.Test.Net.Http
{
    [TestFixture]
    public class RequestHeaderFixture
    {
        [Test]
        public void NotPresentHeaderReturnsDefault()
        {
            var message = new HttpRequestMessage();

            var candidate = message.Headers.GetFirstOrDefaultValue<string>("test");

            Assert.That(candidate, Is.Null);
        }

        [Test]
        public void NotPresentIntegerHeaderReturnsDefault()
        {
            var message = new HttpRequestMessage();

            var candidate = message.Headers.GetFirstOrDefaultValue<int>("test");

            Assert.That(candidate, Is.EqualTo(0));
        }

        [Test]
        public void HeaderPresent()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add("test", "A");

            var candidate = message.Headers.GetFirstOrDefaultValue<string>("test");

            Assert.That(candidate, Is.EqualTo("A"));
        }

        [Test]
        public void FirstValuePresent()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add("test", new [] { "A", "B", "C" });

            var candidate = message.Headers.GetFirstOrDefaultValue<string>("test");

            Assert.That(candidate, Is.EqualTo("A"));
        }

        [Test]
        public void IntegerHeader()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add("test", "1");

            var candidate = message.Headers.GetFirstOrDefaultValue<int>("test");

            Assert.That(candidate, Is.EqualTo(1));
        }

        [Test]
        public void ConversionErrorThrowsException()
        {
            var message = new HttpRequestMessage();

            message.Headers.Add("test", "A");

            try
            {
                message.Headers.GetFirstOrDefaultValue<int>("test");
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

            Assert.That(candidate, Is.EqualTo(date.ToString("u")));
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