using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UkrTrackingBot.Bot.Common.Services
{
    public interface ICommunicationChannel
    {
        Task SendTextMessageAsync(dynamic chatId, string message);
    }
}
