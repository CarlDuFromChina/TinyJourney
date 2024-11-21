using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sixpence.Web.Utils;
using System.Data;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;

namespace Sixpence.Web.Service
{
    public class VersionScriptExecutionLogService : EntityService<VersionScriptExecutionLog>
    {
        #region 构造函数
        public VersionScriptExecutionLogService() : base() { }

        public VersionScriptExecutionLogService(IEntityManager manager) : base(manager) { }
        #endregion

        /// <summary>
        /// 执行SQL脚本并记录（已成功执行过的则跳过）
        /// </summary>
        /// <param name="filePath"></param>
        public int ExecuteScript(string filePath)
        {
            var fileName = Path.GetFileName(filePath);
            var sql = @"
select * from version_script_execution_log
where name = @name and is_success is true
";
            var data = Manager.QueryFirst<VersionScriptExecutionLog>(sql, new Dictionary<string, object>() { { "@name", fileName } });
            if (data == null)
            {
                data = new VersionScriptExecutionLog() { Id = Guid.NewGuid().ToString(), Name = fileName };
                try
                {
                    if (filePath.EndsWith(".sql"))
                    {
                        Manager.ExecuteSqlScript(filePath);
                    }
                    if (filePath.EndsWith(".csv"))
                    {
                        var startIndex = fileName.IndexOf("-");
                        var endIndex = fileName.IndexOf(".");
                        var typeName = fileName.Remove(endIndex, fileName.Length - endIndex).Remove(0, startIndex + 1);
                        var columns = Manager.Query($"select * from {typeName} where 1 <> 1").Columns;
                        var dt = CsvUtil.Read(filePath, columns);
                        Manager.ExecuteTransaction(() =>
                        {
                            Manager.BulkCreateOrUpdate(typeName, "id", dt, null);
                        });
                    }
                    data.IsSuccess = true;
                    Manager.Create(data);
                    return 1;
                }
                catch (Exception ex)
                {
                    data.IsSuccess = false;
                    Manager.Create(data);
                    throw ex;
                }
            }
            return 0;
        }
    }
}
