using Microsoft.Extensions.Logging;
using Sixpence.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    public abstract class BaseService<TService> where TService : class
    {
        protected IEntityManager _manager;
        protected ILogger<TService> _logger;

        public BaseService(IEntityManager manager, ILogger<TService> logger)
        {
            _manager = manager;
            _logger = logger;
        }
    }
}
