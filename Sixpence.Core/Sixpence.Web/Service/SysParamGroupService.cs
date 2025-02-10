using Sixpence.Common;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.Web.EntityOptionProvider;
using Sixpence.EntityFramework;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Sixpence.Web.Service
{
    public class SysParamGroupService : EntityService<SysParamGroup>
    {
        private Lazy<IEnumerable<IEntityOptionProvider>> _entityOptionProviders;
        public SysParamGroupService(IEntityManager manager, ILogger<EntityService<SysParamGroup>> logger, IRepository<SysParamGroup> repository, IServiceProvider provider) : base(manager, logger, repository)
        {
            _entityOptionProviders = new Lazy<IEnumerable<IEntityOptionProvider>>(() => provider.GetServices<IEntityOptionProvider>());
        }

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
            return _manager.Query<SelectOption>(sql, new Dictionary<string, object>() { { "@code", code } }).ToList();
        }

        public IEnumerable<IEnumerable<SelectOption>> GetParamsList(string[] paramsList)
        {
            var dataList = new List<List<SelectOption>>();
            return paramsList.Select(item => GetParams(item));
        }

        public IEnumerable<SelectOption> GetEntityOptions(string code)
        {
            var resolve = _entityOptionProviders?.Value?.ToList()?.FirstOrDefault(name => code.Replace("_", "").ToLower() == name.GetType().Name.Replace("EntityOptionProvider", "").ToLower());
            if (resolve != null)
            {
                return resolve.GetOptions();
            }

            var entity = _manager.QueryFirst<SysEntity>(new { code });
            if (entity != null)
            {
                return _manager.Query<SelectOption>($"select id AS Value, name AS Name from {entity.Code}");
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
            _manager.ExecuteTransaction(() =>
            {
                var inSqlResult = _manager.Driver.SqlBuilder.BuildInClauseSql("sys_param_group_id", 0, ids.Cast<object>().ToList());
                var sql = $@"DELETE FROM sys_param WHERE sys_param_group_id {inSqlResult.sql}";
                _manager.Execute(sql, inSqlResult.param);
                base.DeleteData(ids);
            });
        }
    }
}