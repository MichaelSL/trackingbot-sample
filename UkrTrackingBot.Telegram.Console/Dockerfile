FROM microsoft/dotnet:runtime
ENV BOT_TOKEN token
ENV NP_ADDRESS http://ukrtrackingbotapiwrappernovaposhta
ENV DEL_ADDRESS http://ukrtrackingbotapiwrapperdeliveryauto
ENV REDIS_ADDRESS redismaster:6379
ENV Exceptionless:ApiKey key
ENV RUNTIME_ENVIRONMENT Development
ENV MongoDbLogging:MongoDb database
ENV MongoDbLogging:MongoPassword dbpassword
WORKDIR .
COPY ./bin/Docker/publish .
ENTRYPOINT ["dotnet", "UkrTrackingBot.Telegram.Console.dll"]