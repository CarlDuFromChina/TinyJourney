using Sixpence.ORM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.Entity;
using Sixpence.ORM;

namespace Sixpence.Web.Auth.Role
{
    /// <summary>
    /// 普通用户
    /// </summary>
    public class UserRole : BasicRole
    {
        public override Role Role => Role.User;

        public override string Description => "普通用户：拥有修改特定部分实体权限";

        public override IDictionary<string, IEnumerable<SysRolePrivilege>> GetMissingPrivilege(IEntityManager manager)
        {
            var dic = new Dictionary<string, IEnumerable<SysRolePrivilege>>();

            dic.Add(RoleType.Entity.ToString(), GetMissingEntityPrivileges(manager).Select(item => GenerateRolePrivilege(item, GetSysRole(), (int)OperationType.Read + (int)OperationType.Write + (int)OperationType.Delete)));
            dic.Add(RoleType.Menu.ToString(), GetMissingMenuPrivileges(manager).Select(item => GenerateRolePrivilege(item, GetSysRole(), 0)));

            return dic;
        }
    }
}
