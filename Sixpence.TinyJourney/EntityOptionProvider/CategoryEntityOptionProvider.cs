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
        private IEntityManager _manager;
        public CategoryEntityOptionProvider(IEntityManager manager)
        {
            _manager = manager;
        }

        public IEnumerable<SelectOption> GetOptions()
        {
            return _manager.Query<SelectOption>($"select code AS Value, name AS Name from category");
        }
    }
}
