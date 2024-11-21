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
        ISormPrimaryColumn PrimaryColumn { get; }
        IList<ISormColumn> Columns { get; }
    }
}
