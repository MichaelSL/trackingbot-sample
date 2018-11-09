using System;
using System.Collections.Generic;
using System.Text;
using UkrTrackingBot.Bot.Common.Model;

namespace UkrTrackingBot.Bot.Common.Extensions.TrackingResponseExtensions
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
