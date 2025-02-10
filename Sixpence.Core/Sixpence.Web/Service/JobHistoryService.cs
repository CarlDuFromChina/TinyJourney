using Microsoft.Extensions.Logging;
using Sixpence.EntityFramework;
using Sixpence.Web.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.Service
{
    public class JobHistoryService : EntityService<JobHistory>
    {
        public JobHistoryService(IEntityManager manager, ILogger<EntityService<JobHistory>> logger, IRepository<JobHistory> repository) : base(manager, logger, repository)
        {
        }
    }
}
