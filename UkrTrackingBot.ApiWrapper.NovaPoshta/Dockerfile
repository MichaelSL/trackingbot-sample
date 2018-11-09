FROM microsoft/aspnetcore:2.0.3
ENV API_KEY key
ENV Exceptionless:ApiKey key
ENV ASPNETCORE_ENVIRONMENT Development
ENV MongoDbLogging:MongoDb database
ENV MongoDbLogging:MongoPassword dbpassword
ENV MongoDbLogging:MongoDbAddress dbaddress
WORKDIR .
EXPOSE 80
COPY ./bin/Docker/publish .
ENTRYPOINT ["dotnet", "UkrTrackingBot.ApiWrapper.NovaPoshta.dll"]
