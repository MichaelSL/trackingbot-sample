using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UkrTrackingBot.Bot.Common.Extensions.TrackingResponseExtensions;
using UkrTrackingBot.Bot.Common.Model;
using UkrTrackingBot.Common;

namespace UkrTrackingBot.Bot.Common.Services
{
    public class TrackingBotService : ITrackingBotService
    {
        private readonly ICommunicationChannel communicationChannel;
        private readonly WebClientProxy webClientProxy;
        private readonly ILogger _logger;
        private readonly ICommandArgsParser commandArgsParser;

        public TrackingBotService(ICommunicationChannel communicationChannel, WebClientProxy webClientProxy, ILogger<TrackingBotService> logger, ICommandArgsParser commandArgsParser)
        {
            this.communicationChannel = communicationChannel;
            this.webClientProxy = webClientProxy;
            this._logger = logger;
            this.commandArgsParser = commandArgsParser;
        }

        public async Task ProcessTextMessage(dynamic message)
        {
            Func<string, bool> CheckNovaPoshtaCommand = (string command) =>
            {
                return (command.StartsWith(BotCommands.TRACK_NP_COMMAND_EN) ||
                    command.StartsWith(BotCommands.TRACK_NP_COMMAND_RU) ||
                    command.StartsWith(BotCommands.TRACK_NP_COMMAND_UK));
            };

            Func<string, bool> CheckDeliveryCommand = (string command) =>
            {
                return (command.StartsWith(BotCommands.TRACK_DEL_COMMAND_EN) ||
                    command.StartsWith(BotCommands.TRACK_DEL_COMMAND_RU) ||
                    command.StartsWith(BotCommands.TRACK_DEL_COMMAND_UK));
            };

            var messageText = message.Text as string;
            _logger.LogTrace("Processing {message}", messageText);

            try
            {
                Debug.Assert(messageText != null, nameof(messageText) + " != null");
                var filteredMessageText = messageText.Replace("\"", String.Empty).Replace("'", String.Empty);
                var messageArgs = commandArgsParser.GetArgs(filteredMessageText).ToList();

                var carrier = TrackingNumberUtility.GetCarrierByTrackingNumber(message.Text);

                switch (carrier)
                {
                    case Carriers.NovaPoshta:
                        await TrackNovaPoshta(message, message.Text);
                        break;
                    case Carriers.Delivery:
                        await TrackDeliveryAuto(message, message.Text);
                        break;
                    case null:
                        if (CheckNovaPoshtaCommand(filteredMessageText))
                        {
                            await TrackNovaPoshta(message, messageArgs.First(), messageArgs.Skip(1).FirstOrDefault());
                        }
                        else if (CheckDeliveryCommand(filteredMessageText))
                        {
                            await TrackDeliveryAuto(message, messageArgs.First());
                        }
                        else if (filteredMessageText.StartsWith(BotCommands.CONTACT_COMMAND_NEUTRAL))
                        {
                            await communicationChannel.SendTextMessageAsync(message.Chat.Id, BotPrompts.Get(BotPrompts.MessageType.Contact, message.From.LanguageCode));
                        }
                        else
                        {
                            await communicationChannel.SendTextMessageAsync(message.Chat.Id, BotPrompts.Get(BotPrompts.MessageType.Instructions, message.From.LanguageCode));
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Can't process {bot message}", messageText);
                await communicationChannel.SendTextMessageAsync(message.Chat.Id, BotErrors.Get(BotErrors.ErrorType.BotInternalError, message.From.LanguageCode));
            }
        }

        private async Task TrackDeliveryAuto(dynamic message, string trackingNumber)
        {
            var response = await webClientProxy.TrackDeliveryAuto(trackingNumber, message.From.LanguageCode as string);
            if (response.status)
            {
                await communicationChannel.SendTextMessageAsync(message.Chat.Id, response.data?.GetResponseString());
            }
            else
            {
                _logger.LogError(JsonConvert.SerializeObject(response.message));
                await TrackingFailedResponse(message);
            }
        }

        private async Task TrackNovaPoshta(dynamic message, string trackingNumber, string phoneNumber = null)
        {
            var response = await webClientProxy.TrackNovaPoshta(trackingNumber, phoneNumber);
            if (response.success)
            {
                NovaPoshtaData firstWaybillInfo = response.data.FirstOrDefault();
                await communicationChannel.SendTextMessageAsync(message.Chat.Id, firstWaybillInfo.GetResponseString());
            }
            else
            {
                _logger.LogError(JsonConvert.SerializeObject(response.errors));
                await TrackingFailedResponse(message);
            }
        }

        private async Task TrackingFailedResponse(dynamic message)
        {
            await communicationChannel.SendTextMessageAsync(message.Chat.Id, BotErrors.Get(BotErrors.ErrorType.CarrierError, message.From.LanguageCode));
        }
    }
}
