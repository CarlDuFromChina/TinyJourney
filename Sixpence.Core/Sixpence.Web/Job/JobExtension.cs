using Microsoft.AspNetCore.Builder;
using Quartz;
using Sixpence.Common;
using Sixpence.Common.Utils;
using System;
using System.Linq;
using Sixpence.Web.Model;
using Sixpence.Web.Config;

namespace Sixpence.Web.Job
{
    public static class JobExtension
    {
        public static SelectOption ToSelectOption(this TriggerState triggerState)
        {
            switch (triggerState)
            {
                case TriggerState.Normal:
                    return new SelectOption() { Name = "正常", Value = "0" };
                case TriggerState.Paused:
                    return new SelectOption() { Name = "暂停", Value = "1" };
                case TriggerState.Complete:
                    return new SelectOption() { Name = "完成", Value = "2" };
                case TriggerState.Error:
                    return new SelectOption() { Name = "错误", Value = "3" };
                case TriggerState.Blocked:
                    return new SelectOption() { Name = "阻塞", Value = "4" };
                case TriggerState.None:
                default:
                    return new SelectOption() { Name = "不存在", Value = "-1" };
            }
        }

        public static string GetDelegateType(DriverType dbType)
        {
            switch (dbType)
            {
                case DriverType.Postgresql:
                    return "Quartz.Impl.AdoJobStore.PostgreSQLDelegate, Quartz";
                case DriverType.Sqlite:
                    return "Quartz.Impl.AdoJobStore.SQLiteDelegate, Quartz";
                case DriverType.SqlServer:
                    return "Quartz.Impl.AdoJobStore.SqlServerDelegate, Quartz";
                case DriverType.MySql:
                    return "Quartz.Impl.AdoJobStore.MySQLDelegate, Quartz";
                case DriverType.Oracle:
                    return "Quartz.Impl.AdoJobStore.OracleDelegate, Quartz";
                default:
                    return null;
            }
        }

        public static string GetDbBDriver(DriverType dbType)
        {
            switch (dbType)
            {
                case DriverType.Postgresql:
                    return "Npgsql";
                case DriverType.Sqlite:
                    return "SQLite-Microsoft";
                case DriverType.SqlServer:
                    return "SqlServer";
                case DriverType.MySql:
                    return "MySql";
                case DriverType.Oracle:
                    return "OracleODPManaged";
                default:
                    return null;
            }
        }

        public static TriggerState ToTriggerState(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return TriggerState.None;
            }
            return (TriggerState)Convert.ToInt32(value);
        }
    }
}
