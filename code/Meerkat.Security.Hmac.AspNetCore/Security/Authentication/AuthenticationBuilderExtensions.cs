using System;

using Microsoft.AspNetCore.Authentication;

namespace Meerkat.Security.Authentication
{
    public static class AuthenticationBuilderExtensions
    {
        /// <summary>
        /// Add HMAC authentication to the ASP.NET pipeline.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configureOptions"></param>
        /// <returns></returns>
        public static AuthenticationBuilder AddHmacAuthentication(this AuthenticationBuilder builder, Action<HmacAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<HmacAuthenticationOptions, HmacAuthenticationHandler>(HmacAuthenticationOptions.DefaultScheme, configureOptions);
        }
    }
}