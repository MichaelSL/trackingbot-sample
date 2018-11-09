using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UkrTrackingBot.Bot.Common.Model;

namespace UkrTrackingBot.Bot.Common.Services
{
    public class WebClientProxy
    {
        private readonly string _novaPoshtaApiAddress;
        private readonly string _deliveryApiAddress;
        private readonly Caching.ICache<string> cache;
        private readonly ILogger<WebClientProxy> _logger;
        private static readonly HttpClient httpClient = new HttpClient();

        private const int CacheMinutes = 10;

        private string GetNovaPoshtaKey(string number, string phone)
        {
            return $"novaposhta_{number}_{phone}";
        }

        private string GetDeliveryKey(string number, string culture)
        {
            return $"deliveryauto_{number}_{culture}";
        }

        private void LogCache(string number, bool hit)
        {
            string message = hit ? "Cache hit {number}" : "Cache miss {number}";
            _logger.LogInformation(message, number);
        }

        public WebClientProxy(string novaPoshtaApiAddress, string deliveryApiAddress, Caching.ICache<string> cache, ILogger<WebClientProxy> logger)
        {
            this._novaPoshtaApiAddress = novaPoshtaApiAddress;
            this._deliveryApiAddress = deliveryApiAddress;
            this.cache = cache;
            _logger = logger;
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
                _logger.LogWarning(ex, "Can't read cache.");
            }

            if (response == null)
            {
                LogCache(number, false);
                response = await httpClient.GetStringAsync($"{_novaPoshtaApiAddress}/api/Waybills/Track?number={number}" + (!String.IsNullOrEmpty(phone) ? $"&phone={phone}" : ""));
                try
                {
                    await cache.Add(GetNovaPoshtaKey(number, phone), response, TimeSpan.FromMinutes(CacheMinutes));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Can't add to cache.");
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
                _logger.LogWarning(ex, "Can't read cache.");
            }

            if (response == null)
            {
                LogCache(number, false);
                response = await httpClient.GetStringAsync($"{_deliveryApiAddress}/api/Waybills/Track?number={number}" + (!String.IsNullOrEmpty(culture) ? $"&culture={culture}" : ""));
                try
                {
                    await cache.Add(GetNovaPoshtaKey(number, culture), response, TimeSpan.FromMinutes(CacheMinutes));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Can't add to cache.");
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
