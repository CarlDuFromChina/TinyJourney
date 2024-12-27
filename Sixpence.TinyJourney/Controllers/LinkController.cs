using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixpence.TinyJourney.Service;
using Sixpence.TinyJourney.Entity;
using Sixpence.Web.Model;

namespace Sixpence.TinyJourney.Controller
{
    public class LinkController : EntityBaseController<Link, LinkService>
    {
        [HttpGet("search"), AllowAnonymous]
        public override DataModel<Link> GetViewData(string? pageSize, string? pageIndex, string? searchList, string? viewId, string? searchValue)
        {
            return base.GetViewData(pageSize, pageIndex, searchList, viewId, searchValue);
        }
    }
}
