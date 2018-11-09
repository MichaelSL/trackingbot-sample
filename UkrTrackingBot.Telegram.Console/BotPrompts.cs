using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UkrTrackingBot.Telegram.Console
{
    public class BotPrompts
    {
        public enum MessageType
        {
            Instructions,
            Contact
        }

        public const string ru = "ru-RU";
        public const string uk = "uk-UA";
        public const string en = "en-US";

        private const string SupportEmail = "email@email.com";

        private readonly static BotMessageStrings botMessageStrings = new BotMessageStrings();

        public static BotMessageStrings Error { get; set; }
        public static string Get(MessageType message, string lang) => botMessageStrings.Get(message, lang);

        public class BotMessageStrings
        {
            public BotMessageStrings()
            {
            }

            class MessageString
            {
                public string lang { get; set; }
                public MessageType id { get; set; }
                public string text { get; set; }
            }

            private readonly IEnumerable<MessageString> messageStrings = new List<MessageString>
            {
                new MessageString
                {
                    lang = ru,
                    id = MessageType.Instructions,
                    text = $"Введите номер квитанции для автоматического поиска или\n" + 
                    "/start - инструкция\n" +
                    $"\"{BotCommands.TRACK_NP_COMMAND_RU}\" \"номер квитанции\" - трекинг посылки \"Новая Почта\"\n" +
                    $"\"{BotCommands.TRACK_DEL_COMMAND_RU}\" \"номер квитанции\" - трекинг посылки \"Деливери\"\n" +
                    $"/contact - обратная связь"
                },
                new MessageString
                {
                    lang = uk,
                    id = MessageType.Instructions,
                    text = $"Введіть номер квитанції для автоматичного пошуку або\n" + 
                    "/start - інструкція\n" +
                    $"\"{BotCommands.TRACK_NP_COMMAND_RU}\" \"номер квитанції\" - трекінг відправлення \"Нова Пошта\"\n" +
                    $"\"{BotCommands.TRACK_DEL_COMMAND_RU}\" \"номер квитанції\" - трекінг відправлення \"Делівері\"\n" +
                    $"/contact - зворотній зв'язок"
                },
                new MessageString
                {
                    lang = en,
                    id = MessageType.Instructions,
                    text = $"Send us the waybill number for automatic search or\n" + 
                    "/start - this help message\n" +
                    $"\"{BotCommands.TRACK_NP_COMMAND_EN}\" \"waybill number\" - \"Nova Poshta\" tracking\n" +
                    $"\"{BotCommands.TRACK_DEL_COMMAND_EN}\" \"waybill number\" - \"Delivery auto\" tracking\n" +
                    $"/contact - contact information"
                },
                new MessageString
                {
                    lang = ru,
                    id = MessageType.Contact,
                    text = $"Для связи с разработчиком отправьте письмо на email {SupportEmail}"
                },
                new MessageString
                {
                    lang = uk,
                    id = MessageType.Contact,
                    text = $"Пошта для зв'язку з розробником: {SupportEmail}"
                },
                new MessageString
                {
                    lang = en,
                    id = MessageType.Contact,
                    text = $"Developer team email: {SupportEmail}"
                }
            };

            public string DefaultLanguage { get; set; } = en;

            public string Get(MessageType message, string lang)
            {
                return messageStrings.FirstOrDefault(item => item.id == message && item.lang == lang)?.text ?? 
                    messageStrings.First(item => item.id == message && item.lang == DefaultLanguage).text;
            }

            public string this[MessageType message]
            {
                get
                {
                    return messageStrings.FirstOrDefault(item => item.id == message && item.lang == DefaultLanguage)?.text;
                }
            }
        }
    }
}
