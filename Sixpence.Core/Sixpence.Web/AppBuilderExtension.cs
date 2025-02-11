using Microsoft.AspNetCore.Builder;
using Sixpence.EntityFramework;
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
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Sixpence.Web
{
    public static class AppBuilderExtension
    {
        public static void UseSixpenceWeb(this IApplicationBuilder builder)
        {
            using (var scope = builder.ApplicationServices.CreateScope())
            {
                builder.UseEntityFramework(options =>
                {
                    options.EnableLogging = true;
                    options.MigrateDb = true;
                });

                #region 1. Job 启动
                var jobs = scope.ServiceProvider.GetServices<IJob>().ToList();
                JobHelpers.Start(jobs);
                #endregion

                #region 2. 权限写入缓存
                var roles = scope.ServiceProvider.GetServices<IRole>();
                scope.ServiceProvider.GetRequiredService<SysRolePrivilegeService>().CreateRoleMissingPrivilege();

                // 权限读取到缓存
                roles.Each(item => MemoryCacheUtil.Set(item.GetRoleKey, new RolePrivilegeModel() { Role = item.GetSysRole(), Privileges = item.GetRolePrivilege() }, 3600 * 12));
                #endregion

                #region 3. 初始化用户
                var inits = scope.ServiceProvider.GetServices<IInitDbData>();
                using (var manger = scope.ServiceProvider.GetRequiredService<IEntityManager>())
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
                    scope.ServiceProvider.GetRequiredService<SysUserService>().CreateMissingUser(sysUsers);
                    scope.ServiceProvider.GetRequiredService<SysAuthUserService>().CreateMissingAuthUser(sysAuthUsers);
                    scope.ServiceProvider.GetRequiredService<SysMenuService>().CreateMissingMenu(sysMenus);
                }
                #endregion
            }
        }
    }
}
