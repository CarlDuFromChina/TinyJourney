using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sixpence.Web.Module.SysAttrs;
using Sixpence.Web.Service;

namespace Sixpence.Web.Controller
{
    public class SysAttrsController : EntityBaseController<SysAttrs, SysAttrsService>
    {
    }
}