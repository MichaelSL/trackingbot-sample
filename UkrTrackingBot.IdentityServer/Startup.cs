using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UkrTrackingBot.IdentityServer.Data;
using UkrTrackingBot.IdentityServer.Models;
using UkrTrackingBot.IdentityServer.Services;
using System.Security.Cryptography.X509Certificates;
using Serilog;
using Exceptionless;
using Microsoft.AspNetCore.HttpOverrides;

namespace UkrTrackingBot.IdentityServer
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(o =>
            {
                o.Password.RequireDigit = false;
                o.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddScoped(_ => Log.Logger);

            var redirectUriConfig = Configuration.GetValue<string>("IdentityServerConfig:MvcClient:RedirectUri");
            var postLogoutRedirectUri = Configuration.GetValue<string>("IdentityServerConfig:MvcClient:PostLogoutRedirectUri");

            var signingCertificate = new X509Certificate2(@".sign-cert/sign.pfx", Configuration.GetValue<string>("SIGN_CERT_PASSWORD"));
            services.AddIdentityServer()
                .AddSigningCredential(signingCertificate)
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApiResources())
                .AddInMemoryClients(Config.GetClients(new MvcClientConfigValues
                {
                    RedirectUri = redirectUriConfig ?? Configuration.GetValue<string>("MVC_REDIRECT_URI"),
                    PostLogoutRedirectUri = postLogoutRedirectUri ?? Configuration.GetValue<string>("MVC_POST_LOGOUT_REDIRECT_URI")
                }))
                .AddAspNetIdentity<ApplicationUser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime)
        {
            Serilog.ILogger logger;

            if (env.IsDevelopment())
            {
                logger = new LoggerConfiguration()
                  .Enrich
                    .FromLogContext()
                  .WriteTo
                    .LiterateConsole()
                  .CreateLogger();
            }
            else
            {
                ExceptionlessClient.Default.Startup();
                ExceptionlessClient.Default.Configuration.DefaultTags.Add("IdentityServer");
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
                            .Exceptionless(b => b.AddTags("IdentityServer")))
                  .CreateLogger();
            }

            loggerFactory.AddSerilog(logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            ForwardedHeadersOptions fordwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false
            };
            fordwardedHeaderOptions.KnownNetworks.Clear();
            fordwardedHeaderOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(fordwardedHeaderOptions);

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715
            app.UseIdentityServer();

            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
