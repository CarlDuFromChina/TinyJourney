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
        [HttpGet("options")]
        public IEnumerable<object> GetParams(string code)
        {
            var codeList = code.Split(',');
            return new SysParamGroupService().GetParamsList(codeList);
        }

        [HttpGet("entity_options")]
        public IEnumerable<IEnumerable<SelectOption>> GetEntitiyList(string code)
        {
            var codeList = new string[] { };
            if (!string.IsNullOrEmpty(code))
            {
                codeList = code.Split(',');
            }
            return new SysParamGroupService().GetEntitiyList(codeList);
        }
    }
}