using System;
using System.Collections.Generic;
using System.Text;

namespace UkrTrackingBot.Telegram.Console.TrackingResponseExtensions
{
    public static class TrackingResponseNovaPoshtaExtensions
    {
        public static string GetResponseString(this NovaPoshtaData novaPoshtaData)
        {
            var recieveAddress = String.IsNullOrEmpty(novaPoshtaData.RecipientAddress) ? novaPoshtaData.WarehouseRecipient : novaPoshtaData.RecipientAddress;
            return $"{novaPoshtaData.Status}, Адрес: {recieveAddress}, {novaPoshtaData.Number}";
        }
    }
}
