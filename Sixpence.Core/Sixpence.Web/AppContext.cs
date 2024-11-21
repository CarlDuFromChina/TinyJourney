using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sixpence.EntityFramework;
using Sixpence.Web.Config;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    public class AppContext
    {

        /// <summary>
        /// 数据库上下文
        /// </summary>
        public static IDbDriver DbDriver = ServiceCollectionExtensions.Options.DbSetting.Driver;

        /// <summary>
        /// 文件存储服务
        /// </summary>
        public static IStorage Storage = ServiceFactory.Resolve<IStorage>(StoreConfig.Config.Type);

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static ILogger<T> GetLogger<T>()
        {
            return ServiceFactory.Provider.GetService<ILoggerFactory>().CreateLogger<T>();
        }

        /// <summary>
        /// 获取日志
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ILogger GetLogger(Type type)
        {
            return ServiceFactory.Provider.GetService<ILoggerFactory>().CreateLogger(type);
        }
    }
}
