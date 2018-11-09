using System;
using System.Collections.Generic;
using System.Text;

namespace UkrTrackingBot.Telegram.Console.TrackingResponseExtensions
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
