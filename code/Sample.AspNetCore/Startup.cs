﻿using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;
using Meerkat.Security.Authorization;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sample.AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // See https://stackoverflow.com/questions/45695382/how-do-i-setup-multiple-auth-schemes-in-asp-net-core-2-0
            // and https://github.com/aspnet/Security/issues/1469
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = "smart";
                sharedOptions.DefaultChallengeScheme = "smart";
            })
            .AddPolicyScheme("smart", "JWT or HMAC", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    var authHeader = context.Request.Headers["Authorization"].ToString();
                    if (authHeader?.StartsWith("Bearer") == true)
                    {
                        return JwtBearerDefaults.AuthenticationScheme;
                    }

                    return HmacAuthentication.AuthenticationScheme;
                };
            })
            .AddJwtBearer(options =>
            {
                options.Authority = "https://foo.com/";
                options.Audience = "aud";
            })
            .AddHmacAuthentication(options =>
            {
            });

            services.AddHmacAuthenticator();

            services.AddDistributedMemoryCache();
            services.AddSingleton<ISignatureCache, DistributedSignatureCache>();

            var secretRepository = new SecretStore();
            secretRepository.Assign("1234", "ABCD");
            services.AddSingleton<ISecretRepository>(secretRepository);
            services.AddTransient<IRequestClaimsProvider>(x => new ClientIdRequestClaimsProvider("name"));
            
            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
