using Sixpence.EntityFramework.Entity;
using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.TinyJourney.Service;
using Sixpence.TinyJourney.Entity;
using Sixpence.Web.Model;

namespace Sixpence.TinyJourney.Controller
{
    public class IdeaController : EntityBaseController<Idea, IdeaSerivice>
    {
        [HttpGet("{id}"), AllowAnonymous]
        public override Idea GetData(string id)
        {
            return base.GetData(id);
        }

        [HttpGet("search"), AllowAnonymous]
        public override DataModel<Idea> GetViewData(string? pageSize, string? pageIndex, string? searchList, string? orderBy, string? viewId, string? searchValue)
        {
            return base.GetViewData(pageSize, pageIndex, searchList, orderBy, viewId, searchValue);
        }
    }
}
