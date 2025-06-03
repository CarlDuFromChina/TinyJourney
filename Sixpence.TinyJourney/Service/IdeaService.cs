using Sixpence.EntityFramework;
using Sixpence.TinyJourney.Entity;
using Sixpence.Web;

namespace Sixpence.TinyJourney.Service
{
    public class IdeaService : EntityService<Idea>
    {
        public IdeaService(IEntityManager manager, ILogger<EntityService<Idea>> logger, IRepository<Idea> repository) : base(manager, logger, repository)
        {
        }
    }
}
