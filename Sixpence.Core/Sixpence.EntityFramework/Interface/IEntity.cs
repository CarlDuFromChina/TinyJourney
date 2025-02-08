using Sixpence.EntityFramework.Entity;
using Sixpence.EntityFramework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    /// <summary>
    /// 实体接口
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// 实体映射
        /// </summary>
        IDbEntityMap? EntityMap { get; }

        /// <summary>
        /// 主键字段
        /// </summary>
        IPrimaryEntityColumn PrimaryColumn { get; }

        /// <summary>
        /// 字段集合
        /// </summary>
        IList<IEntityColumn> Columns { get; }
    }
}
