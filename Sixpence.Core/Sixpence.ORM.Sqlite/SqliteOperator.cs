using Dapper;
using Microsoft.Data.Sqlite;
using Sixpence.ORM.Mappers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.ORM.Sqlite
{
    internal class SqliteOperator : IDbOperator
    {
        public void BulkCopy(IDbConnection conn, IDbTransaction transaction, DataTable dataTable, string tableName)
        {
            // 构建插入命令的文本
            var commandText = BuildInsertCommandText(dataTable, tableName);

            // 打开数据库连接
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            bool needCommit = false;

            try
            {
                if (transaction == null)
                {
                    transaction = conn.BeginTransaction();
                    needCommit = true;
                }

                foreach (DataRow row in dataTable.Rows)
                {
                    using (var cmd = new SqliteCommand(commandText, conn as SqliteConnection))
                    {
                        cmd.Transaction = transaction as SqliteTransaction;
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            cmd.Parameters.AddWithValue($"@{column.ColumnName}", row[column]);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }

                if (needCommit)
                {
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                if (needCommit)
                {
                    transaction.Rollback();
                    transaction.Dispose();
                }
                else
                {
                    throw ex;
                }
            }
        }

        private string BuildInsertCommandText(DataTable dataTable, string tableName)
        {
            var columnNames = new StringBuilder();
            var values = new StringBuilder();

            foreach (DataColumn column in dataTable.Columns)
            {
                if (columnNames.Length > 0)
                {
                    columnNames.Append(", ");
                    values.Append(", ");
                }

                columnNames.Append(column.ColumnName);
                values.Append($"@{column.ColumnName}");
            }

            return $"INSERT INTO {tableName} ({columnNames}) VALUES ({values})";
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
