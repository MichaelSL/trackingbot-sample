using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UkrTrackingBot.Telegram.Console
{
    internal class WebClientProxy
    {
        private readonly string NovaPoshtaApiAddress;
        private readonly string DeliveryApiAddress;
        private readonly Caching.ICache<string> cache;

        private const int CacheMinutes = 10;

        private string GetNovaPoshtaKey(string number, string phone)
        {
            return $"novaposhta_{number}_{phone}";
        }

        private string GetDeliveryKey(string number, string culture)
        {
            return $"novaposhta_{number}_{culture}";
        }

        private void LogCache(string number, bool hit)
        {
            string message = hit ? "Cache hit {number}" : "Cache miss {number}";
            Log.ForContext<WebClientProxy>().Information(message, number);
        }

        public WebClientProxy(string NovaPoshtaApiAddress, string DeliveryApiAddress, Caching.ICache<string> cache)
        {
            this.NovaPoshtaApiAddress = NovaPoshtaApiAddress;
            this.DeliveryApiAddress = DeliveryApiAddress;
            this.cache = cache;
        }

        public async Task<TrackingResponseObjectNovaPoshta> TrackNovaPoshta(string number, string phone)
        {
            string response = null;
            try
            {
                response = await cache.Get<string>(GetNovaPoshtaKey(number, phone));
            }
            catch (Exception ex)
            {
                Log.ForContext<WebClientProxy>().Warning(ex, "Can't read cache.");
            }

            if (response == null)
            {
                LogCache(number, false);
                var httpClient = new HttpClient();
                response = await httpClient.GetStringAsync($"{NovaPoshtaApiAddress}/api/Waybills/Track?number={number}" + (!String.IsNullOrEmpty(phone) ? $"&phone={phone}" : ""));
                try
                {
                    await cache.Add(GetNovaPoshtaKey(number, phone), response, TimeSpan.FromMinutes(CacheMinutes));
                }
                catch (Exception ex)
                {
                    Log.ForContext<WebClientProxy>().Warning(ex, "Can't add to cache.");
                }
            }
            else
            {
                LogCache(number, true);
            }
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TrackingResponseObjectNovaPoshta>(response);
        }

        public async Task<TrackingResponseObjectDeliveryAuto> TrackDeliveryAuto(string number, string culture)
        {
            string response = null;
            try
            {
                response = await cache.Get<string>(GetNovaPoshtaKey(number, culture));
            }
            catch (Exception ex)
            {
                Log.ForContext<WebClientProxy>().Warning(ex, "Can't read cache.");
            }

            if (response == null)
            {
                LogCache(number, false);
                var httpClient = new HttpClient();
                response = await httpClient.GetStringAsync($"{DeliveryApiAddress}/api/Waybills/Track?number={number}" + (!String.IsNullOrEmpty(culture) ? $"&culture={culture}" : ""));
                try
                {
                    await cache.Add(GetNovaPoshtaKey(number, culture), response, TimeSpan.FromMinutes(CacheMinutes));
                }
                catch (Exception ex)
                {
                    Log.ForContext<WebClientProxy>().Warning(ex, "Can't add to cache.");
                }
            }
            else
            {
                LogCache(number, true);
            }
           
            return Newtonsoft.Json.JsonConvert.DeserializeObject<TrackingResponseObjectDeliveryAuto>(response);
        }
    }
}
