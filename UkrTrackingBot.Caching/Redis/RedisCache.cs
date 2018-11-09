using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UkrTrackingBot.Caching.Redis
{
    public class RedisCache : ICache<string>
    {
        private readonly StackExchange.Redis.Extensions.Core.ICacheClient cacheClient;

        public RedisCache():this("localhost:6379")
        {
        }

        public RedisCache(string connectionString)
        {
            var serializer = new StackExchange.Redis.Extensions.Newtonsoft.NewtonsoftSerializer();
            cacheClient = new StackExchange.Redis.Extensions.Core.StackExchangeRedisCacheClient(StackExchange.Redis.ConnectionMultiplexer.Connect(connectionString), serializer);
        }

        public Task Add<TValue>(string key, TValue value, TimeSpan ttl)
        {
            return cacheClient.AddAsync(key.ToString(), value, DateTimeOffset.Now.AddMilliseconds(ttl.TotalMilliseconds));
        }

        public Task<TValue> Get<TValue>(string key)
        {
            return cacheClient.GetAsync<TValue>(key.ToString());
        }
    }
}
