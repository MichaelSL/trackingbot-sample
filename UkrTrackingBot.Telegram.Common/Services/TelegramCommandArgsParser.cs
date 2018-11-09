using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UkrTrackingBot.Bot.Common.Services
{
    public class TelegramCommandArgsParser : ICommandArgsParser
    {
        private readonly string[] separators = new[] { " ", "_" };
        public IEnumerable<string> GetArgs(string command)
        {            
            return command.Split(separators, StringSplitOptions.RemoveEmptyEntries).Skip(1);
        }
    }
}
