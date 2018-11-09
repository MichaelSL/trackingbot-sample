using System;
using System.Collections.Generic;
using System.Text;
using UkrTrackingBot.Bot.Common.Model;

namespace UkrTrackingBot.Bot.Common.Extensions.TrackingResponseExtensions
{
    public static class TrackingResponseDeliveryAutoExtensions
    {
        public static string GetResponseString(this DeliveryResponseData messageData)
        {
            var receiveDate = messageData.ReceiveDate.HasValue ? $", Дата получения: {messageData.ReceiveDate.Value.ToShortDateString()}" : String.Empty;
            return $"{messageData.StatusString}, Склад: {messageData.RecepientWarehouseName}{receiveDate}";
        }
    }
}
