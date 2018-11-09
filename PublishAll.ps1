cd .\UkrTrackingBot.Telegram.Console
dotnet publish -c Release -o .\bin\Docker\publish\

cd ..\UkrTrackingBot.ApiWrapper.DeliveryAuto
dotnet publish -c Release -o .\bin\Docker\publish\

cd ..\UkrTrackingBot.ApiWrapper.NovaPoshta
dotnet publish -c Release -o .\bin\Docker\publish\

cd ..\UkrTrackingBot.IdentityServer
dotnet publish -c Release -o .\bin\Docker\publish\

cd ..\UkrTrackingBot.Web
dotnet publish -c Release -o .\bin\Docker\publish\

cd ..\UkrTrackingBot.Telegram.Web 
dotnet publish -c Release -o .\bin\Docker\publish\

cd ..