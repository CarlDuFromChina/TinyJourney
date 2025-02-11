using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    public class DbClientOptions
    {
        public DbClientOptions()
        {
            var dbSetting = ServiceCollectionExtensions.Options?.DbSetting;
            if (dbSetting == null)
            {
                throw new ArgumentNullException("DbSetting is required");
            }

            if (string.IsNullOrEmpty(dbSetting.ConnectionString))
            {
                throw new ArgumentNullException("ConnectionString is required");
            }

            if (dbSetting.Driver == null)
            {
                throw new ArgumentNullException("Driver is required");
            }

            ConnectionString = dbSetting.ConnectionString;
            DbDriver = dbSetting.Driver;
            CommandTimeout = dbSetting.CommandTimeout;
        }

        public DbClientOptions(string connectionString, IDbDriver driver, ILoggerFactory? factory)
        {
            ConnectionString = connectionString;
            DbDriver = driver;
            CommandTimeout = 20;
            LoggerFactory = factory;
        }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 超时时间，默认20秒
        /// </summary>
        public int? CommandTimeout { get; set; }

        /// <summary>
        /// 数据库驱动
        /// </summary>
        public IDbDriver DbDriver { get; set; }

        /// <summary>
        /// 日志工厂，可空
        /// </summary>
        public ILoggerFactory? LoggerFactory { get; set; }
    }
}
