using Sixpence.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Entity;
using Sixpence.Web.Auth.Role;
using Sixpence.ORM;

namespace Sixpence.Web
{
    public interface IRole
    {
        /// <summary>
        /// 获取唯一键
        /// </summary>
        string GetRoleKey { get; }

        /// <summary>
        /// 角色名
        /// </summary>
        Role Role { get; }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        SysRole GetSysRole();

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        IEnumerable<SysRolePrivilege> GetRolePrivilege();

        /// <summary>
        /// 清除缓存
        /// </summary>
        void ClearCache();

        /// <summary>
        /// 获取初始化权限
        /// </summary>
        /// <returns></returns>
        IDictionary<string, IEnumerable<SysRolePrivilege>> GetMissingPrivilege(IEntityManager manager);
    }

    public enum RoleType
    {
        All,
        Entity,
        Menu
    }
}
