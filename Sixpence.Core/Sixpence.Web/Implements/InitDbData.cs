using Sixpence.Common;
using Sixpence.Web.Auth.Role;
using Sixpence.Web.Config;
using Sixpence.Web.Entity;
using Sixpence.Web.Extensions;
using Sixpence.Web.Module.SysMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    internal class InitDbData : IInitDbData
    {
        public List<SysAuthUser> GetSysAuthUsers()
        {
            return new List<SysAuthUser>()
            {
                new SysAuthUser()
                {
                    Id = Role.System.GetIdentifier(),
                    Name = "system",
                    Code = "system",
                    Password = SystemConfig.Config.DefaultPassword,
                    RoleId = Role.System.GetIdentifier(),
                    RoleName = Role.System.GetDescription(),
                    UserId = Role.System.GetIdentifier(),
                    IsLock = true,
                    TryTimes = 0
                },
                new SysAuthUser()
                {
                    Id = Role.Admin.GetIdentifier(),
                    Name = "admin",
                    Code = "admin",
                    Password = SystemConfig.Config.DefaultPassword,
                    RoleId = Role.Admin.GetIdentifier(),
                    RoleName = Role.Admin.GetDescription(),
                    UserId = Role.Admin.GetIdentifier(),
                    IsLock = false,
                    TryTimes = 0
                }
            };
        }

        public List<SysMenu> GetSysMenus()
        {
            var resource = Guid.NewGuid().ToString();
            var setting = Guid.NewGuid().ToString();

            return new List<SysMenu>()
            {
                new SysMenu() { Id = resource, Name = "资源管理", Router = "resource", Icon = "folder", MenuIndex = 10000, IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "文件管理", Router = "filemanage", Icon = "", MenuIndex = 10005, ParentId = resource, ParentName = "资源管理", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "图库", Router = "gallery", Icon = "", MenuIndex = 10010, ParentId = resource, ParentName = "资源管理", IsEnable = true },
                new SysMenu() { Id = setting, Name = "系统设置", Router = "setting", Icon = "setting", MenuIndex = 11000, IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "菜单管理", Router = "menus", Icon = "", MenuIndex = 11005, ParentId = setting, ParentName = "系统设置", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "数据库管理", Router = "db", Icon = "", MenuIndex = 11010, ParentId = setting, ParentName = "系统设置", IsEnable = true },
                new SysMenu() {Id = Guid.NewGuid().ToString(), Name = "作业管理", Router = "job", Icon = "", MenuIndex = 11015, ParentId = setting, ParentName = "系统设置", IsEnable = true},
                new SysMenu() {Id = Guid.NewGuid().ToString(), Name = "用户信息", Router = "users", Icon = "", MenuIndex = 11020, ParentId = setting, ParentName = "系统设置", IsEnable = true},
                new SysMenu() {Id = Guid.NewGuid().ToString(), Name = "选项集", Router = "options", Icon = "", MenuIndex = 11025, ParentId = setting, ParentName = "系统设置", IsEnable = true},
                new SysMenu() {Id = Guid.NewGuid().ToString(), Name = "系统参数", Router = "config", Icon = "", MenuIndex = 11030, ParentId = setting, ParentName = "系统设置", IsEnable = true},
                new SysMenu() {Id = Guid.NewGuid().ToString(), Name = "角色管理", Router = "role", Icon = "", MenuIndex = 11035, ParentId = setting, ParentName = "系统设置", IsEnable = true},
            };
        }

        public List<SysUser> GetSysUsers()
        {
            return new List<SysUser>()
            {
                new SysUser()
                {
                    Id = Role.Admin.GetIdentifier(),
                    Name = "admin",
                    Code = "admin",
                    Gender = 9,
                    GenderName = "未知",
                    Realname = "系统管理员",
                    RoleId = Role.Admin.GetIdentifier(),
                    RoleName = Role.Admin.GetDescription(),
                    isActive = true,
                    isActiveName = "是"
                },
                new SysUser()
                {
                    Id = Role.System.GetIdentifier(),
                    Name = "system",
                    Code = "system",
                    Gender = 9,
                    GenderName = "未知",
                    Realname = "系统",
                    RoleId = Role.System.GetIdentifier(),
                    RoleName = Role.System.GetDescription(),
                    isActive = false,
                    isActiveName = "否"
                }
            };
        }
    }
}
