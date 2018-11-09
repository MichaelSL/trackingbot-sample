using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Models
{
    public class ApiResponse
    {
        [JsonProperty(PropertyName = "success")]
        public bool Success { get; set; }
        [JsonProperty(PropertyName = "data")]
        public IEnumerable<object> Data { get; set; }
        [JsonProperty(PropertyName = "errors")]
        public IEnumerable<object> Errors { get; set; }
        [JsonProperty(PropertyName = "warnings")]
        public IEnumerable<object> Warnings { get; set; }
        [JsonProperty(PropertyName = "info")]
        public IEnumerable<object> Info { get; set; }
    }
}
