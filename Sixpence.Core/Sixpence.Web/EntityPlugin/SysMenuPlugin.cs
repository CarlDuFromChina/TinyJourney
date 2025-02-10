using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sixpence.Web.Module.SysMenu;
using Sixpence.Web.Cache;
using Sixpence.Web.Service;
using Sixpence.EntityFramework;
using Sixpence.Web.Auth.Role;
using Microsoft.Extensions.DependencyInjection;
using Sixpence.Common;

namespace Sixpence.Web.EntityPlugin
{
    public class SysMenuPlugin : IEntityManagerPlugin
    {
        private readonly SysRolePrivilegeService _rolePrivilegeService;
        private readonly IEnumerable<IRole> _roles;
        public SysMenuPlugin(SysRolePrivilegeService rolePrivilegeService, IServiceProvider provider)
        {
            _rolePrivilegeService = rolePrivilegeService;
            _roles = provider.GetServices<IRole>();
        }

        public void Execute(EntityManagerPluginContext context)
        {
            var manager = context.EntityManager;
            var entity = context.Entity as SysMenu;

            switch (context.Action)
            {
                case EntityAction.PreCreate:
                case EntityAction.PreUpdate:
                    WriteStateCodeName(entity);
                    break;
                case EntityAction.PostCreate:
                    // 创建权限
                    _rolePrivilegeService.CreateRoleMissingPrivilege();
                    // 重新注册权限并清除缓存
                    UserPrivilegesCache.Clear();
                    _roles.Each(item => item.RebuildCache());
                    break;
                case EntityAction.PostDelete:
                    var privileges = _rolePrivilegeService.GetPrivileges(context.Entity.PrimaryColumn.Value?.ToString())?.ToArray();
                    manager.Delete(privileges);
                    // 重新注册权限并清除缓存
                    UserPrivilegesCache.Clear();
                    _roles.Each(item => item.RebuildCache());
                    break;
                default:
                    break;
            }
        }

        private void WriteStateCodeName(SysMenu menu)
        {
            if (menu.IsEnable == true)
                menu.IsEnableName = "启用";
            else
                menu.IsEnableName = "停用";
        }
    }
}
