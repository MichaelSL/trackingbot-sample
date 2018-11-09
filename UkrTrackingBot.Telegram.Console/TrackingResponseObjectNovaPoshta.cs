using System;
using System.Collections.Generic;
using System.Text;

namespace UkrTrackingBot.Telegram.Console
{
    public class NovaPoshtaData
    {
        public string Number { get; set; }
        public int? Redelivery { get; set; }
        public double? RedeliverySum { get; set; }
        public string RedeliveryNum { get; set; }
        public string RedeliveryPayer { get; set; }
        public string OwnerDocumentType { get; set; }
        public string LastCreatedOnTheBasisDocumentType { get; set; }
        public string LastCreatedOnTheBasisPayerType { get; set; }
        public string LastCreatedOnTheBasisDateTime { get; set; }
        public string LastTransactionStatusGM { get; set; }
        public string LastTransactionDateTimeGM { get; set; }
        public string DateCreated { get; set; }
        public double DocumentWeight { get; set; }
        public double? CheckWeight { get; set; }
        public double? DocumentCost { get; set; }
        public double? SumBeforeCheckWeight { get; set; }
        public string PayerType { get; set; }
        public string RecipientFullName { get; set; }
        public string RecipientDateTime { get; set; }
        public string ScheduledDeliveryDate { get; set; }
        public string PaymentMethod { get; set; }
        public string CargoDescriptionString { get; set; }
        public string CargoType { get; set; }
        public string CitySender { get; set; }
        public string CityRecipient { get; set; }
        public string WarehouseRecipient { get; set; }
        public string CounterpartyType { get; set; }
        public double? AfterpaymentOnGoodsCost { get; set; }
        public string ServiceType { get; set; }
        public string UndeliveryReasonsSubtypeDescription { get; set; }
        public int? WarehouseRecipientNumber { get; set; }
        public string LastCreatedOnTheBasisNumber { get; set; }
        public string PhoneRecipient { get; set; }
        public string RecipientFullNameEW { get; set; }
        public string WarehouseRecipientInternetAddressRef { get; set; }
        public string MarketplacePartnerToken { get; set; }
        public string ClientBarcode { get; set; }
        public string RecipientAddress { get; set; }
        public string CounterpartyRecipientDescription { get; set; }
        public string CounterpartySenderType { get; set; }
        public string DateScan { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentStatusDate { get; set; }
        public string AmountToPay { get; set; }
        public string AmountPaid { get; set; }
        public string Status { get; set; }
        public string StatusCode { get; set; }
        public string RefEW { get; set; }
        public List<object> BackwardDeliverySubTypesServices { get; set; }
        public List<object> BackwardDeliverySubTypesActions { get; set; }
        public string UndeliveryReasons { get; set; }
    }

    public class TrackingResponseObjectNovaPoshta
    {
        public bool success { get; set; }
        public List<NovaPoshtaData> data { get; set; }
        public List<object> errors { get; set; }
        public List<object> warnings { get; set; }
        public List<object> info { get; set; }
        public List<object> messageCodes { get; set; }
        public List<object> errorCodes { get; set; }
        public List<object> warningCodes { get; set; }
        public List<object> infoCodes { get; set; }
    }
}
