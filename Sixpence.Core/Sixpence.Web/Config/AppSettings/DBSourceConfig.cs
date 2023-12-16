using Sixpence.Common.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Config
{
    public class DBSourceConfig : BaseAppConfig<DBSourceConfig>
    {
        /// <summary>
        /// 驱动：Postgresql、MySql
        /// </summary>
        public string DriverType { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// 超时时间
        /// </summary>
        public int CommandTimeOut { get; set; }
    }

    public enum DriverType
    {
        [Description("Postgres")]
        Postgresql,
        [Description("MySql")]
        MySql,
        [Description("SqlServer")]
        SqlServer,
        [Description("Oracle")]
        Oracle,
        [Description("Sqlite")]
        Sqlite,
    }
}
