using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Serilog;
using UkrTrackingBot.Bot.Common.Services;
using UkrTrackingBot.Telegram.Web.Services;

namespace UkrTrackingBot.Telegram.Web
{
    public class Startup
    {
        private static Dictionary<string, Bot.Common.Services.WebClientProxy> webClientProxies = new Dictionary<string, Bot.Common.Services.WebClientProxy>();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            ServicesRegistration(services);

            services.AddMvc();
        }

        private void ServicesRegistration(IServiceCollection services)
        {
            var token = Configuration.GetValue<string>("BOT_TOKEN");
            var delAddress = Configuration.GetValue<string>("DEL_ADDRESS");
            var npAddress = Configuration.GetValue<string>("NP_ADDRESS");
            var redisAddress = Configuration.GetValue<string>("REDIS_ADDRESS");

            services.AddSingleton<Caching.ICache<string>>(_ => new Caching.Redis.RedisCache(redisAddress));
            services.AddSingleton(_ => new WebClientProxy(npAddress, delAddress, _.GetService<Caching.ICache<string>>(), _.GetService<ILogger<WebClientProxy>>()));
            services.AddSingleton<ICommunicationChannel>(_ => new TelegramCommunicationChannel(token));
            services.AddScoped<ITrackingBotService, TrackingBotService>();
            services.AddScoped<ICommandArgsParser, TelegramCommandArgsParser>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
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
                ExceptionlessClient.Default.Configuration.DefaultTags.Add("TelegramWebhook");
                ExceptionlessClient.Default.Configuration.ApiKey = Configuration.GetValue<string>("Exceptionless:ApiKey");

                string logsDbName = Configuration.GetValue<string>("MongoDbLogging:MongoDb");
                string logsDbPass = Configuration.GetValue<string>("MongoDbLogging:MongoPassword");

                Console.WriteLine($"{logsDbName}:{logsDbPass}");

                logger = new LoggerConfiguration()
                  .MinimumLevel.Debug()
                  .Enrich
                    .FromLogContext()
                  .WriteTo
                    .LiterateConsole()
                  .WriteTo              
                    .MongoDBCapped($"mongodb://{logsDbName}:{logsDbPass}@{Configuration.GetValue<string>("MongoDbLogging:MongoDbAddress")}", cappedMaxSizeMb: 250)
                  .WriteTo
                    .Logger(lc =>
                        lc
                            .MinimumLevel.Warning()
                            .WriteTo
                            .Exceptionless(b => b.AddTags("TelegramWebhook")))
                  .CreateLogger();
            }

            loggerFactory.AddSerilog(logger);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
