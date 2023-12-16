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

        public static DbContext DB = new DbContext(DBSourceConfig.Config.DriverType.GetEnum<DriverType>());

        public static ILogger<T> GetLogger<T>()
        {
            return ServiceFactory.Provider.GetService<ILoggerFactory>().CreateLogger<T>();
        }

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
                    this.DbDialect = new PostgresDialect();
                    break;
                default:
                    throw new SpException("不支持的数据库类型");
            }
        }
        public DriverType DriverType { get; set; }
        public IDbDialect DbDialect { get; set; }
    }
}
