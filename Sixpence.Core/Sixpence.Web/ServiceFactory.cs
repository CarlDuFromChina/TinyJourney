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

        public static T Resolve<T>()
        {
            using (var scope = Provider.CreateScope())
                return scope.ServiceProvider.GetService<T>();
        }

        public static T Resolve<T>(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return default;
            }
            using (var scope = Provider.CreateScope())
            {
                var result = scope.ServiceProvider.GetServices<T>();
                return result.Where(x => x.GetType().Name == name).FirstOrDefault();
            }
        }

        public static T Resolve<T>(Func<T, bool> predicate)
        {
            using (var scope = Provider.CreateScope())
            {
                var services = Provider.GetServices<T>();
                return services.Where(predicate).FirstOrDefault();
            }
        }

        public static IEnumerable<T> ResolveAll<T>()
        {
            using (var scope = Provider.CreateScope())
            {
                return scope.ServiceProvider.GetServices<T>();
            }
        }

        public static IEnumerable<T> ResolveAll<T>(Func<T, bool> predicate)
        {
            using (var scope = Provider.CreateScope())
            {
                return scope.ServiceProvider.GetServices<T>().Where(predicate);
            }
        }
    }
}
