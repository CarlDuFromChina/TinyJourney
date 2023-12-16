﻿using Sixpence.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Module.SysMenu;
using Sixpence.ORM.Repository;
using Sixpence.Web.Entity;
using Sixpence.ORM.Entity;
using Sixpence.ORM;

namespace Sixpence.Web.Service
{
    public class SysRolePrivilegeService : EntityService<SysRolePrivilege>
    {
        #region 构造函数
        public SysRolePrivilegeService() : base() { }

        public SysRolePrivilegeService(IEntityManager manger)
        {
            Repository = new Repository<SysRolePrivilege>(manger);
        }
        #endregion

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="roleid"></param>
        /// <returns></returns>
        public IEnumerable<SysRolePrivilege> GetUserPrivileges(string roleid, RoleType roleType)
        {
            var role = Manager.QueryFirst<SysRole>(roleid);
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
                privileges = Manager.Query<SysRolePrivilege>(sql, new Dictionary<string, object>() { { "@id", roleid } }).ToList();
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
            return Manager.Query<SysRolePrivilege>(sql, new Dictionary<string, object>() { { "@id", entityid } });
        }

        /// <summary>
        /// 批量更新或创建
        /// </summary>
        /// <param name="dataList"></param>
        public void BulkSave(List<SysRolePrivilege> dataList)
        {
            Manager.BulkCreateOrUpdate(dataList);
        }

        /// <summary>
        /// 自动生成权限
        /// </summary>
        public void CreateRoleMissingPrivilege()
        {
            var roles = ServiceFactory.ResolveAll<IRole>();
            var privileges = new List<SysRolePrivilege>();

            roles.Each(item =>
            {
                item.GetMissingPrivilege(Manager)
                    .Each(item =>
                    {
                        if (!item.Value.IsEmpty())
                        {
                            privileges.AddRange(item.Value);
                        }
                    });
            });

            Manager.ExecuteTransaction(() => Manager.BulkCreate(privileges));
        }
    }
}
