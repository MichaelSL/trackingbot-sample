using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UkrTrackingBot.ApiWrapper.DeliveryAuto.Domain.Models;

namespace UkrTrackingBot.ApiWrapper.DeliveryAuto.Domain.Waybills
{
    public class WaybillsService : IWaybillsService
    {
        private readonly Configuration.DeliveryConfiguration configuration;

        public WaybillsService(Configuration.DeliveryConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<ApiResponse> TrackWaybill(TrackWaybillApiRequest request)
        {
            string url = $"{configuration.Endpoint}/GetReceiptDetails?culture={request.Culture}&number={request.Number}";

            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(url);
                var responseString = await response.Content.ReadAsStringAsync();
                ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(responseString);
                return result;
            }
        }
    }
}
