FROM microsoft/aspnetcore:1.1.2
ENV MVC_REDIRECT_URI redirectUri
ENV MVC_POST_LOGOUT_REDIRECT_URI logoutRedirectUri
ENV SIGN_CERT_PASSWORD pwd
ENV Exceptionless:ApiKey key
ENV MongoDbLogging:MongoDb database
ENV MongoDbLogging:MongoPassword dbpassword
ENV MongoDbLogging:MongoDbAddress dbaddress
ENV ASPNETCORE_ENVIRONMENT Development
WORKDIR /app
EXPOSE 80
COPY ./bin/Docker/publish .
COPY ./.sign-cert/uni-tracking.pfx ./.sign-cert/uni-tracking.pfx
ENTRYPOINT ["dotnet", "UkrTrackingBot.IdentityServer.dll"]