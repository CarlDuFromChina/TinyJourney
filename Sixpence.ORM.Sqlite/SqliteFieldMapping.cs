using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.ORM.Sqlite
{
    public class SqliteFieldMapping : IFieldMapping
    {
        public Dictionary<Type, string> GetFieldMappings()
        {
            var typeMappings = new Dictionary<Type, string>()
            {
                { typeof(short), "INTEGER" }, // SQLite 中整数类型
                { typeof(short?), "INTEGER" }, // 可空版本
                { typeof(int), "INTEGER" },
                { typeof(int?), "INTEGER" }, // 可空版本
                { typeof(long), "INTEGER" },
                { typeof(long?), "INTEGER" }, // 可空版本
                { typeof(decimal), "REAL" }, // SQLite 使用 REAL 表示浮点数
                { typeof(decimal?), "REAL" }, // 可空版本
                { typeof(float), "REAL" },
                { typeof(float?), "REAL" }, // 可空版本
                { typeof(double), "REAL" },
                { typeof(double?), "REAL" }, // 可空版本
                { typeof(DateTime), "TEXT" }, // SQLite 使用 TEXT 存储日期
                { typeof(DateTime?), "TEXT" }, // 可空版本
                { typeof(DateTimeOffset), "TEXT" }, // 同样使用 TEXT 存储日期和时间
                { typeof(DateTimeOffset?), "TEXT" }, // 可空版本
                { typeof(bool), "INTEGER" }, // SQLite 使用 INTEGER 存储布尔值（0为false，非0为true）
                { typeof(bool?), "INTEGER" }, // 可空版本
                { typeof(string), "TEXT" },
                { typeof(char), "TEXT" }, // 单个字符也使用 TEXT
                { typeof(char?), "TEXT" }, // 可空版本
                { typeof(Guid), "TEXT" }, // 使用 TEXT 存储 GUID
                { typeof(Guid?), "TEXT" }, // 可空版本
                { typeof(byte[]), "BLOB" }, // 用于存储二进制数据
                { typeof(TimeSpan), "TEXT" }, // 时间间隔可以使用 TEXT 表示
                { typeof(TimeSpan?), "TEXT" }, // 可空版本
                { typeof(byte), "INTEGER" }, // 小整数类型
                { typeof(byte?), "INTEGER" } // 可空版本
            };
            return typeMappings;
        }

    }
}
