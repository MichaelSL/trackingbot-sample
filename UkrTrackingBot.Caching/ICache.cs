using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UkrTrackingBot.Caching
{
    public interface ICache<TKey>
    {
        Task Add<TValue>(TKey key, TValue value, TimeSpan ttl);
        Task<TValue> Get<TValue>(TKey key);
    }
}
