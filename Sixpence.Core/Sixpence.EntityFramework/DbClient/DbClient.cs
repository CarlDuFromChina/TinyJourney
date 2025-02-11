using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    /// <summary>
    /// 数据库客户端，SQL 查询的封装器，更为底层的数据库操作
    /// 所有SQL日志记录均在此处进行
    /// </summary>
    public class DbClient : IDisposable
    {
        private IDbDriver _driver;
        private int? _commandTimeout = 20;
        private readonly ILogger<DbClient>? _logger;
        private readonly bool _isLoggingEnabled = AppBuilderExtensions.BuilderOptions.EnableLogging;

        public int? CommandTimeout => _commandTimeout;
        public IDbConnection DbConnection { get; private set; }
        public IDbDriver Driver => _driver;

        internal DbClient(DbClientOptions options)
        {
            if (options == null)
            {
                options = new DbClientOptions();
            }
            _driver = options.DbDriver;
            DbConnection = options.DbDriver.GetDbConnection(options.ConnectionString);
            _commandTimeout = options.CommandTimeout;
            _logger = options.LoggerFactory?.CreateLogger<DbClient>();
        }

        /// <summary>
        ///获取数据库连接状态 
        /// </summary>
        /// <returns></returns>
        public ConnectionState ConnectionState => DbConnection.State;

        #region 开启数据库连接
        //数据库打开、关闭的计数器
        private int _dbOpenCounter;

        /// <summary>
        ///打开数据库的连接（如果已经Open，则忽略）
        /// </summary>
        public void Open()
        {
            //counter = 0代表没有打开过，否则说明已经打开过了，不需要再打开
            if (_dbOpenCounter++ == 0)
            {
                if (DbConnection.State != ConnectionState.Open)
                    DbConnection.Open();
            }

        }

        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        public void Close()
        {
            //counter先自减1，然后判断是否=0，是的话代表是最后一次关闭
            if (--_dbOpenCounter == 0)
            {
                if (DbConnection.State != ConnectionState.Closed)
                {
                    DbConnection?.Close();
                }
            }
        }
        #endregion

        #region 事务

        private IDbTransaction _trans;
        private int _transCounter = 0;

        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            if (_transCounter++ == 0)
            {
                _trans = DbConnection.BeginTransaction();
            }
            return _trans;
        }

        /// <summary>
        /// 提交数据库的事务
        /// </summary>
        public void CommitTransaction()
        {
            if (--_transCounter == 0)
            {
                _trans?.Commit();
                _trans?.Dispose();
                _trans = null;
            }
        }

        /// <summary>
        /// 回滚数据库的事务
        /// </summary>
        public void Rollback()
        {
            try
            {
                if (--_transCounter == 0)
                {
                    _trans?.Rollback();
                    _trans?.Dispose();
                    _trans = null;
                }
            }
            finally
            {
                if (_transCounter == 0)
                    _trans = null;
            }
        }
        #endregion

        #region Execute
        /// <summary>
        /// 执行SQL语句
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public int Execute(string sql, object? param = null)
        {
            var paramList = param.ToDictionary();

            if (_isLoggingEnabled)
                _logger.LogDebug(sql + paramList.ToLogString());

            return DbConnection.Execute(sql, param, commandTimeout: CommandTimeout);
        }

        /// <summary>
        /// 执行SQL语句，并返回第一行第一列
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, object? param = null)
        {
            var paramList = param.ToDictionary();

            if (_isLoggingEnabled)
                _logger.LogDebug(sql + paramList.ToLogString());

            return DbConnection.ExecuteScalar(sql, param, commandTimeout: CommandTimeout);
        }

        /// <summary>
        /// 执行SQL文件
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="sqlFile"></param>
        /// <returns></returns>
        public int ExecuteSqlScript(string sqlFile)
        {
            int returnValue = -1;
            int sqlCount = 0, errorCount = 0;
            if (!File.Exists(sqlFile))
            {
                _logger?.LogError($"文件({sqlFile})不存在");
                return -1;
            }
            using (StreamReader sr = new StreamReader(sqlFile))
            {
                string line = string.Empty;
                char spaceChar = ' ';
                string newLIne = "\r\n", semicolon = ";";
                string sprit = "/", whiffletree = "-";
                string sql = string.Empty;
                do
                {
                    line = sr.ReadLine();
                    // 文件结束
                    if (line == null) break;
                    // 跳过注释行
                    if (line.StartsWith(sprit) || line.StartsWith(whiffletree)) continue;
                    // 去除右边空格
                    line = line.TrimEnd(spaceChar);
                    sql += line;
                    // 以分号(;)结尾，则执行SQL
                    if (sql.EndsWith(semicolon))
                    {
                        try
                        {
                            sqlCount++;
                            Execute(sql);
                        }
                        catch (Exception ex)
                        {
                            errorCount++;
                            if (_isLoggingEnabled)
                                _logger?.LogError(sql + newLIne + ex.Message, ex);
                        }
                        sql = string.Empty;
                    }
                    else
                    {
                        // 添加换行符
                        if (sql.Length > 0) sql += newLIne;
                    }
                } while (true);
            }
            if (sqlCount > 0 && errorCount == 0)
                returnValue = 1;
            if (sqlCount == 0 && errorCount == 0)
                returnValue = 0;
            else if (sqlCount > errorCount && errorCount > 0)
                returnValue = -1;
            else if (sqlCount == errorCount)
                returnValue = -2;
            return returnValue;
        }
        #endregion

        #region Query
        /// <summary>
        /// 执行SQL语句，并返回查询结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object? param = null)
        {
            var paramList = param.ToDictionary();

            if (_isLoggingEnabled)
                _logger.LogDebug(sql + paramList.ToLogString());

            return DbConnection.Query<T>(sql, param, commandTimeout: CommandTimeout);
        }

        /// <summary>
        /// 执行SQL语句，并返回查询结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public T QueryFirst<T>(string sql, object? param = null)
        {
            var paramList = param.ToDictionary();

            if (_isLoggingEnabled)
                _logger.LogDebug(sql + paramList.ToLogString());

            return DbConnection.QueryFirstOrDefault<T>(sql, param, commandTimeout: CommandTimeout);
        }
        #endregion

        #region DataTable
        /// <summary>
        /// 执行SQL语句，并返回查询结果集
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public DataTable Query(string sql, object? param = null)
        {
            var paramList = param.ToDictionary();

            if (_isLoggingEnabled)
                _logger.LogDebug(sql + paramList.ToLogString());

            DataTable dt = new DataTable();
            var reader = DbConnection.ExecuteReader(sql, param, commandTimeout: CommandTimeout);
            dt.Load(reader);
            return dt;
        }
        #endregion

        /// <summary>
        /// 创建临时表
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public string CreateTemporaryTable(string tableName)
        {
            var tempTableName = $"{SchemaHelper.RemoveSchemaName(tableName)}_{DateTime.Now.ToString("yyyyMMddHHmmss")}";
            var sql = Driver.SqlBuilder.BuildCreateTemporaryTableSql(tableName, tempTableName);

            if (_isLoggingEnabled)
                _logger.LogDebug(sql);

            DbConnection.Execute(sql);
            return tempTableName;
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableName"></param>
        public void DropTable(string tableName)
        {
            var sql = _driver.SqlBuilder.BuildDropTableSql(tableName);

            if (_isLoggingEnabled)
                _logger?.LogDebug(sql);

            DbConnection.Execute(sql, commandTimeout: CommandTimeout);
        }

        /// <summary>
        /// 释放连接
        /// </summary>
        public void Dispose()
            => DbConnection.Dispose();

        /// <summary>
        /// 拷贝数据
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="tableName"></param>
        public void BulkCopy(DataTable dataTable, string tableName)
            => _driver.Operator.BulkCopy(DbConnection, _trans, dataTable, tableName);
    }
}
