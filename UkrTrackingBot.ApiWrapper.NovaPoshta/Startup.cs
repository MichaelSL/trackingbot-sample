using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.Exceptionless;
using Exceptionless;

namespace UkrTrackingBot.ApiWrapper.NovaPoshta
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

            Console.WriteLine(env.IsDevelopment());
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            ServicesRegistration(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "UkrTrackingBot.ApiWrapper.NovaPoshta", Version = "v1" });
            });
        }

        private void ServicesRegistration(IServiceCollection services)
        {
            services.AddScoped<Domain.IDataProvider, Domain.HttpDataProvider>();
            services.AddScoped<Domain.Waybills.IWaybillsService, Domain.Waybills.WaybillsService>();
            services.AddSingleton(_ =>
            {
                var section = Configuration.GetSection("NovaPoshta");
                var config = section.Get<Domain.Configuration.NovaPoshtaConfigurationOptions>();
                config.ApiKey = Configuration.GetValue<string>("API_KEY");
                return config;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
                        ILoggerFactory loggerfactory,
                        IApplicationLifetime appLifetime)
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
                ExceptionlessClient.Default.Configuration.DefaultTags.Add("ApiWrapper.NovaPoshta");
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
                            .Exceptionless(b => b.AddTags("ApiWrapper.NovaPoshta")))
                  .CreateLogger();
            }

            loggerfactory.AddSerilog(logger);

            app.UseMvc();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                string virtualPath = Configuration.GetValue<String>("VirtualPath");
                c.SwaggerEndpoint(virtualPath + "/swagger/v1/swagger.json", "UkrTrackingBot.ApiWrapper.NovaPoshta v1");
            });
        }
    }
}
