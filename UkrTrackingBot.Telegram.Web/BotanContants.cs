using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UkrTrackingBot.Telegram.Web
{
    public class BotanEvents
    {
        public const string EMPTY_UPDATE_RECEIVED = nameof(EMPTY_UPDATE_RECEIVED);
        public const string COMMAND = nameof(COMMAND);
        public const string PHOTO_MESSAGE = nameof(PHOTO_MESSAGE);
        public const string UNSUPPORTED_MESSAGE_TYPE = nameof(UNSUPPORTED_MESSAGE_TYPE);
        public const string COMMAND_PROCESSING_ERROR = nameof(COMMAND_PROCESSING_ERROR);
    }
}
