using Sixpence.EntityFramework.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework.Interface
{
    /// <summary>
    /// 实体映射
    /// </summary>
    public interface IDbEntityMap
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// 模式，根据不同数据库各自实现
        /// SqlServer中为Schema，Mysql中为Database，Oracle中为User，Sqlite中为null，PostgreSql中为Schema，DB2中为Schema，Firebird中为Schema，Access中为null
        /// </summary>
        public string Schema { get; set; }

        /// <summary>
        /// 完整限定名，模式+表名
        /// </summary>
        public string FullQualifiedName
        {
            get
            {
                // Sqlite 不支持 Schema
                if (string.IsNullOrEmpty(Schema))
                {
                    return Table;
                }
                return $"{Schema}.{Table}";
            }
        }

        /// <summary>
        /// 表描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 实体属性
        /// </summary>
        public IList<IDbPropertyMap> Properties { get; set; }
    }
}
