using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Sixpence.Web.Utils;
using System.Data;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;
using Microsoft.Extensions.Logging;

namespace Sixpence.Web.Service
{
    public class VersionScriptExecutionLogService : EntityService<VersionScriptExecutionLog>
    {
        public VersionScriptExecutionLogService(IEntityManager manager, ILogger<EntityService<VersionScriptExecutionLog>> logger, IRepository<VersionScriptExecutionLog> repository) : base(manager, logger, repository)
        {
        }

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
            var data = _manager.QueryFirst<VersionScriptExecutionLog>(sql, new Dictionary<string, object>() { { "@name", fileName } });
            if (data == null)
            {
                data = new VersionScriptExecutionLog() { Id = Guid.NewGuid().ToString(), Name = fileName };
                try
                {
                    if (filePath.EndsWith(".sql"))
                    {
                        _manager.ExecuteSqlScript(filePath);
                    }
                    if (filePath.EndsWith(".csv"))
                    {
                        var startIndex = fileName.IndexOf("-");
                        var endIndex = fileName.IndexOf(".");
                        var typeName = fileName.Remove(endIndex, fileName.Length - endIndex).Remove(0, startIndex + 1);
                        var columns = _manager.Query($"select * from {typeName} where 1 <> 1").Columns;
                        var dt = CsvUtil.Read(filePath, columns);
                        _manager.ExecuteTransaction(() =>
                        {
                            _manager.BulkCreateOrUpdate(typeName, "id", dt, null);
                        });
                    }
                    data.IsSuccess = true;
                    _manager.Create(data);
                    return 1;
                }
                catch (Exception ex)
                {
                    data.IsSuccess = false;
                    _manager.Create(data);
                    throw ex;
                }
            }
            return 0;
        }
    }
}
