FROM microsoft/aspnetcore:2.0.3
ENV BOT_TOKEN token
ENV NP_ADDRESS http://ukrtrackingbotapiwrappernovaposhta
ENV DEL_ADDRESS http://ukrtrackingbotapiwrapperdeliveryauto
ENV REDIS_ADDRESS redismaster:6379
ENV ASPNETCORE_ENVIRONMENT Development
ENV Exceptionless:ApiKey key
ENV MongoDbLogging:MongoDb database
ENV MongoDbLogging:MongoPassword dbpassword
ENV MongoDbLogging:MongoDbAddress dbaddress
WORKDIR .
EXPOSE 80
COPY ./bin/Docker/publish .
ENTRYPOINT ["dotnet", "UkrTrackingBot.Telegram.Web.dll"]