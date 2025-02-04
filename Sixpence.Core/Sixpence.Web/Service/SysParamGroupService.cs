﻿using Sixpence.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.Web.EntityOptionProvider;
using Sixpence.EntityFramework;

namespace Sixpence.Web.Service
{
    public class SysParamGroupService : EntityService<SysParamGroup>
    {
        #region 构造函数
        public SysParamGroupService() : base() { }
        public SysParamGroupService(IEntityManager manger) : base(manger) { }
        #endregion

        public override IList<EntityView> GetViewList()
        {
            var sql = @"
SELECT
	*
FROM
	sys_param_group
";
            var customFilter = new List<string>() { "name" };
            return new List<EntityView>()
            {
                new EntityView()
                {
                    Sql = sql,
                    CustomFilter = customFilter,
                    OrderBy = "name, created_at",
                    ViewId = "457CA7F7-BE57-4934-9434-3234EAF68E14",
                    Name = "所有的选项集"
                }
            };
        }

        public IEnumerable<SelectOption> GetParams(string code)
        {
            var sql = @"
SELECT 
	sys_param.code AS Value,
	sys_param.name AS Name
FROM sys_param
INNER JOIN sys_param_group ON sys_param.sys_param_group_id = sys_param_group.id
WHERE sys_param_group.code = @code
";
            return Manager.Query<SelectOption>(sql, new Dictionary<string, object>() { { "@code", code } }).ToList();
        }

        public IEnumerable<IEnumerable<SelectOption>> GetParamsList(string[] paramsList)
        {
            var dataList = new List<List<SelectOption>>();
            return paramsList.Select(item => GetParams(item));
        }

        public IEnumerable<SelectOption> GetEntityOptions(string code)
        {
            var resolve = ServiceFactory.Resolve<IEntityOptionProvider>(name => code.Replace("_", "").ToLower() == name.GetType().Name.Replace("EntityOptionProvider", "").ToLower());
            if (resolve != null)
            {
                return resolve.GetOptions();
            }

            var entity = Manager.QueryFirst<SysEntity>(new { code });
            if (entity != null)
            {
                return Manager.Query<SelectOption>($"select id AS Value, name AS Name from {entity.Code}");
            }
            return new List<SelectOption>();
        }

        public IEnumerable<IEnumerable<SelectOption>> GetEntityOptions(string[] codeList)
        {
            var dataList = new List<List<SelectOption>>();
            return codeList.Select(item => GetEntityOptions(item));
        }

        public override void DeleteData(List<string> ids)
        {
            Manager.ExecuteTransaction(() =>
            {
                var inSqlResult = Manager.Driver.SqlBuilder.BuildInClauseSql("sys_param_group_id", 0, ids.Cast<object>().ToList());
                var sql = $@"DELETE FROM sys_param WHERE sys_param_group_id {inSqlResult.sql}";
                Manager.Execute(sql, inSqlResult.param);
                base.DeleteData(ids);
            });
        }
    }
}