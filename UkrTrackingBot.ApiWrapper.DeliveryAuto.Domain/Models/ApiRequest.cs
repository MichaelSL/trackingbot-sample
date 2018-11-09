using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UkrTrackingBot.ApiWrapper.DeliveryAuto.Domain.Models
{
    public abstract class ApiRequest
    {

    }

    public abstract class LocalizableApiRequest : ApiRequest
    {
        [JsonProperty(PropertyName = "culture")]
        public string Culture { get; set; }
    }
}
