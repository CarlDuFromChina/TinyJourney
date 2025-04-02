using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sixpence.Common.Cache
{
    /// <summary>
    /// 本地缓存
    /// </summary>
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly SemaphoreSlim _asyncLock = new SemaphoreSlim(1, 1); // 异步锁防止缓存击穿

        public MemoryCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public T GetOrCreate<T>(string key, Func<T> dataGetter, TimeSpan? expiry = null)
        {
            // 尝试从缓存获取
            if (_memoryCache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }

            // 缓存未命中，执行数据获取逻辑
            var value = dataGetter();
            if (value != null)
            {
                var cacheEntryOptions = new MemoryCacheEntryOptions();
                if (expiry.HasValue)
                {
                    cacheEntryOptions.SetAbsoluteExpiration(expiry.Value);
                }
                _memoryCache.Set(key, value, cacheEntryOptions);
            }

            return value;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> dataGetter, TimeSpan? expiry = null)
        {
            // 尝试从缓存获取
            if (_memoryCache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }

            // 加锁防止并发重复加载
            await _asyncLock.WaitAsync();
            try
            {
                // 双重检查（Double-Check）
                if (_memoryCache.TryGetValue(key, out cachedValue))
                {
                    return cachedValue;
                }

                // 执行异步数据获取
                var value = await dataGetter();
                if (value != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions();
                    if (expiry.HasValue)
                    {
                        cacheEntryOptions.SetAbsoluteExpiration(expiry.Value);
                    }
                    _memoryCache.Set(key, value, cacheEntryOptions);
                }

                return value;
            }
            finally
            {
                _asyncLock.Release();
            }
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            return Task.CompletedTask; // MemoryCache 的 Remove 是同步操作
        }
    }
}
