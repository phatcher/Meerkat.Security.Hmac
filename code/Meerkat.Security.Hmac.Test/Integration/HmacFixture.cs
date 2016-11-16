﻿using System;
using System.Net.Http;
using System.Threading.Tasks;

using Meerkat.Net.Http;
using Meerkat.Security;

using Microsoft.Owin.Testing;
using Microsoft.Practices.Unity;

using NUnit.Framework;

namespace Meerkat.Test.Integration
{
    public class HmacFixture
    {
        protected IUnityContainer Container { get; set; }

        public async Task OnAllowAnonymous()
        {
            using (var server = TestServer.Create<Sample.Web.Startup>())
            {
                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/insecure");
                var response = await client.SendAsync(request);

                Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code differs");
                Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("[\"C\",\"D\"]"), "Content differs");
            }
        }

        public async Task OnSecured()
        {
            using (var server = TestServer.Create<Sample.Web.Startup>())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234");
                var response = await client.SendAsync(request);

                Assert.That((int)response.StatusCode, Is.EqualTo(200), "Status code differs");
                Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("[\"A\",\"B\"]"), "Content differs");
            }
        }

        public async Task OnSecureNoHmac(int status)
        {
            using (var server = TestServer.Create<Sample.Web.Startup>())
            {
                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure");
                var response = await client.SendAsync(request);

                // TODO: Why 500 rather than 401?
                Assert.That((int)response.StatusCode, Is.EqualTo(status), "Status code differs");
                //Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("[\"C\",\"D\"]"), "Content differs");
            }
        }

        public async Task OnInvalidClientId(int status)
        {
            using (var server = TestServer.Create<Sample.Web.Startup>())
            {
                // We sign it, but the server doesn't know this client id
                AssignSecret("1234", "1234", false);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234");
                var response = await client.SendAsync(request);

                // TODO: Why 500 rather than 401?
                Assert.That((int)response.StatusCode, Is.EqualTo(status), "Status code differs");
                //Assert.That(await response.Content.ReadAsStringAsync(), Is.EqualTo("[\"C\",\"D\"]"), "Content differs");
            }
        }

        public async Task OnReplayAttack()
        {
            using (var server = TestServer.Create<Sample.Web.Startup>())
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
            }
        }

        public async Task OnNoncedRequests()
        {
        }

        public async Task OnMessageDateTooEarly()
        {
            using (var server = TestServer.Create<Sample.Web.Startup>())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234");
                request.Headers.Date = DateTimeOffset.UtcNow.AddHours(-2);
                var response = await client.SendAsync(request);

                // TODO: Why 500 rather than 401?
                Assert.That((int)response.StatusCode, Is.EqualTo(500), "Status code differs");
            }

        }

        public async Task OnMessageDateTooLate()
        {
            using (var server = TestServer.Create<Sample.Web.Startup>())
            {
                AssignSecret("1234", "1234", true);

                var client = HmacClient(server.Handler);
                var request = RequestMessage("/api/values/secure", "1234");
                request.Headers.Date = DateTimeOffset.UtcNow.AddHours(2);
                var response = await client.SendAsync(request);

                // TODO: Why 500 rather than 401?
                Assert.That((int)response.StatusCode, Is.EqualTo(500), "Status code differs");
            }
        }

        [SetUp]
        public virtual void SetUp()
        {
            // NB Need this to ensure we don't retain state between tests
            Container = new UnityContainer();
            Sample.Web.UnityConfig.RegisterHmacCore(Container);
            Container.RegisterType<HmacSigningHandler, HmacSigningHandler>();

            // Wipe down the server container
            Sample.Web.Startup.Reset();

            // Standard behaviour
            Sample.Web.Startup.UseMvc = false;
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

        private HttpRequestMessage RequestMessage(string url, string clientId = null, bool addNonce = false)
        {
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(url, UriKind.Relative),
                Method = HttpMethod.Get
            };
            if (!string.IsNullOrEmpty(clientId))
            {
                request.Headers.Add(HmacAuthentication.ClientIdHeader, clientId);
            }

            // TODO: Add nonce header if required

            return request;
        }

        private HttpClient HmacClient(HttpMessageHandler baseHandler)
        {
            var client = new HttpClient(ClientHandler(baseHandler))
            {
                // NB Dummy base address is mandatory for this to work.
                BaseAddress = new Uri("http://sample.com")
            };

            return client;
        }

        private HttpMessageHandler ClientHandler(HttpMessageHandler baseHandler)
        {
            var handler1 = new RequestContentMd5Handler();
            // NB Need a new one each time as it gets disposed
            var handler2 = Container.Resolve<HmacSigningHandler>();

            handler1.InnerHandler = handler2;
            handler2.InnerHandler = baseHandler;

            return handler1;
        }
    }
}