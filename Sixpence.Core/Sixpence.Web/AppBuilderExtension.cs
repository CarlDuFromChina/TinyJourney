using Microsoft.AspNetCore.Builder;
using Sixpence.ORM;
using Sixpence.Web.Job;
using Sixpence.Web.Service;
using Sixpence.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Common.Utils;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.Web.Module.SysMenu;

namespace Sixpence.Web
{
    public static class AppBuilderExtension
    {
        public static void UseSixpenceWeb(this IApplicationBuilder builder)
        {
            ServiceFactory.Provider = builder.ApplicationServices;

            builder.UseSorm(options =>
            {
                options.EnableLogging = true;
                options.MigrateDb = true;
            });

            #region 1. Job 启动
            JobHelpers.Start();
            #endregion

            #region 2. 权限写入缓存
            var roles = ServiceFactory.ResolveAll<IRole>();
            using var manager = new EntityManager();
            new SysRolePrivilegeService(manager).CreateRoleMissingPrivilege();

            // 权限读取到缓存
            roles.Each(item => MemoryCacheUtil.Set(item.GetRoleKey, new RolePrivilegeModel() { Role = item.GetSysRole(), Privileges = item.GetRolePrivilege() }, 3600 * 12));
            #endregion

            #region 3. 系统配置自动创建
            var settings = ServiceFactory.ResolveAll<ISysConfig>();
            new SysConfigService().CreateMissingConfig(settings);
            #endregion

            #region 4. 初始化用户
            var inits = ServiceFactory.ResolveAll<IInitDbData>();
            using (var manger = new EntityManager())
            {
                var sysUsers = new List<SysUser>();
                var sysAuthUsers = new List<SysAuthUser>();
                var sysMenus = new List<SysMenu>();
                foreach (var item in inits)
                {
                    sysUsers = sysUsers.Concat(item.GetSysUsers()).ToList();
                    sysAuthUsers = sysAuthUsers.Concat(item.GetSysAuthUsers()).ToList();
                    sysMenus = sysMenus.Concat(item.GetSysMenus()).ToList();
                }
                new SysUserService(manager).CreateMissingUser(sysUsers);
                new SysAuthUserService(manager).CreateMissingAuthUser(sysAuthUsers);
                new SysMenuService(manager).CreateMissingMenu(sysMenus);
            }
            #endregion
        }
    }
}
