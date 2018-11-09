using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace UkrTrackingBot.ApiWrapper.DeliveryAuto.Domain.Models
{
    public class ApiResponse
    {
        [JsonProperty(PropertyName = "status")]
        public bool Status { get; set; }
        [JsonProperty(PropertyName = "message")]
        public string Message { get; set; }
        [JsonProperty(PropertyName = "data")]
        public virtual object Data { get; set; }
    }
}
