using Sixpence.Web.Module.SysAttrs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sixpence.ORM;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;

namespace Sixpence.Web.Service
{
    public class SysEntityService : EntityService<SysEntity>
    {
        #region 构造函数
        public SysEntityService() : base() { }

        public SysEntityService(IEntityManager manager) : base(manager) { }
        #endregion

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
            return Manager.Query<SysAttrs>(new { entity_id = id }).ToList();
        }

        /// <summary>
        /// 创建实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public override string CreateData(SysEntity t)
        {
            var id = "";
            Manager.ExecuteTransaction(() =>
            {
                id = base.CreateData(t);
                var sql = $"CREATE TABLE {t.Code} (id VARCHAR(100) PRIMARY KEY)";
                Manager.Execute(sql);
            });
            return id;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="ids"></param>
        public override void DeleteData(List<string> ids)
        {
            Manager.ExecuteTransaction(() =>
            {
                var dataList = Manager.Query<SysEntity>(ids).ToList();
                base.DeleteData(ids); // 删除实体
                var sql = $@"
DELETE FROM sys_attrs WHERE entity_id {Manager.Driver.SqlBuilder.BuildInClauseSql("@ids")};
";
                Manager.Execute(sql, new { ids }); // 删除级联字段
                dataList.ForEach(data =>
                {
                    Manager.Execute($"DROP TABLE {data.Code}");
                });
            });
        }
    }
}