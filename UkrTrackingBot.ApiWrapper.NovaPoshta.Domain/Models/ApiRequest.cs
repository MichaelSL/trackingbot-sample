using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Models
{
    public abstract class ApiRequest
    {
        [JsonProperty(PropertyName = "modelName")]
        public string ModelName { get; set; }
        [JsonProperty(PropertyName = "calledMethod")]
        public string MethodName { get; set; }
        [JsonProperty(PropertyName = "methodProperties")]
        public virtual MethodPropertiesBase MethodProperties { get; set; }

        public class MethodPropertiesBase
        {

        }
    }

    public abstract class ApiKeyRequest: ApiRequest
    {
        [JsonProperty(PropertyName = "apiKey")]
        public string ApiKey { get; set; }
    }
}
