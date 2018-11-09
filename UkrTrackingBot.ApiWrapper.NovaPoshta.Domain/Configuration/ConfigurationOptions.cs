using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Configuration
{
    public class NovaPoshtaConfigurationOptions
    {
        public NovaPoshtaConfigurationOptions()
        {

        }

        public string Endpoint { get; set; }
        public string ApiKey { get; set; }
    }
}
