using Meerkat.Security.Authentication.Hmac;
using Meerkat.Security.Authorization;

using Microsoft.Extensions.DependencyInjection;

namespace Meerkat.Security.Authentication
{
    public static class ServiceConfigurationExtensions
    {
        /// <summary>
        /// Adds services for validating HMAC signatures.
        /// <para>
        /// Additionally a <see cref="ISecretRepository"/> and <see cref="IRequestClaimsProvider"/> must be registered externally.
        /// </para>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="signatureDuration">How long to cache signatures in minutes to avoid replay attacks (default 10 min)</param>
        /// <param name="clockDrift">How much clock drift to allow between client and server in minutes (default 10 min)</param>
        /// <param name="nameClaimType">Name of the name claim type</param>
        /// <param name="roleClaimType">Name of the role claim type</param>
        /// <returns></returns>
        public static IServiceCollection AddHmacAuthenticator(this IServiceCollection services, int signatureDuration = 10, int clockDrift = 10, string nameClaimType = "name", string roleClaimType = "role")
        {
            services.AddTransient<IMessageRepresentationBuilder, HmacMessageRepresentationBuilder>();
            services.AddTransient<ISignatureCalculator, HmacSignatureCalculator>();

            // Scope this as the ISecretStore access will need to be scoped
            services.AddScoped<ISignatureValidator>(x => new HmacSignatureValidator(x.GetRequiredService<ISignatureCalculator>(), 
                                                                                    x.GetRequiredService<IMessageRepresentationBuilder>(),
                                                                                    x.GetRequiredService<ISecretRepository>(),
                                                                                    x.GetRequiredService<ISignatureCache>(),
                                                                                    signatureDuration, clockDrift));

            services.AddScoped<IHmacAuthenticator>(x => new HmacAuthenticator(x.GetRequiredService<ISignatureValidator>(),
                                                                              x.GetRequiredService<IRequestClaimsProvider>(),
                                                                              nameClaimType, roleClaimType));

            return services;
        }
    }
}
