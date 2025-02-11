using Sixpence.Web.Module.SysAttrs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sixpence.EntityFramework;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Microsoft.Extensions.Logging;
using Sixpence.EntityFramework.Entity;

namespace Sixpence.Web.Service
{
    public class SysEntityService : EntityService<SysEntity>
    {
        public SysEntityService(IEntityManager manager, ILogger<EntityService<SysEntity>> logger, IRepository<SysEntity> repository) : base(manager, logger, repository)
        {
        }

        public override IList<EntityView> GetViewList()
        {
            var sql = @"
SELECT
	*
FROM
	sys_entity
";
            var customFilter = new List<string>() { "name" };
            return new List<EntityView>()
            {
                new EntityView()
                {
                    Sql = sql,
                    CustomFilter = customFilter,
                    OrderBy = "name, created_at",
                    ViewId = "FBEC5163-587B-437E-995F-1DC97229C906",
                    Name = "所有的实体"
                }
            };
        }

        /// <summary>
        /// 根据实体 id 查询字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IList<SysAttrs> GetEntityAttrs(string id)
        {
            return _manager.Query<SysAttrs>(new { entity_id = id }).ToList();
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override string CreateData(SysEntity t)
        {
            var id = "";
            _manager.ExecuteTransaction(() =>
            {
                id = base.CreateData(t);
                var sql = $"CREATE TABLE {t.Code} (id VARCHAR(100) PRIMARY KEY)";
                _manager.Execute(sql);
            });
            return id;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids"></param>
        public override void DeleteData(List<string> ids)
        {
            _manager.ExecuteTransaction(() =>
            {
                var dataList = _manager.Query<SysEntity>(ids).ToList();
                base.DeleteData(ids); // 删除实体
                var inSqlResult = _manager.Driver.SqlBuilder.BuildInClauseSql("entity_id", 0, ids.Cast<object>().ToList());
                var sql = $@"
DELETE FROM sys_attrs WHERE entity_id {inSqlResult.sql};
";
                _manager.Execute(sql, inSqlResult.param); // 删除级联字段
                dataList.ForEach(data =>
                {
                    _manager.Execute($"DROP TABLE {data.Code}");
                });
            });
        }

        public override IEnumerable<SelectOption> GetOptions()
        {
            var sql = $@"SELECT code AS Value, name AS Name FROM {new SysEntity().EntityMap.FullQualifiedName}";
            return _manager.Query<SelectOption>(sql);
        }
    }
}