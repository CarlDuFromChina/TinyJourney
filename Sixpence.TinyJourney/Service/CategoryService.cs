using Sixpence.TinyJourney.Entity;
using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.EntityFramework;
using Sixpence.Web;
using Sixpence.Web.Model;

namespace Sixpence.TinyJourney.Service
{
    public class CategoryService : EntityService<Category>
    {
        public CategoryService(IEntityManager manager, ILogger<EntityService<Category>> logger, IRepository<Category> repository) : base(manager, logger, repository)
        {
        }

        public override IEnumerable<SelectOption> GetOptions()
        {
            var sql = $"SELECT code AS Value, name as Name FROM {new Category().EntityMap.FullQualifiedName}";
            return _manager.Query<SelectOption>(sql);
        }
    }
}
