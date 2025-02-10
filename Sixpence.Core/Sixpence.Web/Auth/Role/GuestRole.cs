using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;

namespace Sixpence.Web.Auth.Role
{
    /// <summary>
    /// 访客
    /// </summary>
    public class GuestRole : BasicRole
    {
        public GuestRole(IEntityManager manager) : base(manager)
        {
        }

        public override Role Role => Role.Guest;

        public override string Description => "匿名用户：一般情况下无修改更新权限，只读";

        public override IDictionary<string, IEnumerable<SysRolePrivilege>> GetMissingPrivilege(IEntityManager manager)
        {
            var dic = new Dictionary<string, IEnumerable<SysRolePrivilege>>();

            dic.Add(RoleType.Entity.ToString(), GetMissingEntityPrivileges(manager).Select(item => GenerateRolePrivilege(item, this.GetSysRole(), (int)OperationType.Read)));
            dic.Add(RoleType.Menu.ToString(), GetMissingMenuPrivileges(manager).Select(item => GenerateRolePrivilege(item, this.GetSysRole(), 0)));

            return dic;
        }
    }
}
