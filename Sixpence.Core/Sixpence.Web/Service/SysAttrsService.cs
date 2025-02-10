using Microsoft.Extensions.Logging;
using Sixpence.EntityFramework;
using Sixpence.Web.Module.SysAttrs;

namespace Sixpence.Web.Service
{
    public class SysAttrsService : EntityService<SysAttrs>
    {
        public SysAttrsService(IEntityManager manager, ILogger<EntityService<SysAttrs>> logger, IRepository<SysAttrs> repository) : base(manager, logger, repository)
        {
        }
    }
}