using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Quartz.Impl.AdoJobStore.Common;
using Sixpence.Common;
using Sixpence.ORM;
using Sixpence.ORM.Postgres;
using Sixpence.Web.Config;
using System;
using System.Collections.Generic;
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
        public static DbContext DB = new DbContext(DBSourceConfig.Config.DriverType.ToEnum<DriverType>());

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

    public class DbContext
    {
        public DbContext(DriverType driverType)
        {
            this.DriverType = driverType;
            switch (driverType)
            {
                case DriverType.Postgresql:
                    this.DbDialect = new PostgresSqlBuilder();
                    break;
                default:
                    throw new SpException("不支持的数据库类型");
            }
        }
        public DriverType DriverType { get; set; }
        public ISqlBuilder DbDialect { get; set; }
    }
}
