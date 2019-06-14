using System;
using System.Net.Http;

using Meerkat.Net.Http;
using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

using Microsoft.Extensions.DependencyInjection;

using NUnit.Framework;

using StructureMap;

namespace Meerkat.Hmac.Test.Integration
{
    [TestFixture]
    public class HmacHttpClient
    {
        [Test]
        public void ServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddHttpClient("client")
                    .AddHttpMessageHandler<RequestContentMd5Handler>()
                    .AddHttpMessageHandler<HmacSigningHandler>();

            services.AddSingleton<ISecretRepository, SecretStore>();
            services.AddTransient<ISignatureCalculator, HmacSignatureCalculator>();
            services.AddTransient<IMessageRepresentationBuilder, HmacMessageRepresentationBuilder>();

            services.AddTransient<RequestContentMd5Handler>();
            services.AddTransient<HmacSigningHandler>();

            var serviceProvider = services.BuildServiceProvider();

            var factory = serviceProvider.GetService<IHttpClientFactory>();

            factory.CreateClient("client");
        }

        [Test]
        public void HttpClient()
        {
            var services = new ServiceCollection();

            services.AddHttpClient("client")
                    .AddHttpMessageHandler<RequestContentMd5Handler>()
                    .AddHttpMessageHandler<HmacSigningHandler>();

            var container = new StructureMap.Container();
            container.Configure(config =>
            {
                config.For<ISecretRepository>().Singleton().Use(new SecretStore());

                config.For<ISignatureCalculator>().Use<HmacSignatureCalculator>();
                config.For<IMessageRepresentationBuilder>().Use<HmacMessageRepresentationBuilder>();

                config.For<RequestContentMd5Handler>().Use<RequestContentMd5Handler>();
                config.For<HmacSigningHandler>().Use<HmacSigningHandler>();

                config.Populate(services);
            });

            var serviceProvider = container.GetInstance<IServiceProvider>();

            var factory = serviceProvider.GetService<IHttpClientFactory>();

            factory.CreateClient("client");
        }

        [Test]
        public void Lamar()
        {
            var services = new ServiceCollection();

            services.AddHttpClient("client")
                    .AddHttpMessageHandler<RequestContentMd5Handler>()
                    .AddHttpMessageHandler<HmacSigningHandler>();

            var container = new Lamar.Container(services);
            container.Configure(config =>
            {
                config.AddSingleton<ISecretRepository, SecretStore>();
                config.AddTransient<ISignatureCalculator, HmacSignatureCalculator>();
                config.AddTransient<IMessageRepresentationBuilder, HmacMessageRepresentationBuilder>();

                config.AddTransient<RequestContentMd5Handler>();
            });

            var serviceProvider = container.GetInstance<IServiceProvider>();

            var factory = serviceProvider.GetService<IHttpClientFactory>();

            factory.CreateClient("client");
        }

        [Test]
        public void SimpleInjector()
        {
            var services = new ServiceCollection();

            services.AddHttpClient("client")
                    .AddHttpMessageHandler<RequestContentMd5Handler>()
                    .AddHttpMessageHandler<HmacSigningHandler>();

            var container = new SimpleInjector.Container();
            //container.Configure(config =>
            //{
            //    config.AddSingleton<ISecretRepository, SecretStore>();
            //    config.AddTransient<ISignatureCalculator, HmacSignatureCalculator>();
            //    config.AddTransient<IMessageRepresentationBuilder, HmacMessageRepresentationBuilder>();

            //    config.AddTransient<RequestContentMd5Handler>();
            //});

            var serviceProvider = container.GetInstance<IServiceProvider>();

            var factory = serviceProvider.GetService<IHttpClientFactory>();

            factory.CreateClient("client");
        }
    }
}
