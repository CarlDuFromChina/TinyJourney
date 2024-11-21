using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    public interface IEntityManagerBeforeCreateOrUpdate
    {
        void Execute(EntityManagerPluginContext context);
    }
}
