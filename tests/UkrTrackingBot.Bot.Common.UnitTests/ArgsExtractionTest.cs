using System;
using System.Collections.Generic;
using System.Linq;
using UkrTrackingBot.Bot.Common.Services;
using Xunit;

namespace UkrTrackingBot.Bot.Common.UnitTests
{
    public class ArgsExtractionTest
    {
        [Theory]
        [InlineData("tracknp 1234567890", "1234567890")]
        [InlineData("tracknp_1234567890", "1234567890")]
        [InlineData("/tracknp_1234567890", "1234567890")]
        public void WhiteSpaceAndUnderscoreInput_Parsed(string input, string expectedArg)
        {
            ICommandArgsParser commandArgsParser = new TelegramCommandArgsParser();
            IEnumerable<string> parsedArg = commandArgsParser.GetArgs(input);
            Assert.Equal(expectedArg, parsedArg.FirstOrDefault());
        }
    }
}
