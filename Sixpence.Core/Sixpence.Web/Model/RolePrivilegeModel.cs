using Sixpence.Web.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Model
{
    public class RolePrivilegeModel
    {
        public SysRole Role { get; set; }
        public IEnumerable<SysRolePrivilege> Privileges { get; set; }
    }
}
