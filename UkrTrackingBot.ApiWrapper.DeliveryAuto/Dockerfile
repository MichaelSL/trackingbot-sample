FROM microsoft/aspnetcore:2.0.3
ENV Exceptionless:ApiKey key
ENV ASPNETCORE_ENVIRONMENT Development
ENV MongoDbLogging:MongoDb database
ENV MongoDbLogging:MongoPassword dbpassword
ENV MongoDbLogging:MongoDbAddress dbaddress
ENV API_KEY delapikey
WORKDIR .
EXPOSE 80
COPY ./bin/Docker/publish .
ENTRYPOINT ["dotnet", "UkrTrackingBot.ApiWrapper.DeliveryAuto.dll"]
