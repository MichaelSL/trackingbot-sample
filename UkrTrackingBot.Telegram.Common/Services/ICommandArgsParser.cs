using System;
using System.Collections.Generic;
using System.Text;

namespace UkrTrackingBot.Bot.Common.Services
{
    public interface ICommandArgsParser
    {
        IEnumerable<string> GetArgs(string command);
    }
}
