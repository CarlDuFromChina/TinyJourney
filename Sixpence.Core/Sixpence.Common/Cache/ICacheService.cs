using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Common.Cache
{
    public interface ICacheService
    {
        Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> dataGetter, TimeSpan? expiry = null);

        T GetOrCreate<T>(string key, Func<T> dataGetter, TimeSpan? expiry = null);

        Task RemoveAsync(string key);

        void Remove(string key);
    }
}
