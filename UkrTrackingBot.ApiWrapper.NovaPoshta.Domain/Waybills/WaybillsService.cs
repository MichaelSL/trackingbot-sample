using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Models;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Waybills
{
    public class WaybillsService : IWaybillsService
    {
        private IDataProvider dataProvider;
        private Configuration.NovaPoshtaConfigurationOptions options;

        public WaybillsService(IDataProvider dataProvider, Configuration.NovaPoshtaConfigurationOptions options)
        {
            this.dataProvider = dataProvider;
            this.options = options;
        }

        public async Task<ApiResponse> TrackWaybill(string documentNumber, string phone = null)
        {
            TrackingApiRequest request = new TrackingApiRequest(documentNumber, phone);
            request.ApiKey = options.ApiKey;

            return await this.dataProvider.PostData(options.Endpoint, request);
        }
    }
}
