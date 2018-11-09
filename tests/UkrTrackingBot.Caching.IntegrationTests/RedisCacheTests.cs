using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using UkrTrackingBot.Caching.Redis;

namespace UkrTrackingBot.Caching.IntegrationTests
{
    [TestClass]
    public class RedisCacheTests
    {
        private RedisCache cache;

        [TestInitialize]
        public void Initialize()
        {
            cache = new RedisCache("localhost:6379");
        }

        [TestCleanup]
        public void Cleanup()
        {
            cache = null;
        }

        [TestMethod]
        [Ignore]
        public void TestInitialization()
        {
            var cache = new RedisCache("localhost:6379");
            Assert.IsNotNull(cache);
        }

        [TestMethod]
        [Ignore]
        public async Task TestCacheAddAndRetrieve()
        {
            var key = $"test_val_{System.Guid.NewGuid()}";
            var value = new Tuple<string, int>(key, 42);

            await cache.Add(key, value, TimeSpan.FromSeconds(5));
            var cachedValue = await cache.Get<Tuple<string, int>>(key);

            Assert.IsNotNull(cachedValue);
            Assert.AreEqual(value, cachedValue);
        }

        [TestMethod]
        [Ignore]
        public async Task TestCacheTimeout()
        {
            var key = $"test_val_{System.Guid.NewGuid()}";
            var value = new Tuple<string, int>(key, 42);

            await cache.Add(key, value, TimeSpan.FromMilliseconds(5));

            System.Threading.Thread.Sleep(100);

            var cachedValue = await cache.Get<Tuple<string, int>>(key);

            Assert.IsNull(cachedValue);
        }
    }
}
