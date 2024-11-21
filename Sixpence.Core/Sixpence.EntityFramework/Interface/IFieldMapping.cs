using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    /// <summary>
    /// C# 数据类型与数据库数据类型映射
    /// </summary>
    public interface IFieldMapping
    {
        Dictionary<Type, string> GetFieldMappings();
    }
}
