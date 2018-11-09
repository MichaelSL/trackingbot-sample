FROM microsoft/aspnetcore:1.1.2
ENV IDENTITYSERVER_AUTHORITYURL authorityUrl
ENV ASPNETCORE_ENVIRONMENT Development
ENV Exceptionless:ApiKey key
ENV MongoDbLogging:MongoDb database
ENV MongoDbLogging:MongoPassword dbpassword
ENV MongoDbLogging:MongoDbAddress dbaddress
WORKDIR /app
EXPOSE 80
COPY ./bin/Docker/publish .
ENTRYPOINT ["dotnet", "UkrTrackingBot.Web.dll"]