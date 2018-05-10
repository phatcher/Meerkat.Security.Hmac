using Meerkat.Caching;
using Meerkat.Security.Authentication;
using Meerkat.Security.Authentication.Hmac;

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
            //services.AddSingleton<ICache>(x => MemoryObjectCacheFactory.Default());
            services.AddHmacAuthenticator();

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
                options.Authority = $"https://login.microsoftonline.com/tfp/{Configuration["AzureAdB2C:Tenant"]}/{Configuration["AzureAdB2C:Policy"]}/v2.0/";
                options.Audience = Configuration["AzureAdB2C:ClientId"];
            })
            .AddHmac(options =>
            {
            });

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
