using Sixpence.EntityFramework;
using Sixpence.TinyJourney.Entity;
using Sixpence.Web;

namespace Sixpence.TinyJourney.Service
{
    public class LinkService : EntityService<Link>
    {
        public LinkService(IEntityManager manager, ILogger<EntityService<Link>> logger, IRepository<Link> repository) : base(manager, logger, repository)
        {
        }
    }
}
