using Meerkat.Security.Authentication.Hmac;

using Microsoft.Extensions.DependencyInjection;

namespace Meerkat.Security.Authentication
{
    public static class ServiceConfigurationExtensions
    {
        /// <summary>
        /// Adds services for validating HMAC signatures.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="signatureDuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddHmacAuthenticator(this IServiceCollection services, int signatureDuration = 10)
        {
            services.AddTransient<IMessageRepresentationBuilder, MessageRepresentationBuilder>();
            services.AddTransient<ISignatureCalculator, HmacSignatureCalculator>();

            services.AddTransient<ISignatureValidator, HmacSignatureValidator>();

            services.AddTransient<IHmacAuthenticator, HmacAuthenticator>();

            return services;
        }
    }
}
