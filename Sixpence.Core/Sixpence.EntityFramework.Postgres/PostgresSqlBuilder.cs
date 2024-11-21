using Dapper;
using Sixpence.EntityFramework.Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework.Postgres
{
    public class PostgresSqlBuilder : ISqlBuilder
    {
        public char ParameterPrefix => '@';

        public string Schema => "public";

        /// <summary>
        /// 获取临时表创建语句
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="tableName">目标表</param>
        /// <param name="tempTableName">临时表表名</param>
        /// <returns></returns>
        public string BuildCreateTemporaryTableSql(string tableName, string tempTableName)
        {
            return $@"CREATE TEMP TABLE {tempTableName}
ON COMMIT DROP AS SELECT * FROM {tableName}
WHERE 1!=1;";
        }

        /// <summary>
        /// 获取列新增获取语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public string BuildAddColumnSql(string tableName, IList<IDbPropertyMap> columns)
        {
            var sql = new StringBuilder();
            var tempSql = $@"ALTER TABLE {tableName}";
            foreach (var item in columns)
            {
                var require = item.CanBeNull == false ? " NOT NULL" : "";
                var length = item.Length != null ? $"({item.Length})" : "";
                var unique = item.IsUnique == true ? " UNIQUE" : "";
                var defaultValue = item.DefaultValue == null ? "" : item.DefaultValue is string ? $"DEFAULT '{item.DefaultValue}'" : $"DEFAULT {item.DefaultValue}";
                sql.Append($"{tempSql} ADD COLUMN IF NOT EXISTS {item.Name} {item.DbType}{length} {require} {unique} {defaultValue};\r\n");
            }
            return sql.ToString();
        }

        /// <summary>
        /// 获取列删除语句
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 表是否存在
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string BuildTableExsitSql(string tableName)
        {
            return $@"
SELECT COUNT(1) > 0 FROM pg_tables
WHERE schemaname = '{Schema}' AND tablename = '{tableName}'";
        }

        /// <summary>
        /// 添加查询数量限制
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="index"></param>
        /// <param name="size"></param>
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

        /// <summary>
        /// 创建或更新SQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="updatedColumns"></param>
        /// <param name="values"></param>
        /// <param name="primaryKeys"></param>
        /// <param name="updatedValues"></param>
        /// <returns></returns>
        public string BuildCreateOrUpdateSQL(string tableName, string updatedColumns, string values, string primaryKeys, string updatedValues)
        {
            var templateSQL = $@"INSERT INTO {tableName} ({updatedColumns}) VALUES ({values})
ON CONFLICT ({primaryKeys}) DO UPDATE SET {updatedValues};";
            return templateSQL;
        }

        /// <summary>
        /// 删除表SQL
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string BuildDropTableSql(string tableName)
        {
            return $"DROP TABLE IF EXISTS {tableName}";
        }

        /// <summary>
        /// 处理字段名和值的前缀和后缀
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public (string name, object value) HandleParameter(string name, object value)
        {
            return (name, value);
        }

        /// <summary>
        /// 获取 IN 句
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public (string sql, Dictionary<string, object> param) BuildInClauseSql(string parameter, int count, List<object> value, bool isNotIn = false)
        {
            if (isNotIn)
            {
                return ($@"<> ANY({ParameterPrefix}{parameter}{count})", new Dictionary<string, object>() { { $"{ParameterPrefix}{parameter}", value } });
            }
            return ($@"= ANY({ParameterPrefix}{parameter}{count})", new Dictionary<string, object>() { { $"{ParameterPrefix}{parameter}", value } });
        }
    }
}
