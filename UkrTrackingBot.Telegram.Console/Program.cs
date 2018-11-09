using Exceptionless;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using UkrTrackingBot.Common;
using UkrTrackingBot.Telegram.Console.TrackingResponseExtensions;

namespace UkrTrackingBot.Telegram.Console
{
    class Program
    {
        private const int TEN_MINUTES = 10 * 60 * 1000;
        private static TelegramBotClient Bot;

        private static WebClientProxy WebClientProxy;

        private static readonly Timer tenMinutes = new Timer(new TimerCallback(RequestsCounter), requestsCount, TEN_MINUTES, TEN_MINUTES);

        private static void RequestsCounter(object param)
        {
            try
            {
                double rpm = requestsCount / 10.0;
                if (rpm > 0)
                {
                    Log.Information("Requests per minute: {requests}", rpm);
                }
            }
            catch { }
            finally
            {
                requestsCount = 0;
            }
        }

        private static bool CheckNovaPoshtaCommand(string command)
        {
            return (command.StartsWith(BotCommands.TRACK_NP_COMMAND_EN) ||
                command.StartsWith(BotCommands.TRACK_NP_COMMAND_RU) ||
                command.StartsWith(BotCommands.TRACK_NP_COMMAND_UK));            
        }

        private static bool CheckDeliveryCommand(string command)
        {
            return (command.StartsWith(BotCommands.TRACK_DEL_COMMAND_EN) ||
                command.StartsWith(BotCommands.TRACK_DEL_COMMAND_RU) ||
                command.StartsWith(BotCommands.TRACK_DEL_COMMAND_UK));
        }

        private static long requestsCount = 0;

        static void Main(string[] args)
        {
            var envVars = Environment.GetEnvironmentVariables();

            switch ((string)envVars["RUNTIME_ENVIRONMENT"])
            {
                case "Development":
                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel
                            .Debug()
                        .WriteTo
                            .LiterateConsole()
                        .CreateLogger();
                    break;
                default:
                    ExceptionlessClient.Default.Startup();
                    ExceptionlessClient.Default.Configuration.DefaultTags.Add("Telegram.Console");
                    ExceptionlessClient.Default.Configuration.ApiKey = (string)envVars["Exceptionless:ApiKey"];

                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel
                            .Information()
                        .WriteTo
                            .LiterateConsole()
                        .WriteTo
                            .MongoDBCapped($"mongodb://{(string)envVars["MongoDbLogging:MongoDb"]}:{(string)envVars["MongoDbLogging:MongoPassword"]}@{(string)envVars["MongoDbLogging:MongoDbAddress"]}", cappedMaxSizeMb: 250)
                        .MinimumLevel
                            .Warning()
                        .WriteTo
                            .Exceptionless(b => b.AddTags("Telegram.Console"))                      
                        .CreateLogger();
                    break;
            }

            Log.ForContext<Program>().Debug((string)envVars["BOT_TOKEN"]);

            Bot = new TelegramBotClient((string)envVars["BOT_TOKEN"]);
            Caching.Redis.RedisCache cache = null;
            string cacheConnectionString = (string)envVars["REDIS_ADDRESS"];
            try
            {
                cache = new Caching.Redis.RedisCache(cacheConnectionString);
            }
            catch (Exception cacheCreationException)
            {
                Log.ForContext<Program>().Warning(cacheCreationException, "Can't create cache provider: {address}", cacheConnectionString);
            }

            WebClientProxy = new WebClientProxy((string)envVars["NP_ADDRESS"], (string)envVars["DEL_ADDRESS"], cache);

            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnMessage += BotOnMessageReceived;
            Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            var me = Bot.GetMeAsync().Result;

            System.Console.Title = me.Username;

            Bot.StartReceiving();
            while (System.Console.ReadLine()?.ToLower() != "quit")
            {
                Thread.Sleep(300);
            }
            Bot.StopReceiving();
        }

