using Sixpence.EntityFramework.Entity;
using Sixpence.Web.Module.SysMenu;
using Sixpence.Common;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Entity;
using Sixpence.Web.Model;
using System.ComponentModel;
using Sixpence.EntityFramework;
using Sixpence.Web.Extensions;

namespace Sixpence.Web.Auth.Role
{
    public abstract class BasicRole : IRole
    {
        public IEntityManager Manager = new EntityManager();
        protected const string ROLE_PREFIX = "BasicRole";
        protected const string PRIVILEGE_PREFIX = "RolePrivilege";

        public string GetRoleKey => GetType().Name;

        /// <summary>
        /// 系统角色
        /// </summary>
        public abstract Role Role { get; }
        public abstract string Description { get; }
        public string RoleId => "{848FA7AC-C4E3-413A-B531-6EA2661B6B70}"; // Role.ToString("D");

        /// <summary>
        /// 系统角色名
        /// </summary>
        public string RoleName => Role.ToString();

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        public SysRole GetSysRole()
        {
            var key = $"{ROLE_PREFIX}_{RoleName}";
            return MemoryCacheUtil.GetOrAddCacheItem(key, () =>
            {
                var role = Manager.QueryFirst<SysRole>(new { name = Role.GetDescription() });
                if (role == null)
                {
                    role = new SysRole()
                    {
                        Id = Role.GetIdentifier(),
                        Name = Role.GetDescription(),
                        Description = this.Description,
                        IsBasic = true
                    };
                    Manager.Create(role);
                }
                return role;
            }, DateTime.Now.AddHours(12));
        }

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SysRolePrivilege> GetRolePrivilege()
        {
            var key = $"{PRIVILEGE_PREFIX}_{Role.GetIdentifier()}";
            return MemoryCacheUtil.GetOrAddCacheItem(key, () =>
            {
                var dataList = Manager.Query<SysRolePrivilege>(new { role_id = Role.GetIdentifier() });
                return dataList;
            }, DateTime.Now.AddHours(12));
        }

        /// <summary>
        /// 清除角色缓存
        /// </summary>
        public void ClearCache()
        {
            MemoryCacheUtil.RemoveCacheItem($"{PRIVILEGE_PREFIX}_{Role.GetIdentifier()}");
            MemoryCacheUtil.RemoveCacheItem($"{ROLE_PREFIX}_{Role.GetIdentifier()}");
        }

        /// <summary>
        /// 获取缺失权限
        /// </summary>
        /// <returns></returns>
        public abstract IDictionary<string, IEnumerable<SysRolePrivilege>> GetMissingPrivilege(IEntityManager manager);

        /// <summary>
        /// 获取缺失实体权限
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<SysEntity> GetMissingEntityPrivileges(IEntityManager manager)
        {
            var role = GetSysRole();
            var paramList = new Dictionary<string, object>() { { "@id", role.Id } };
            var sql = @"
SELECT * FROM sys_entity
WHERE id NOT IN (
	SELECT object_id FROM sys_role_privilege
	WHERE object_type = 'sys_entity' AND role_id = @id
)
";
            return manager.Query<SysEntity>(sql, paramList);
        }

        /// <summary>
        /// 获取缺失菜单权限
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<SysMenu> GetMissingMenuPrivileges(IEntityManager manager)
        {
            var role = GetSysRole();
            var paramList = new Dictionary<string, object>() { { "@id", role.Id } };
            var sql = @"
SELECT * FROM sys_menu
WHERE id NOT IN (
	SELECT object_id FROM sys_role_privilege
	WHERE object_type = 'sys_menu' AND role_id = @id
)
";
            var dataList = manager.Query<SysMenu>(sql, paramList);
            return dataList;
        }

        /// <summary>
        /// 生成权限（仅限于实体和菜单权限）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="role"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SysRolePrivilege GenerateRolePrivilege(BaseEntity entity, SysRole role, int value)
        {
            var privilege = new SysRolePrivilege()
            {
                Id = Guid.NewGuid().ToString(),
                ObjectId = entity.PrimaryColumn.Value?.ToString(),
                ObjectName = EntityCommon.GetAttributeValue(entity, "Name")?.ToString(),
                ObjectType = entity.EntityMap.Table,
                RoleId = role.Id,
                RoleName = role.Name,
                Privilege = value,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            return privilege;
        }
    }

    /// <summary>
    /// 基础系统角色
    /// </summary>
    public enum Role
    {
        /// <summary>
        /// 拥有系统所有权限
        /// </summary>
        [Description("系统管理员")]
        Admin = 0,
        /// <summary>
        /// 系统角色
        /// </summary>
        [Description("系统")]
        System = 1,
        /// <summary>
        /// 拥有自己的权限
        /// </summary>
        [Description("用户")]
        User = 2,
        /// <summary>
        /// 拥有只读权限
        /// </summary>
        [Description("访客")]
        Guest = 3
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public static class OperationTypeValue
    {
        /// <summary>
        /// 写
        /// </summary>
        public static SelectOption Write => new SelectOption("写", "write");

        /// <summary>
        /// 删
        /// </summary>
        public static SelectOption Delete => new SelectOption("删", "delete");

        /// <summary>
        /// 读
        /// </summary>
        public static SelectOption Read => new SelectOption("读", "read");
    }

    /// <summary>
    /// 操作类型枚举
    /// </summary>
    public enum OperationType
    {
        [Description("读")]
        Read = 1,
        [Description("写")]
        Write = 2,
        [Description("删")]
        Delete = 4
    }
}
