using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    public static class ServiceFactory
    {
        internal static IServiceProvider Provider { get; set; }

        public static T Resolve<T>(string name)
        {
            return Provider.GetServices<T>().Where(x => x.GetType().Name == name).FirstOrDefault();
        }

        public static T Resolve<T>(Func<T, bool> predicate)
        {
            var services = Provider.GetServices<T>();
            return services.Where(predicate).FirstOrDefault();
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            return Provider.GetServices<T>();
        }

        public static IEnumerable<T> ResolveAll<T>(Func<T, bool> predicate)
        {
            return Provider.GetServices<T>().Where(predicate);
        }
    }
}
