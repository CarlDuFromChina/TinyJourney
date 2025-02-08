using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sixpence.Web.Service;
using Sixpence.Web.Module.SysMenu;

namespace Sixpence.Web.Controllers
{
    public class SysMenuController : EntityBaseController<SysMenu, SysMenuService>
    {
        public SysMenuController(SysMenuService service) : base(service)
        {
        }

        [HttpGet]
        [Route("first_menu")]
        public IList<SysMenu> GetFirstMenu()
        {
            return _service.GetFirstMenu();
        }
    }
}