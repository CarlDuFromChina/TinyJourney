using Microsoft.Extensions.Logging;
using Sixpence.EntityFramework;
using Sixpence.Web.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sixpence.Web.Service
{
    public class SysParamService : EntityService<SysParam>
    {
        public SysParamService(IEntityManager manager, ILogger<EntityService<SysParam>> logger, IRepository<SysParam> repository) : base(manager, logger, repository)
        {
        }
    }
}