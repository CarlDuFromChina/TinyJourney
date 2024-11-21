using Microsoft.Extensions.Logging;
using Sixpence.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    public class BaseService
    {
        protected IEntityManager _manager;
        protected ILogger _logger;

        public BaseService()
        {
            _manager = new EntityManager();
            _logger = AppContext.GetLogger<AppContext>();
        }

        public BaseService(IEntityManager manager)
        {
            _manager = manager;
            _logger = AppContext.GetLogger<AppContext>();
        }
    }
}
