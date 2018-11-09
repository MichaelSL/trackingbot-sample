using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UkrTrackingBot.ApiWrapper.NovaPoshta.Domain.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace UkrTrackingBot.ApiWrapper.NovaPoshta.Domain
{
    public class HttpDataProvider : IDataProvider
    {
        public async Task<ApiResponse> PostData(string url, ApiRequest request)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));
                var responseString = await response.Content.ReadAsStringAsync();
                ApiResponse result = JsonConvert.DeserializeObject<ApiResponse>(responseString);
                return result;
            }
        }
    }
}
