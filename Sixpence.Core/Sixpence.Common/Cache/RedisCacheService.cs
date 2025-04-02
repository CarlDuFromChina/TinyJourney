using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sixpence.Common.Cache
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redisDb;

        public RedisCacheService(IConnectionMultiplexer redis)
            => _redisDb = redis.GetDatabase();

        public T GetOrCreate<T>(string key, Func<T> dataGetter, TimeSpan? expiry = null)
        {
            var cachedValue = _redisDb.StringGet(key);
            if (!cachedValue.IsNull)
            {
                return JsonSerializer.Deserialize<T>(cachedValue);
            }

            var data = dataGetter();

            if (data != null)
            {
                _redisDb.StringSet(key, JsonSerializer.Serialize(data), expiry);
            }

            return data;
        }

        public async Task<T> GetOrCreateAsync<T>(
            string key,
            Func<Task<T>> dataGetter,
            TimeSpan? expiry = null)
        {
            // 1. 尝试从缓存获取
            var cachedValue = await _redisDb.StringGetAsync(key);
            if (!cachedValue.IsNull)
            {
                return JsonSerializer.Deserialize<T>(cachedValue);
            }

            // 2. 执行数据获取委托
            var data = await dataGetter();

            if (data != null)
            {
                // 3. 回填缓存
                await _redisDb.StringSetAsync(
                    key,
                    JsonSerializer.Serialize(data),
                    expiry);
            }

            return data;
        }

        public void Remove(string key)
        {
            _redisDb.KeyDelete(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _redisDb.KeyDeleteAsync(key);
        }
    }
}
