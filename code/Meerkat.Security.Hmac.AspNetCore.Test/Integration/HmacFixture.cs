using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

using Meerkat.Net.Http;
using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

using NUnit.Framework;

using Sample.AspNetCore;

namespace Meerkat.Hmac.Test.Integration
{
    [TestFixture]
    public class HmacFixture
    {
        private WebApplicationFactory<Startup> factory;

        [Test]
        public async Task Unauthenticated()
        {
            HmacClient.ClientId = null;

            var response = await Client.GetAsync(new Uri("/values/3", UriKind.Relative));

            var content = await response.Content.ReadAsStringAsync();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content, Is.EqualTo("/False"));
            });
        }

        [Test]
        public async Task UnauthenticatedAuthorized()
        {
            HmacClient.ClientId = null;

            var response = await Client.GetAsync(new Uri("/values/3/authenticated", UriKind.Relative));

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task Authenticated()
        {
            var response = await Client.GetAsync(new Uri("/values/3", UriKind.Relative));

            var content = await response.Content.ReadAsStringAsync();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content, Is.EqualTo("1234/True"));
            });
        }

        [Test]
        public async Task AuthenticatedAuthorized()
        {
            var response = await Client.GetAsync(new Uri("/values/3/authenticated", UriKind.Relative));

            var content = await response.Content.ReadAsStringAsync();
            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(content, Is.EqualTo("1234/True"));
            });
        }

        protected WebApplicationFactory<Startup> Factory => factory ?? (factory = CreateFactory());

        protected HttpClient Client { get; private set; }

        protected HmacClient HmacClient { get; set; }

        protected virtual WebApplicationFactory<Startup> CreateFactory()
        {
            var f = new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => builder.UseContentRoot("."));
            //{
            //    Environment = TestEnvironment
            //};

            var secretRepository = new SecretStore();
            secretRepository.Assign("1234", "ABCD");

            var mrb = new HmacMessageRepresentationBuilder();
            var calculator = new HmacSignatureCalculator();

            HmacClient = new HmacClient
            {
                ClientId = "1234"
            };
            var hmacHandler = new HmacClientHandler(HmacClient);
            var requestContentMd5Handler = new RequestContentMd5Handler();
            var hmacSigningHandler = new HmacSigningHandler(secretRepository, mrb, calculator);

            // Inject all the handlers in the correct order
            Client = f.CreateDefaultClient(hmacHandler, requestContentMd5Handler, hmacSigningHandler);

            //Startup = Program.Startup;

            return f;
        }

        [SetUp]
        public void Setup()
        {
            var x = Factory;
        }
    }
}
