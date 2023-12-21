using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Sixpence.ORM.Postgres
{
    public class PostgresFieldMapping : IFieldMapping
    {
        public Dictionary<Type, string> GetFieldMappings()
        {
            var typeMappings = new Dictionary<Type, string>()
            {
                { typeof(short), "int2" }, // 或 "smallint"
                { typeof(short?), "int2" }, // 可空版本
                { typeof(int), "int4" }, // 或 "integer"
                { typeof(int?), "int4" }, // 可空版本
                { typeof(long), "int8" }, // 或 "bigint"
                { typeof(long?), "int8" }, // 可空版本
                { typeof(decimal), "numeric" },
                { typeof(decimal?), "numeric" }, // 可空版本
                { typeof(float), "real" }, // 单精度浮点数
                { typeof(float?), "real" }, // 可空版本
                { typeof(double), "double precision" },
                { typeof(double?), "double precision" }, // 可空版本
                { typeof(DateTime), "timestamp without time zone" }, // 无时区时间戳
                { typeof(DateTime?), "timestamp without time zone" }, // 可空版本
                { typeof(DateTimeOffset), "timestamp with time zone" }, // 有时区时间戳
                { typeof(DateTimeOffset?), "timestamp with time zone" }, // 可空版本
                { typeof(bool), "boolean" },
                { typeof(bool?), "boolean" }, // 可空版本
                { typeof(string), "text" },
                { typeof(char), "char" }, // 单个字符
                { typeof(char?), "char" }, // 可空版本
                { typeof(Guid), "uuid" }, // 唯一标识符
                { typeof(Guid?), "uuid" }, // 可空版本
                { typeof(byte[]), "bytea" }, // 字节数据类型
                { typeof(TimeSpan), "interval" }, // 时间间隔
                { typeof(TimeSpan?), "interval" }, // 可空版本
                { typeof(byte), "smallint" }, // 一个字节可能映射为smallint
                { typeof(byte?), "smallint" } // 可空版本
            };
            return typeMappings;
        }
    }
}
