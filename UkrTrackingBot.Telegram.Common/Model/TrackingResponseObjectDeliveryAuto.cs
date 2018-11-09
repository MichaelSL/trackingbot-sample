using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UkrTrackingBot.Bot.Common.Model
{
    public class TrackingResponseObjectDeliveryAuto
    {
        public bool status { get; set; }
        public string message { get; set; }
        public DeliveryResponseData data { get; set; }
    }

    public class DeliveryResponseData
    {
        public string id { get; set; }
        public string number { get; set; }
        public DateTime? SendDate { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string SenderWarehouseName { get; set; }
        public string RecepientWarehouseName { get; set; }
        public double? Discount { get; set; }
        public double? TotalCost { get; set; }
        public int? Status { get; set; }
        public double? Weight { get; set; }
        public double? Volume { get; set; }
        public string Sites { get; set; }
        public bool PaymentStatus { get; set; }
        public int? Currency { get; set; }
        public object InsuranceCost { get; set; }
        public object InsuranceCurrency { get; set; }
        public object PushStateCode { get; set; }
        public object codCost { get; set; }
        public object codCurrency { get; set; }
        public int? Type { get; set; }
        public object DateArrivalExpress { get; set; }
        public object SenderPhone { get; set; }
        public object ReceiverPhone { get; set; }
        public object CitySendName { get; set; }
        public object CityReceiveName { get; set; }
        public object DeliveryType { get; set; }

        [JsonIgnore]
        public string StatusString
        {
            get
            {
                switch (this.Status)
                {
                    case 0: return "Выдана";
                    case 1: return "Частично выдана";
                    case 2: return "Оформлена";
                    case 3: return "Утилизирована";
                    case 4: return "Продана";
                    case 5: return "Отменена";
                    case 6: return "В пути";
                    case 7: return "Оприходована";
                    case 8: return "Зарезервирована";
                }

                return "Неизвестный статус";
            }
        }
    }
}
