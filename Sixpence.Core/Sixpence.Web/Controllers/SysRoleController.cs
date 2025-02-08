using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.Web.Service;

namespace Sixpence.Web.Controllers
{
    public class SysRoleController : EntityBaseController<SysRole, SysRoleService>
    {
        public SysRoleController(SysRoleService service) : base(service)
        {
        }

        [HttpGet("basic_role_options")]
        public IEnumerable<SelectOption> GetBasicRole()
        {
            return _service.GetBasicRole();
        }

        [HttpGet("role_options")]
        public IEnumerable<SelectOption> GetRoles()
        {
            return _service.GetRoles();
        }
    }
}
