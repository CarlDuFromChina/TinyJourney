using Sixpence.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework.Sqlite
{
    public class SqliteSqlBuilder : ISqlBuilder
    {
        public char ParameterPrefix => '@';

        public string Schema => "";

        public string BuildAddColumnSql(string tableName, IList<IDbPropertyMap> columns)
        {
            var sql = new StringBuilder();
            var tempSql = $@"ALTER TABLE {tableName}";
            foreach (var item in columns)
            {
                var require = item.CanBeNull == false ? " NOT NULL" : "";
                var length = item.Length != null ? $"({item.Length})" : "";
                var unique = item.IsUnique == true ? " UNIQUE" : "";
                var defaultValue = item.DefaultValue == null ? "" : item.DefaultValue is string ? $"DEFAULT ('{item.DefaultValue}')" : $"DEFAULT ({item.DefaultValue})";
                sql.Append($"{tempSql} ADD COLUMN IF NOT EXISTS {item.Name} {item.DbType}{length} {require} {unique} {defaultValue};\r\n");
            }
            return sql.ToString();
        }

        public string BuildCreateOrUpdateSQL(string tableName, string updatedColumns, string values, string primaryKeys, string updatedValues)
        {
            var templateSQL = $@"INSERT INTO {tableName} ({updatedColumns}) VALUES ({values})
ON CONFLICT ({primaryKeys}) DO UPDATE SET {updatedValues};";
            return templateSQL;
        }

        public string BuildCreateTemporaryTableSql(string tableName, string tempTableName)
        {
            return $@"CREATE TEMP TABLE {tempTableName} AS
SELECT * FROM {tableName} WHERE 0;";
        }

        public string BuildDropColumnSql(string tableName, IList<string> columns)
        {
            var sql = $@"
ALTER TABLE {tableName}
";
            var count = 0;
            foreach (var item in columns)
            {
                var itemSql = $"DROP COLUMN IF EXISTS {item} {(++count == columns.Count ? ";" : ",")}";
                sql += itemSql;
            }
            return sql;
        }

        public string BuildDropTableSql(string tableName)
        {
            return $"DROP TABLE IF EXISTS {tableName}";
        }

        public (string sql, Dictionary<string, object> param) BuildInClauseSql(string parameter, int count, List<object> value, bool isNotIn = false)
        {
            var parameters = new Dictionary<string, object>();
            var parameterNames = new List<string>();
            value.ForEach(item =>
            {
                var parameterName = $"{ParameterPrefix}{parameter}{count++}";
                parameters.Add(parameterName, item);
            });
            if (isNotIn)
            {
                return ($@"NOT IN ({string.Join(",", parameterNames)})", parameters);
            }
            return ($@"IN ({string.Join(",", parameterNames)})", parameters);
        }

        public string BuildPageSql(int? index, int size)
        {
            if (index.HasValue)
            {
                return $" LIMIT {size} OFFSET {(index - 1) * size}";
            }
            else
            {
                return $" LIMIT {size}";
            }
        }

        public string BuildTableExsitSql(string tableName)
        {
            return $@"SELECT COUNT(1) > 0
FROM sqlite_master
WHERE type='table' AND name='{tableName}';";
        }

        public (string name, object value) HandleParameter(string name, object value)
        {
            return (name, value);
        }
    }
}
