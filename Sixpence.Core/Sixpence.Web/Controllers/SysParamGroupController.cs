using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.Web.Service;

namespace Sixpence.Web.Controllers
{
    public class SysParamGroupController : EntityBaseController<SysParamGroup, SysParamGroupService>
    {
        public SysParamGroupController(SysParamGroupService service) : base(service)
        {
        }

        [HttpGet("options")]
        public IEnumerable<object> GetParams(string code)
        {
            var codeList = code.Split(',');
            return _service.GetParamsList(codeList);
        }

        [HttpGet("entity_options")]
        public IEnumerable<IEnumerable<SelectOption>> GetEntityOptions(string code)
        {
            var codeList = new string[] { };
            if (!string.IsNullOrEmpty(code))
            {
                codeList = code.Split(',');
            }
            return _service.GetEntityOptions(codeList);
        }
    }
}