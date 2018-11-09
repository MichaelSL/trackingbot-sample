using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using UkrTrackingBot.Bot.Common.Services;

namespace UkrTrackingBot.Telegram.Web.Services
{
    public class TelegramCommunicationChannel : ICommunicationChannel
    {
        private readonly string botToken;
        private readonly TelegramBotClient telegramBotClient;

        public TelegramCommunicationChannel(string token)
        {
            botToken = token;
            telegramBotClient = new TelegramBotClient(botToken);
        }

        public async Task SendTextMessageAsync(dynamic chatId, string message)
        {
            await telegramBotClient.SendTextMessageAsync(chatId, message);
        }
    }
}
