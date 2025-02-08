using Sixpence.Web.Module.SysAttrs;
using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;

namespace Sixpence.Web.Controller
{
    public class SysEntityController : EntityBaseController<SysEntity, SysEntityService>
    {
        public SysEntityController(SysEntityService service) : base(service)
        {
        }

        /// <summary>
        /// 根据实体 id 查询字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("attrs")]
        public IList<SysAttrs> GetEntityAttrs(string id)
        {
            return _service.GetEntityAttrs(id);
        }
    }
}