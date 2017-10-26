using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Meerkat.Owin.Security.Infrastructure;

using Microsoft.Owin;

using NUnit.Framework;

namespace Meerkat.Test.Owin.Security.Infrastructure
{
    [TestFixture]
    public class AuthorizationHeaderFixture
    {
        [Test]
        public void NoAuthenticationHeader()
        {
            var values = new Dictionary<string, string[]>();
            var headers = new HeaderDictionary(values);

            var candidate = headers.Authentication();

            Assert.That(candidate, Is.Null, "Unexpected authorization value: " + candidate);
        }

        [Test]
        public void NullAuthenticationHeader()
        {
            var values = new Dictionary<string, string[]>
            {
                ["Authorization"] = null
            };
            var headers = new HeaderDictionary(values);

            var candidate = headers.Authentication();

            Assert.That(candidate, Is.Null, "Unexpected authorization value: " + candidate);
        }

        [Test]
        public void ValidAuthenticationHeader()
        {
            var values = new Dictionary<string, string[]>
            {
                ["Authorization"] = new [] { "Bearer cn389ncoiwuencr" }
            };
            var headers = new HeaderDictionary(values);

            var expected = new AuthenticationHeaderValue("Bearer", "cn389ncoiwuencr");
            var candidate = headers.Authentication();

            Assert.That(candidate.Scheme, Is.EqualTo(expected.Scheme), "Scheme differs");
            Assert.That(candidate.Parameter, Is.EqualTo(expected.Parameter), "Parameter differs");
        }
    }
}