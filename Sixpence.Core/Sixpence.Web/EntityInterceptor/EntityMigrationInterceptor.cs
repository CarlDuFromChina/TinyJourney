using Microsoft.Extensions.Logging;
using Sixpence.Common;
using Sixpence.Common.Utils;
using Sixpence.ORM;
using Sixpence.Web.Auth;
using Sixpence.Web.Entity;
using Sixpence.Web.Module.SysAttrs;
using Sixpence.Web.Service;
using Sixpence.Web.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Sixpence.Web.EntityInterceptor
{
    /// <summary>
    /// 自动创建实体插件，记录实体
    /// </summary>
    public class EntityMigrationInterceptor : IEntityMigrationInterceptor
    {
        public void Execute(EntityMigrationInterceptorContext context)
        {
            // 设置当前线程为系统用户
            UserIdentityUtil.SetCurrentUser(UserIdentityUtil.GetSystem());

            switch (context.Action)
            {
                case EntityMigrationAction.PreUpdateEntities: // 所有实体变更前执行
                    break;
                case EntityMigrationAction.PostUpdateEntities: // 所有实体变更后执行
                    MigrateScripts(context);
                    LogEntityMigration(context);
                    break;
                case EntityMigrationAction.PreUpdateEntity: // 每个实体变更前都会执行
                    break;
                case EntityMigrationAction.PostUpdateEntity: // 每个实体变更后都会执行
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 记录实体变更
        /// </summary>
        /// <param name="context"></param>
        private void LogEntityMigration(EntityMigrationInterceptorContext context)
        {
            var manager = context.Manager;
            var logger = context.Logger;

            context.EntityList.Each(item =>
            {
                #region 实体添加自动写入记录
                var entity = manager.QueryFirst<SysEntity>(new { code = item.EntityMap.Table });
                if (entity == null)
                {
                    entity = new SysEntity()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = item.EntityMap.Description,
                        Schema = item.EntityMap.Schema,
                        Code = item.EntityMap.Table,
                    };
                    manager.Create(entity, false);
                }
                #endregion

                var attrs = entity.EntityMap.Properties; // 实体类字段
                var attrsList = new SysEntityService(manager).GetEntityAttrs(entity.Id).Select(e => e.Code); // 现有字段

                #region 实体字段变更（删除字段）
                attrsList.Each(attr =>
                {
                    if (!attrs.Any(item => item.Name.ToLower() == attr.ToLower()))
                    {
                        var sql = @"DELETE FROM sys_attrs WHERE lower(code) = @code AND entityid = @entityid";
                        manager.Execute(sql, new Dictionary<string, object>() { { "@code", attr.ToLower() }, { "@entityid", entity.Id } });
                        sql = manager.Driver.SqlBuilder.BuildDropColumnSql(item.EntityMap.Table, new List<string>() { attr });
                        manager.Execute(sql);
                        logger.LogDebug($"实体{entity.Name} （{entity.Code}）删除字段：{attr}");
                    }
                });
                #endregion

                #region 实体字段变更（新增字段）
                attrs.Each(attr =>
                {
                    if (!attrsList.Contains(attr.Name))
                    {
                        var _attr = new SysAttrs()
                        {
                            Id = Guid.NewGuid().ToString(),
                            Name = string.IsNullOrEmpty(attr.Remark) ? attr.Name : attr.Remark,
                            Code = attr.Name,
                            EntityId = entity.Id,
                            EntityName = entity.Name,
                            EntityCode = entity.Code,
                            AttrType = attr.DbType.ToString().ToLower(),
                            AttrLength = attr.Length ?? 0,
                            IsRequire = attr.CanBeNull.HasValue && !attr.CanBeNull.Value,
                            DefaultValue = ConvertUtil.ConToString(attr.DefaultValue)
                        };
                        manager.Create(_attr);
                        logger.LogDebug($"实体{item.EntityMap.Description}（{item.EntityMap.Table}）创建字段：{attr.Remark}（{attr.Name}）成功");
                    }
                });
                #endregion
            });
        }

        /// <summary>
        /// 迁移版本更新脚本
        /// </summary>
        /// <param name="context"></param>
        private void MigrateScripts(EntityMigrationInterceptorContext context)
        {
            var manager = context.Manager;
            var logger = context.Logger;
            var fileList = FileHelper.GetFileList("*.*", FolderType.Version).OrderBy(item => Path.GetFileName(item)).ToList();
            fileList.Each(filePath =>
            {
                try
                {
                    if (filePath.EndsWith(".sql") || filePath.EndsWith(".csv"))
                    {
                        var count = new VersionScriptExecutionLogService(manager).ExecuteScript(filePath);
                        if (count == 1)
                        {
                            logger.LogInformation($"脚本：{Path.GetFileName(filePath)}执行成功");
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError($"脚本：{Path.GetFileName(filePath)}执行失败", ex);
                }
            });
        }
    }
}
