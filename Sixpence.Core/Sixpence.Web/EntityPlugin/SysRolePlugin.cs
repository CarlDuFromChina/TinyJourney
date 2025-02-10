using Sixpence.Common;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Entity;
using Sixpence.Web.Cache;
using Sixpence.Web.Service;
using Sixpence.EntityFramework;
using Microsoft.Extensions.DependencyInjection;

namespace Sixpence.Web.EntityPlugin
{
    public class SysRolePlugin : IEntityManagerPlugin
    {
        private readonly SysRolePrivilegeService _rolePrivilegeService;
        private readonly IEnumerable<IRole> _roles;
        public SysRolePlugin(SysRolePrivilegeService rolePrivilegeService, IServiceProvider provider)
        {
            _rolePrivilegeService = rolePrivilegeService;
            _roles = provider.GetServices<IRole>();
        }

        public void Execute(EntityManagerPluginContext context)
        {
            var obj = context.Entity as SysRole;

            switch (context.Action)
            {
                case EntityAction.PostDelete:
                    {
                        AssertUtil.IsTrue(obj.IsBasic is true, "禁止删除基础角色");
                    }
                    break;
                case EntityAction.PostCreate:
                    {
                        // 重新创建权限
                        var rootRoleId = string.IsNullOrEmpty(obj.InheritedRoleId) ? obj.Id : obj.InheritedRoleId;
                        var privileges = _rolePrivilegeService.GetUserPrivileges(rootRoleId, RoleType.All).ToList();
                        privileges.Each(item =>
                        {
                            item.Id = Guid.NewGuid().ToString();
                            item.RoleId = obj.Id;
                            item.RoleName = obj.Name;
                            item.CreatedAt = new DateTime();
                            item.UpdatedAt = new DateTime();
                        });
                        context.EntityManager.BulkCreate(privileges);
                        // 权限缓存清空
                        UserPrivilegesCache.Clear();
                        _roles.Each(e => e.RebuildCache());
                    }
                    break;
                case EntityAction.PostUpdate:
                    {
                        // 删除所有权限
                        var rootRoleId = string.IsNullOrEmpty(obj.InheritedRoleId) ? obj.Id : obj.InheritedRoleId;
                        var privileges = _rolePrivilegeService.GetUserPrivileges(obj.Id, RoleType.All).ToList();
                        privileges.Each(item => context.EntityManager.Delete(item));
                        // 重新创建权限
                        privileges = _rolePrivilegeService.GetUserPrivileges(rootRoleId, RoleType.All).ToList();
                        privileges.Each(item =>
                        {
                            item.Id = Guid.NewGuid().ToString();
                            item.RoleId = obj.Id;
                            item.RoleName = obj.Name;
                            item.CreatedAt = new DateTime();
                            item.UpdatedAt = new DateTime();
                        });
                        context.EntityManager.BulkCreate(privileges);
                        // 权限缓存清空
                        UserPrivilegesCache.Clear();
                        _roles.Each(e => e.RebuildCache());
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
