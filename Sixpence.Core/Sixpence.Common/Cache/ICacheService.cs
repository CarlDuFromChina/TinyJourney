using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Common.Cache
{
    /// <summary>
    /// 缓存服务接口
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// 获取或创建缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataGetter"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> dataGetter, TimeSpan? expiry = null);

        /// <summary>
        /// 获取或创建缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="dataGetter"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        T GetOrCreate<T>(string key, Func<T> dataGetter, TimeSpan? expiry = null);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task RemoveAsync(string key);

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);
    }
}
