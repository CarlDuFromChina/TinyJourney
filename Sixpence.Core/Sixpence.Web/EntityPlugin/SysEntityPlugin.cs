using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Cache;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;
using Sixpence.Common;
using Sixpence.Common.Utils;
using Sixpence.Web.Model;
using Microsoft.Extensions.DependencyInjection;

namespace Sixpence.Web.EntityPlugin
{
    public class SysEntityPlugin : IEntityManagerPlugin
    {
        private readonly SysRolePrivilegeService _rolePrivilegeService;
        private readonly IEnumerable<IRole> _roles;
        public SysEntityPlugin(SysRolePrivilegeService rolePrivilegeService, IServiceProvider provider)
        {
            _rolePrivilegeService = rolePrivilegeService;
            _roles = provider.GetServices<IRole>();
        }

        public void Execute(EntityManagerPluginContext context)
        {
            var manager = context.EntityManager;
            switch (context.Action)
            {
                case EntityAction.PostCreate:
                    // 创建权限
                    _rolePrivilegeService.CreateRoleMissingPrivilege();
                    // 重新注册权限并清除缓存
                    UserPrivilegesCache.Clear();
                    _roles.Each(item => item.RebuildCache());
                    break;
                case EntityAction.PostDelete:
                    // 删除权限
                    var privileges = _rolePrivilegeService.GetPrivileges(context.Entity.PrimaryColumn.Value?.ToString()).ToArray();
                    manager.Delete(privileges);
                    // // 重新注册权限并清除缓存
                    UserPrivilegesCache.Clear();
                    _roles.Each(item => item.RebuildCache());
                    break;
                default:
                    break;
            }
        }
    }
}
