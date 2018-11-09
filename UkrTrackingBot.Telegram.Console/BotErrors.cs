using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UkrTrackingBot.Telegram.Console
{
    public class BotErrors
    {
        public enum ErrorType
        {
            CarrierError,
            BotInternalError
        }

        public const string ru = "ru-RU";
        public const string uk = "uk-UA";
        public const string en = "en-US";

        private readonly static BotErrorStrings botErrorStrings = new BotErrorStrings();

        public static BotErrorStrings Error { get; set; }
        public static string Get(ErrorType error, string lang) => botErrorStrings.Get(error, lang);

        public class BotErrorStrings
        {
            class ErrorString
            {
                public string lang { get; set; }
                public ErrorType id { get; set; }
                public string text { get; set; }
            }

            private readonly IEnumerable<ErrorString> errorStrings = new List<ErrorString>
            {
                new ErrorString
                {
                    lang = ru,
                    id = ErrorType.CarrierError,
                    text = "Сайт перевозчика недоступен. Пожалуйста, попробуйте позже."
                },
                new ErrorString
                {
                    lang = uk,
                    id = ErrorType.CarrierError,
                    text = "Сайт перевізника не доступний. Будь ласка, спробуйте ще раз."
                },
                new ErrorString
                {
                    lang = en,
                    id = ErrorType.CarrierError,
                    text = "Carrier tracking failed. Please try again later"
                },
                new ErrorString
                {
                    lang = ru,
                    id = ErrorType.BotInternalError,
                    text = "Произошла ошибка при обработке вашего запроса. Пожалуйста попробуйте позже."
                },
                new ErrorString
                {
                    lang = uk,
                    id = ErrorType.BotInternalError,
                    text = "Сталася помилка при обробці вашого запиту. Будь ласка, спробуйте ще раз."
                },
                new ErrorString
                {
                    lang = en,
                    id = ErrorType.BotInternalError,
                    text = "Oops! Something went wrong. Please try again later."
                }
            };

            public string DefaultLanguage { get; set; } = en;

            public string Get(ErrorType error, string lang)
            {
                return errorStrings.FirstOrDefault(item => item.id == error && item.lang == lang)?.text ??
                    errorStrings.First(item => item.id == error && item.lang == DefaultLanguage).text;
            }

            public string this[ErrorType error]
            {
                get
                {
                    return errorStrings.FirstOrDefault(item => item.id == error && item.lang == DefaultLanguage)?.text;
                }
            }
        }
    }

}
