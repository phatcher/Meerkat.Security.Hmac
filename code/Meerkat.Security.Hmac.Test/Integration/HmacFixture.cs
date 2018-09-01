using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

using Meerkat.Net.Http;
using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

using Microsoft.Owin.Testing;

using NUnit.Framework;

using Unity;

namespace Meerkat.Hmac.Test.Integration
{
    public class HmacFixture
    {
        protected IUnityContainer Container { get; set; }

        protected async Task OnAllowAnonymous()
        {
            using (var server = InitializeWeb())
            {
                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/insecure");
                var response = await client.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("OK"), "Reason phrase differs");
                Assert.That(result, Is.EqualTo("[\"C\",\"D\"]"), "Content differs");
            }
        }

        protected async Task OnSecured()
        {
            using (var server = InitializeWeb())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234");
                var response = await client.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("OK"), "Reason phrase differs");
                Assert.That(result, Is.EqualTo("[\"A\",\"B\"]"), "Content differs");
            }
        }

        protected async Task OnSecuredZeroLength()
        {
            using (var server = InitializeWeb())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234", content: string.Empty);
                var response = await client.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("OK"), "Reason phrase differs");
                Assert.That(result, Is.EqualTo("[\"A\",\"B\"]"), "Content differs");
            }
        }

        protected async Task OnSecureNoHmac()
        {
            using (var server = InitializeWeb())
            {
                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure");
                var response = await client.SendAsync(request);

                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(401), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("Unauthorized"), "Reason phrase differs");
            }
        }

        protected async Task OnPost()
        {
            using (var server = InitializeWeb())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/update", "1234", method: "POST", content: "{ B = 1 }");
                var response = await client.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("OK"), "Reason phrase differs");
                Assert.That(result, Is.EqualTo("1"), "Content differs");
            }
        }

        protected async Task OnPostZeroLength()
        {
            using (var server = InitializeWeb())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/update", "1234", method: "POST", content: string.Empty);
                var response = await client.SendAsync(request);

                var result = await response.Content.ReadAsStringAsync();
                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("OK"), "Reason phrase differs");
                Assert.That(result, Is.EqualTo("1"), "Content differs");
            }
        }

        protected async Task OnPostNoHmac()
        {
            using (var server = InitializeWeb())
            {
                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/update", "1234", method: "POST", content: "{ B = 1 }");
                var response = await client.SendAsync(request);

                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(401), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("Unauthorized"), "Reason phrase differs");
            }
        }

        protected async Task OnInvalidScheme()
        {
            using (var server = InitializeWeb())
            {
                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure");

                var header = new AuthenticationHeaderValue("Foo", "Bar");
                request.Headers.Authorization = header;

                var response = await client.SendAsync(request);

                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(401), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("Unauthorized"), "Reason phrase differs");
            }
        }

        protected async Task OnInvalidClientId()
        {
            using (var server = InitializeWeb())
            {
                // We sign it, but the server doesn't know this client id
                AssignSecret("1234", "1234", false);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234");
                var response = await client.SendAsync(request);

                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(401), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("Invalid signature"), "Reason phrase differs");
            }
        }

        protected async Task OnMissingSignature()
        {
            using (var server = InitializeWeb())
            {
                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure");

                var header = new AuthenticationHeaderValue(HmacAuthentication.AuthenticationScheme, null);
                request.Headers.Authorization = header;

                var response = await client.SendAsync(request);

                Console.Out.WriteLine("Status: {0} - {1}", response.StatusCode, response.ReasonPhrase);
                Assert.That((int)response.StatusCode, Is.EqualTo(401), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("Missing credentials"), "Reason phrase differs");
            }
        }

        protected async Task OnReplayAttack()
        {
            using (var server = InitializeWeb())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234");
                var response = await client.SendAsync(request);

                Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code differs");
                Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("[\"A\",\"B\"]"), "Content differs");

                request = RequestMessage("/api/values/secure", "1234");
                response = await client.SendAsync(request);

                Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("OK"), "Reason phrase differs");
            }
        }

        protected async Task OnNoncedRequests()
        {
        }

        protected async Task OnMessageDateTooEarly()
        {
            using (var server = InitializeWeb())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234");
                request.Headers.Date = DateTimeOffset.UtcNow.AddHours(-2);
                var response = await client.SendAsync(request);

                Assert.That((int)response.StatusCode, Is.EqualTo(401), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("OK"), "Reason phrase differs");
            }
        }

        protected async Task OnMessageDateTooLate()
        {
            using (var server = InitializeWeb())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234");
                request.Headers.Date = DateTimeOffset.UtcNow.AddHours(2);
                var response = await client.SendAsync(request);

                Assert.That((int)response.StatusCode, Is.EqualTo(401), "Status code differs");
                Assert.That(response.ReasonPhrase, Is.EqualTo("OK"), "Reason phrase differs");
            }
        }

        protected void CheckTransient<T>()
        {
            var value = Container.Resolve<T>();
            var second = Container.Resolve<T>();

            Assert.That(value, Is.Not.SameAs(second), "Values same");
        }

        protected void CheckSingleton<T>()
        {
            var value = Container.Resolve<T>();
            var second = Container.Resolve<T>();

            Assert.That(value, Is.SameAs(second), "Values not same");
        }

        protected virtual TestServer InitializeWeb()
        {
            var server = TestServer.Create<Sample.Web.Startup>();
            //Container = Sample.Web.UnityConfig.Container;

            return server;
        }

        [SetUp]
        public virtual void SetUp()
        {
            // Enough that we can sign HMAC messages
            Container = new UnityContainer();
            Sample.Web.UnityConfig.RegisterHmacCore(Container);
            Container.RegisterType<HmacSigningHandler, HmacSigningHandler>();

            // Wipe down the server container
            Sample.Web.Startup.Reset();

            // Standard behaviour
            Sample.Web.Startup.SelfHost = true;
            Sample.Web.Startup.IgnoreMvc = true;
            Sample.Web.Startup.UseOwinHmac = false;
        }

        [TearDown]
        public virtual void TearDown()
        {
            Container = null;
            Sample.Web.Startup.Reset();
        }

        private void AssignSecret(string clientId, string secret, bool storeServer)
        {
            var localStore = Container.Resolve<ISecretStore>();
            localStore.Assign(clientId, secret);

            if (storeServer)
            {
                var serverStore = Sample.Web.UnityConfig.GetConfiguredContainer().Resolve<ISecretStore>();
                serverStore.Assign(clientId, secret);
            }
        }

        private HttpRequestMessage RequestMessage(string url, string clientId = null, bool addNonce = false, string method = "GET", string content = null)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url, UriKind.Relative),
                Method = new HttpMethod(method)
            };

            if (!string.IsNullOrEmpty(clientId))
            {
                request.Headers.Add(HmacAuthentication.ClientIdHeader, clientId);
            }

            if (content != null)
            {
                request.Content = new StringContent(content);
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            // TODO: Add nonce header if required

            return request;
        }

        private HttpClient HmacClient(HttpMessageHandler baseHandler)
        {
            var client = new HttpClient(ClientHandler(baseHandler), false)
            {
                // NB Dummy base address is mandatory for this to work.
                BaseAddress = new Uri("http://sample.com")
            };

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }

        private HttpMessageHandler ClientHandler(HttpMessageHandler baseHandler)
        {
            var md5Handler = new RequestContentMd5Handler();
            var signingHandler = Container.Resolve<HmacSigningHandler>();

            md5Handler.InnerHandler = signingHandler;
            signingHandler.InnerHandler = baseHandler;

            return md5Handler;
        }
    }
}