using Sixpence.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Module.SysMenu;
using Sixpence.EntityFramework.Repository;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework.Entity;
using Sixpence.EntityFramework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Sixpence.Web.Service
{
    public class SysRolePrivilegeService : EntityService<SysRolePrivilege>
    {
        private readonly IServiceProvider _provider;
        public SysRolePrivilegeService(IEntityManager manager, ILogger<EntityService<SysRolePrivilege>> logger, IRepository<SysRolePrivilege> repository, IServiceProvider provider) : base(manager, logger, repository)
        {
            _provider = provider;
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public IEnumerable<SysRolePrivilege> GetUserPrivileges(string roleid, RoleType roleType)
        {
            var role = _manager.QueryFirst<SysRole>(roleid);
            var privileges = new List<SysRolePrivilege>();

            if (role.IsBasic.Value)
            {
                privileges = ServiceFactory.ResolveAll<IRole>().FirstOrDefault(item => item.Role.GetDescription() == role.Name).GetRolePrivilege().ToList();
            }
            else
            {
                var sql = @"
SELECT * FROM sys_role_privilege
WHERE role_id = @id
";
                privileges = _manager.Query<SysRolePrivilege>(sql, new Dictionary<string, object>() { { "@id", roleid } }).ToList();
            }

            switch (roleType)
            {
                case RoleType.All:
                    return privileges;
                case RoleType.Entity:
                    return privileges.Where(item => string.Equals(nameof(SysEntity), EntityCommon.UnderlineToPascal(item.ObjectType)));
                case RoleType.Menu:
                    return privileges.Where(item => string.Equals(nameof(SysMenu), EntityCommon.UnderlineToPascal(item.ObjectType)));
                default:
                    return new List<SysRolePrivilege>();
            }
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="entityid"></param>
        /// <returns></returns>
        public IEnumerable<SysRolePrivilege> GetPrivileges(string entityid)
        {
            var sql = @"
SELECT * FROM sys_role_privilege
WHERE object_id = @id";
            return _manager.Query<SysRolePrivilege>(sql, new Dictionary<string, object>() { { "@id", entityid } });
        }

        /// <summary>
        /// 批量更新或创建
        /// </summary>
        /// <param name="dataList"></param>
        public void BulkSave(List<SysRolePrivilege> dataList)
        {
            _manager.BulkCreateOrUpdate(dataList);
        }

        /// <summary>
        /// 自动生成权限
        /// </summary>
        public void CreateRoleMissingPrivilege()
        {
            var roles = _provider.GetServices<IRole>();
            var privileges = new List<SysRolePrivilege>();

            roles.Each(item =>
            {
                item.GetMissingPrivilege(_manager)
                    .Each(item =>
                    {
                        if (!item.Value.IsEmpty())
                        {
                            privileges.AddRange(item.Value);
                        }
                    });
            });

            _manager.ExecuteTransaction(() => _manager.BulkCreate(privileges));
        }
    }
}
