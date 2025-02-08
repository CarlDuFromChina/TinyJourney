using System.Collections.Generic;
using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Sixpence.Web.Entity;
using Sixpence.Web.Service;

namespace Sixpence.Web.Controllers
{
    public class SysRolePrivilegeController : EntityBaseController<SysRolePrivilege, SysRolePrivilegeService>
    {
        public SysRolePrivilegeController(SysRolePrivilegeService service) : base(service)
        {
        }

        [HttpGet("{roleid}/{roleType}")]
        public IEnumerable<SysRolePrivilege> GetUserPrivileges(string roleid, RoleType roleType)
        {
            return _service.GetUserPrivileges(roleid, roleType);
        }

        [HttpPost("bulk_save")]
        public void BulkSave([FromBody] string dataList)
        {
            var privileges = string.IsNullOrEmpty(dataList) ? null : JsonConvert.DeserializeObject<List<SysRolePrivilege>>(dataList);
            _service.BulkSave(privileges);
        }
    }
}
