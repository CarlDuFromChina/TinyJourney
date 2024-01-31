using Dapper;
using Sixpence.ORM.Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.ORM.Sqlite
{
    internal class SqliteOperator : IDbOperator
    {
        public void BulkCopy(IDbConnection conn, DataTable dataTable, string tableName)
        {
            // 确保连接是 SQLiteConnection 类型
            if (conn is SQLiteConnection sqliteConn)
            {
                // 打开连接（如果尚未打开）
                if (sqliteConn.State != ConnectionState.Open)
                {
                    sqliteConn.Open();
                }

                // 开始一个事务
                using (var transaction = sqliteConn.BeginTransaction())
                {
                    // 构建插入命令的基本部分
                    var columns = string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName));
                    var parameters = string.Join(", ", dataTable.Columns.Cast<DataColumn>().Select(c => "@" + c.ColumnName));
                    var cmdText = $"INSERT INTO {tableName} ({columns}) VALUES ({parameters})";

                    using (var command = new SQLiteCommand(cmdText, sqliteConn))
                    {
                        // 为命令添加参数
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            command.Parameters.Add(new SQLiteParameter("@" + column.ColumnName));
                        }

                        // 遍历 DataTable 中的所有行
                        foreach (DataRow row in dataTable.Rows)
                        {
                            // 更新参数值
                            foreach (SQLiteParameter parameter in command.Parameters)
                            {
                                parameter.Value = row[parameter.ParameterName.Substring(1)] ?? DBNull.Value;
                            }

                            // 执行命令
                            command.ExecuteNonQuery();
                        }
                    }

                    // 提交事务
                    transaction.Commit();
                }
            }
            else
            {
                throw new ArgumentException("The provided IDbConnection is not a SQLiteConnection.");
            }
        }

        public IEnumerable<IDbPropertyMap> GetTableColumns(IDbConnection connection, string tableName)
        {
            var sql = $"PRAGMA table_info({tableName})";
            var result = connection.Query(sql);
            return result.Select(x => new DbPropertyMap
            {
                Name = x.name,
                DbType = x.type, // TEXT(100)
                CanBeNull = x.notnull == 0,
                IsKey = x.pk == 1
            });
        }
    }
}
