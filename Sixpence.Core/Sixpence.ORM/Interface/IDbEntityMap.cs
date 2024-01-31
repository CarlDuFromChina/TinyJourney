using Sixpence.ORM.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.ORM.Interface
{
    public interface IDbEntityMap
    {
        public string Table { get; set; }
        public string Schema { get; set; }
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
        public string Description { get; set; }
        public IList<IDbPropertyMap> Properties { get; set; }
    }
}
