using Sixpence.Web.Entity;
using System.Collections.Generic;
using Sixpence.Web.Model;
using Sixpence.Web.EntityOptionProvider;
using Sixpence.Web;
using Sixpence.EntityFramework;

namespace Sixpence.TinyJourney.EntityOptionProvider
{
    public class CategoryEntityOptionProvider : IEntityOptionProvider
    {
        public IEnumerable<SelectOption> GetOptions()
        {
            var manager = new EntityManager();
            return manager.Query<SelectOption>($"select code AS Value, name AS Name from category");
        }
    }
}
