using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using Serilog;
using Exceptionless;
using Microsoft.AspNetCore.HttpOverrides;

namespace UkrTrackingBot.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            Serilog.ILogger logger;

            if (env.IsDevelopment())
            {
                logger = new LoggerConfiguration()
                  .Enrich
                    .FromLogContext()
                  .WriteTo
                    .LiterateConsole()
                  .CreateLogger();

                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                ExceptionlessClient.Default.Startup();
                ExceptionlessClient.Default.Configuration.DefaultTags.Add("WebUI");
                ExceptionlessClient.Default.Configuration.ApiKey = Configuration.GetValue<string>("Exceptionless:ApiKey");

                logger = new LoggerConfiguration()
                  .Enrich
                    .FromLogContext()
                  .WriteTo
                    .LiterateConsole()                  
                  .WriteTo
                    .MongoDBCapped($"mongodb://{Configuration.GetValue<string>("MongoDbLogging:MongoDb")}:{Configuration.GetValue<string>("MongoDbLogging:MongoPassword")}@{Configuration.GetValue<string>("MongoDbLogging:MongoDbAddress")}", cappedMaxSizeMb: 250)
                  .WriteTo
                    .Logger(lc => 
                        lc
                            .MinimumLevel.Warning()
                            .WriteTo
                            .Exceptionless(b => b.AddTags("WebUI")))                    
                  .CreateLogger();

                app.UseExceptionHandler("/Home/Error");
            }

            loggerFactory.AddSerilog(logger);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies"
            });

            app.UseOpenIdConnectAuthentication(new OpenIdConnectOptions
            {
                AuthenticationScheme = "oidc",
                SignInScheme = "Cookies",

                Authority = env.IsDevelopment() ? Configuration.GetValue<string>("IdentityServer:authorityUrl") : Configuration.GetValue<string>("IDENTITYSERVER_AUTHORITYURL"),
                RequireHttpsMetadata = false,

                ClientId = "marketingmvcclient",
                ClientSecret = "marketingmvcsecret",

                ResponseType = "code id_token",
                Scope = { "npapi", "offline_access" },

                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true
            });

            app.UseStaticFiles();

            ForwardedHeadersOptions fordwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };
            fordwardedHeaderOptions.KnownNetworks.Clear();
            fordwardedHeaderOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(fordwardedHeaderOptions);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
