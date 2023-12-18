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
    /// 高级用户
    /// </summary>
    public class SystemRole : BasicRole
    {
        public override Role Role => Role.System;

        public override string Description => "系统角色：拥有最高权限";

        public override IDictionary<string, IEnumerable<SysRolePrivilege>> GetMissingPrivilege(IEntityManager manager)
        {
            var dic = new Dictionary<string, IEnumerable<SysRolePrivilege>>();

            dic.Add(RoleType.Entity.ToString(), GetMissingEntityPrivileges(manager).Select(item => GenerateRolePrivilege(item, this.GetSysRole(), (int)OperationType.Read + (int)OperationType.Write + (int)OperationType.Delete)));
            dic.Add(RoleType.Menu.ToString(), GetMissingMenuPrivileges(manager).Select(item => GenerateRolePrivilege(item, this.GetSysRole(), (int)OperationType.Read)));

            return dic;
        }
    }
}
