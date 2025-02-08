using Sixpence.TinyJourney.Entity;
using Sixpence.TinyJourney.Service;
using Sixpence.Web.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.TinyJourney.Controller
{
    public class CategoryController : EntityBaseController<Category, CategoryService>
    {
        public CategoryController(CategoryService service) : base(service)
        {
        }
    }
}
