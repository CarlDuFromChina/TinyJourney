using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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
        private IDbDriver driver;
        private int? commandTimeout = 20;
        private readonly ILogger<DbClient> Logger;
        private readonly bool EnableLogging = AppBuilderExtensions.BuilderOptions.EnableLogging;

        public int? CommandTimeout => commandTimeout;
        public IDbConnection DbConnection { get; private set; }
        public IDbDriver Driver => driver; // 数据库驱动
        public ISqlBuilder SqlBuilder => driver.SqlBuilder; // 数据库方言
        public IDbOperator Operator => driver.Operator; // 数据库批量操作

        internal DbClient()
        {
            var dbSetting = ServiceCollectionExtensions.Options?.DbSetting;
            driver = dbSetting.Driver;
            DbConnection = driver.GetDbConnection(dbSetting.ConnectionString);
            if (dbSetting.CommandTimeout != null)
            {
                this.commandTimeout = dbSetting.CommandTimeout;
            }
            Logger = ServiceContainer.Provider.GetService<ILogger<DbClient>>();
        }

        internal DbClient(IDbDriver dbDriver, string connectionString, int? commandTimeout)
        {
            driver = dbDriver;
            DbConnection = dbDriver.GetDbConnection(connectionString);
            if (commandTimeout != null)
            {
                this.commandTimeout = commandTimeout;
            }
            Logger = ServiceContainer.Provider.GetService<ILogger<DbClient>>();
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

            if (EnableLogging)
                Logger.LogDebug(sql + paramList.ToLogString());

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

            if (EnableLogging)
                Logger.LogDebug(sql + paramList.ToLogString());

            return DbConnection.ExecuteScalar(sql, param, commandTimeout: CommandTimeout);
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

            if (EnableLogging)
                Logger.LogDebug(sql + paramList.ToLogString());

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

            if (EnableLogging)
                Logger.LogDebug(sql + paramList.ToLogString());

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

            if (EnableLogging)
                Logger.LogDebug(sql + paramList.ToLogString());

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

            if (EnableLogging)
                Logger.LogDebug(sql);

            DbConnection.Execute(sql);
            return tempTableName;
        }

        /// <summary>
        /// 删除表
        /// </summary>
        /// <param name="tableName"></param>
        public void DropTable(string tableName)
        {
            var sql = SqlBuilder.BuildDropTableSql(tableName);

            if (EnableLogging)
                Logger.LogDebug(sql);

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
            => Operator.BulkCopy(DbConnection, _trans, dataTable, tableName);
    }
}
