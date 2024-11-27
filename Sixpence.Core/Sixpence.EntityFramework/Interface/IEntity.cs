using Sixpence.EntityFramework.Entity;
using Sixpence.EntityFramework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    public interface IEntity
    {
        IDbEntityMap? EntityMap { get; }
        IPrimaryEntityColumn PrimaryColumn { get; }
        IList<IEntityColumn> Columns { get; }
    }
}