        #region RESERVED_FOR_FUTURE_USE
        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Log.Debug(receiveErrorEventArgs.ApiRequestException.Message + "\n\n" + receiveErrorEventArgs.ApiRequestException.StackTrace);
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Log.Debug($"Received choosen inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        private static void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Log.Debug($"Received InlineQuery: {inlineQueryEventArgs.InlineQuery.ToString()}");
        }

        private static void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            Log.Debug($"Received {callbackQueryEventArgs.CallbackQuery.Data}");
        }
        #endregion

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            Func<object, string> toJson = (input) =>
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(input, Newtonsoft.Json.Formatting.Indented);
            };

            Func<string, IEnumerable<string>> getArgs = (input) =>
            {
                return input.Split(" ", StringSplitOptions.RemoveEmptyEntries).Skip(1);
            };

            try
            {
                var message = messageEventArgs.Message;

                if (message == null || message.Type != MessageType.TextMessage)
                {
                    Log.ForContext<Program>().Information("Message processing halted: {message}", message);
                    return;
                }

                var filteredMessageText = message.Text.Replace("\"", String.Empty).Replace("'", String.Empty);
                var messageArgs = getArgs(filteredMessageText);

                var carrier = TrackingNumberUtility.GetCarrierByTrackingNumber(message.Text);

                switch (carrier)
                {
                    case Carriers.NovaPoshta:
                        requestsCount++;
                        await TrackNovaPoshta(message, message.Text);
                        break;
                    case Carriers.Delivery:
                        requestsCount++;
                        await TrackDeliveryAuto(message, message.Text);
                        break;
                    case null:
                        if (CheckNovaPoshtaCommand(filteredMessageText))
                        {
                            requestsCount++;
                            await TrackNovaPoshta(message, messageArgs.First(), messageArgs.Skip(1).FirstOrDefault());
                        }
                        else if (CheckDeliveryCommand(filteredMessageText))
                        {
                            requestsCount++;
                            await TrackDeliveryAuto(message, messageArgs.First());
                        }
                        else if (filteredMessageText.StartsWith(BotCommands.CONTACT_COMMAND_NEUTRAL))
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, BotPrompts.Get(BotPrompts.MessageType.Contact, message.From.LanguageCode));
                        }
                        else
                        {
                            await Bot.SendTextMessageAsync(message.Chat.Id, BotPrompts.Get(BotPrompts.MessageType.Instructions, message.From.LanguageCode));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.ForContext<Program>().Error(ex, "Can't process {bot message}", messageEventArgs.Message);
                await Bot.SendTextMessageAsync(messageEventArgs.Message.Chat.Id, BotErrors.Get(BotErrors.ErrorType.BotInternalError, messageEventArgs.Message.From.LanguageCode));
            }
        }

        private static async Task TrackDeliveryAuto(global::Telegram.Bot.Types.Message message, string trackingNumber)
        {
            var response = await WebClientProxy.TrackDeliveryAuto(trackingNumber, message.From.LanguageCode);
            if (response.status)
            {
                await Bot.SendTextMessageAsync(message.Chat.Id, response.data?.GetResponseString());
            }
            else
            {
                Log.ForContext<Program>().Error(JsonConvert.SerializeObject(response.message));
                await TrackingFailedResponse(message);
            }
        }

        private static async Task TrackNovaPoshta(global::Telegram.Bot.Types.Message message, string trackingNumber, string phoneNumber = null)
        {
            var response = await WebClientProxy.TrackNovaPoshta(trackingNumber, phoneNumber);
            if (response.success)
            {
                NovaPoshtaData firstWaybillInfo = response.data.FirstOrDefault();
                await Bot.SendTextMessageAsync(message.Chat.Id, firstWaybillInfo.GetResponseString());
            }
            else
            {
                Log.ForContext<Program>().Error(JsonConvert.SerializeObject(response.errors));
                await TrackingFailedResponse(message);
            }
        }

        private static async Task TrackingFailedResponse(global::Telegram.Bot.Types.Message message)
        {
            await Bot.SendTextMessageAsync(message.Chat.Id, BotErrors.Get(BotErrors.ErrorType.CarrierError, message.From.LanguageCode));
        }
    }
}
