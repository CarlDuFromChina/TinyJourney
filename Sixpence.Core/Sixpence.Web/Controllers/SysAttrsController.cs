using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sixpence.Web.Module.SysAttrs;
using Sixpence.Web.Service;

namespace Sixpence.Web.Controllers
{
    public class SysAttrsController : EntityBaseController<SysAttrs, SysAttrsService>
    {
        public SysAttrsController(SysAttrsService service) : base(service)
        {
        }
    }
}