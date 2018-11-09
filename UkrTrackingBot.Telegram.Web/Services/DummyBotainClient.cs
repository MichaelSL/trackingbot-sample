using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotanClient;
using BotanClient.Models;

namespace UkrTrackingBot.Telegram.Web.Services
{
    public class DummyBotainClient: IBotanClient
    {
        public Task<BotanResponse> SendEvent(BotanMessage botanMessage) => Task.FromResult(new BotanResponse());

        public Task<string> ShortenUrl(string url, IEnumerable<string> userIds) => Task.FromResult(url);
    }
}
