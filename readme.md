# TrackingBot

This repository is an example of microservices and chat bots usage.

Application allows to track parcels of two major transporatation companies in Ukraine.

## Quick start

### How to build the project

I use [Cake](https://cakebuild.net/) to build the project.

Run `build.ps1` on Windows or `build.sh` on Linux to build the projects and create Docker containers.

If you plan to push your containers to a repository use `DockerPush` task.

Cake process `docker-compose.template.yml` file and creates `docker-compose.yml` which you can use to rune the project.

### How to run the project

Define all necessary environment variables in you `docker-compose.yml` file and use `docker-compose up` or `docker stack deploy` commands: [docker-compose up](https://docs.docker.com/compose/reference/up/), [docker stack deploy](https://docs.docker.com/engine/reference/commandline/stack_deploy/)

## Application description

### Logging

Application logs are sent to [Exceptionless](https://exceptionless.com/) service and any [MongoDb](https://www.mongodb.com/) database.

You will need to provide Exceptionless API key and MongoDb connection string to enable logging.

### Transportation API access

Transporation companies API access implemented with `UkrTrackingBot.ApiWrapper.DeliveryAuto` and `UkrTrackingBot.ApiWrapper.NovaPoshta` projects.

You will need to obtain API keys for using tracking API.

You can do this here:

- [Delivery-Auto](https://www.delivery-auto.com.ua/en-us/Account/RegisterPageDelGroup?return_url=https%3A%2F%2Fwww.delivery-auto.com.ua%2Fen-US%2FAccount%2FApiKey&site_url=https%3A%2F%2Fwww.delivery-auto.com.ua)
- [Nova-Poshta](https://my.novaposhta.ua/settings/index#apikeys)

### Telegram bot

Telegram chat bots API support 2 ways of getting updates:

- [long polling](https://core.telegram.org/bots/api#getupdates): `UkrTrackingBot.Telegram.Console` project
- [webhooks](https://core.telegram.org/bots/api#setwebhook): `UkrTrackingBot.Telegram.Web` project

You will need to [create your bot](https://core.telegram.org/bots#3-how-do-i-create-a-bot) and supply bot token into application.

### Authentication and web fronend

Solution contains web frontend project `UkrTrackingBot.Web`.

`UkrTrackingBot.IdentityServer` project is an [IdentityServer4](https://github.com/IdentityServer/IdentityServer4) host. You will need to generate a ```pfx``` sertificate to use it.

### Utilities

[Nginx](https://www.nginx.com/) is used as a reverse proxy. [Letsencrypt](https://letsencrypt.org/) certificates generated on server startup.

[Redis](https://redis.io/) is used to cache tracking API results.

## Misc

- `CleanDocker.ps1` script removes all containers and images from your local Docker.
- `PublishAll.ps1` script publishes all projects to local directories.