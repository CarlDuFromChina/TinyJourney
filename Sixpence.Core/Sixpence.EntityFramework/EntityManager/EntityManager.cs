using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.IO;
using Sixpence.EntityFramework.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text;

namespace Sixpence.EntityFramework
{
    /// <summary>
    /// 实体管理
    /// </summary>
    public class EntityManager : IEntityManager, IDisposable
    {
        private DbClient _dbClient;
        private readonly bool EnableLogging = AppBuilderExtensions.BuilderOptions.EnableLogging;

        private readonly ILogger<EntityManager>? _logger;
        private readonly Lazy<IEnumerable<IEntityManagerBeforeCreateOrUpdate>> _entityManagerBeforeCreateOrUpdates;
        private readonly Lazy<IEnumerable<IEntityManagerPlugin>> _entityManagerPlugins;
        private readonly Lazy<IEnumerable<IEntity>> _entities;
        private readonly ILoggerFactory _loggerFactory;

        public IDbDriver Driver => _dbClient.Driver;
        public DbClient DbClient => _dbClient;

        public EntityManager(IServiceProvider provider)
        {
            _loggerFactory = provider.GetService<ILoggerFactory>() ?? throw new InvalidOperationException("ILoggerFactory service not found.");
            _logger = _loggerFactory.CreateLogger<EntityManager>();

            _dbClient = new DbClient(new DbClientOptions() { LoggerFactory = _loggerFactory });

            // 将集合包裹在 Lazy 中，懒加载依赖项
            _entityManagerBeforeCreateOrUpdates = new Lazy<IEnumerable<IEntityManagerBeforeCreateOrUpdate>>(
                () => provider.GetServices<IEntityManagerBeforeCreateOrUpdate>());

            _entityManagerPlugins = new Lazy<IEnumerable<IEntityManagerPlugin>>(
                () => provider.GetServices<IEntityManagerPlugin>());

            _entities = new Lazy<IEnumerable<IEntity>>(
                () => provider.GetServices<IEntity>());
        }

        #region CRUD
        /// <summary>
        /// 创建实体记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string Create(BaseEntity entity, bool usePlugin = true)
        {
            return this.ExecuteTransaction(() =>
            {
                #region 创建前 Plugin
                var context = new EntityManagerPluginContext() { Entity = entity, EntityManager = this, Action = EntityAction.PreCreate, EntityName = entity.EntityMap.Table };
                _entityManagerBeforeCreateOrUpdates?.Value?.Each(item => item.Execute(context));
                if (usePlugin)
                {
                    _entityManagerPlugins
                        ?.Value
                        ?.Where(item => EntityCommon.MatchEntityManagerPlugin(item.GetType().Name, entity.EntityMap.Table))
                        ?.Each(item => item.Execute(context));
                }
                #endregion

                var sql = "INSERT INTO {0}({1}) Values({2})";
                var attrs = new List<string>();
                var values = new List<object>();
                var paramList = new Dictionary<string, object>();
                foreach (var attr in EntityCommon.GetDbColumns(entity))
                {
                    var attrName = attr.Key; // 列名
                    var keyValue = Driver.SqlBuilder.HandleParameter($"{Driver.SqlBuilder.ParameterPrefix}{attrName}", attr.Value); // 值
                    attrs.Add($@"""{attrName}""");
                    values.Add(keyValue.name);
                    paramList.Add(keyValue.name, keyValue.value);
                }
                sql = string.Format(sql, $"{entity.EntityMap.FullQualifiedName}", string.Join(",", attrs), string.Join(",", values));
                this.Execute(sql, paramList);

                #region 创建后 Plugin
                if (usePlugin)
                {
                    context.Action = EntityAction.PostCreate;
                    _entityManagerPlugins?.Value
                        ?.Where(item => EntityCommon.MatchEntityManagerPlugin(item.GetType().Name, entity.EntityMap.Table))
                        ?.Each(item => item.Execute(context));
                }
                #endregion

                return entity.PrimaryColumn.Value.ToString() ?? "";
            });
        }

        /// <summary>
        /// 删除实体记录
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(string tableName, string id)
        {
            var entity = _entities.Value.FirstOrDefault(item => item.EntityMap.Table == tableName) as BaseEntity;
            AssertUtil.IsNull(entity, $"未找到实体：{tableName}");
            var sql = $"SELECT * FROM {tableName} WHERE {entity.PrimaryColumn.DbPropertyMap.Name} = {Driver.SqlBuilder.ParameterPrefix}id";
            var dataList = DbClient.Query(sql, new { id });

            if (dataList.Rows.Count == 0) return 0;

            var attributes = dataList.Rows[0].ToDictionary(dataList.Columns);
            attributes.Each(item => EntityCommon.SetDbColumnValue(entity, item.Key, item.Value.Equals(DBNull.Value) ? null : item.Value));

            var plugins = _entityManagerPlugins
                ?.Value
                ?.Where(item => EntityCommon.MatchEntityManagerPlugin(item.GetType().Name, entity.EntityMap.Table));

            plugins?.Each(item => item.Execute(new EntityManagerPluginContext() { EntityManager = this, Entity = entity, EntityName = tableName, Action = EntityAction.PreDelete }));

            sql = $"DELETE FROM {tableName} WHERE {entity.PrimaryColumn.DbPropertyMap.Name} = {Driver.SqlBuilder.ParameterPrefix}id";
            int result = this.Execute(sql, new { id });

            plugins?.Each(item => item.Execute(new EntityManagerPluginContext() { EntityManager = this, Entity = entity, EntityName = tableName, Action = EntityAction.PostDelete }));
            return result;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Delete(BaseEntity entity)
        {
            return this.ExecuteTransaction(() =>
            {
                var plugins = _entityManagerPlugins?.Value
                    ?.Where(item => EntityCommon.MatchEntityManagerPlugin(item.GetType().Name, entity.EntityMap.Table));

                plugins?.Each(item => item.Execute(new EntityManagerPluginContext() { EntityManager = this, Entity = entity, EntityName = entity.EntityMap.Table, Action = EntityAction.PreDelete }));

                var sql = $"DELETE FROM {entity.EntityMap.Table} WHERE {entity.PrimaryColumn.DbPropertyMap.Name} = {Driver.SqlBuilder.ParameterPrefix}id";
                int result = this.Execute(sql, new { id = entity.PrimaryColumn?.Value });

                plugins?.Each(item => item.Execute(new EntityManagerPluginContext() { EntityManager = this, Entity = entity, EntityName = entity.EntityMap.Table, Action = EntityAction.PostDelete }));
                return result;
            });
        }

        /// <summary>
        /// 批量删除实体记录
        /// </summary>
        /// <param name="objArray"></param>
        /// <returns></returns>
        public int Delete(BaseEntity[] objArray)
        {
            if (objArray == null || objArray.Length == 0) return 0;

            return objArray.Sum(Delete);
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="where"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public int DeleteByWhere(string entityName, string where, Dictionary<string, object> paramList = null)
        {
            var sql = "DELETE FROM {0} WHERE 1=1 {1}";
            sql = string.Format(sql, entityName, string.IsNullOrEmpty(where) ? "" : $" AND {where}");
            int result = this.Execute(sql, paramList);
            return result;
        }

        public int Delete<TEntity>(string id) where TEntity : BaseEntity, new()
        {
            var t = new TEntity();
            var tableName = t.EntityMap.FullQualifiedName;
            var primaryKeyName = t.PrimaryColumn.DbPropertyMap.Name;
            var sql = $"DELETE FROM {tableName} WHERE {primaryKeyName} = {Driver.SqlBuilder.ParameterPrefix}id";
            return this.Execute(sql, new { id });
        }

        public int Delete<TEntity>(object param) where TEntity : BaseEntity, new()
        {
            var t = new TEntity();
            var tableName = t.EntityMap.FullQualifiedName;
            var sql = $"DELETE FROM {tableName} WHERE  1 = 1";
            var keyValues = param.ToDictionary();
            foreach (var item in keyValues)
            {
                sql += $" AND {item.Key} = {Driver.SqlBuilder.ParameterPrefix}{item.Key}";
            }
            return this.Execute(sql, param);
        }

        /// <summary>
        /// 保存实体记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public string Save(BaseEntity entity)
        {
            var sql = $@"
SELECT * FROM {entity.EntityMap.FullQualifiedName}
WHERE {entity.PrimaryColumn.DbPropertyMap.Name} = {Driver.SqlBuilder.ParameterPrefix}id;
";
            var dataList = this.Query(sql, new { id = entity.PrimaryColumn?.Value });

            if (dataList != null && dataList.Rows.Count > 0)
                Update(entity);
            else
                Create(entity);

            return entity.PrimaryColumn?.Value?.ToString() ?? "";
        }

        /// <summary>
        /// 更新实体记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int Update(BaseEntity entity)
        {
            return this.ExecuteTransaction(() =>
            {
                var tableName = entity.EntityMap.FullQualifiedName;
                var primaryKeyName = entity.PrimaryColumn.DbPropertyMap.Name;
                var prefix = Driver.SqlBuilder.ParameterPrefix;

                #region 更新前 Plugin
                var context = new EntityManagerPluginContext() { EntityManager = this, Entity = entity, EntityName = tableName, Action = EntityAction.PreUpdate };
                _entityManagerBeforeCreateOrUpdates?.Value?.Each(item => item.Execute(context));

                _entityManagerPlugins
                    ?.Value
                    ?.Where(item => EntityCommon.MatchEntityManagerPlugin(item.GetType().Name, entity.EntityMap.Table))
                    ?.Each(item => item.Execute(context));
                #endregion

                var paramList = new Dictionary<string, object>();
                var setValueSql = "";

                #region 处理字段SQL
                var attributes = new List<string>();
                var dbColumns = EntityCommon.GetDbColumns(entity);
                foreach (var attr in dbColumns)
                {
                    var parameter = Driver.SqlBuilder.HandleParameter($"{prefix}{attr.Key}", attr.Value);
                    paramList.Add(parameter.name, parameter.value);
                    if (attr.Key != entity.PrimaryColumn.Name)
                    {
                        attributes.Add($@"{attr.Key} = {parameter.name}"); // user_name = :user_name
                    }
                }
                setValueSql = string.Join(',', attributes);

                var sql = $@"UPDATE {tableName} SET {setValueSql} WHERE {primaryKeyName} = {prefix}{primaryKeyName};";
                #endregion

                var result = this.Execute(sql, paramList);

                #region 更新后 Plugin
                context.Action = EntityAction.PostUpdate;
                _entityManagerPlugins
                    ?.Value
                    ?.Where(item => EntityCommon.MatchEntityManagerPlugin(item.GetType().Name, entity.EntityMap.Table))
                    ?.Each(item => item.Execute(context));
                #endregion
                return result;
            });
        }
        #endregion

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            (this.DbClient as IDisposable).Dispose();
        }

        #region Transcation
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="func"></param>
        public void ExecuteTransaction(Action func)
        {
            try
            {
                DbClient.Open();
                DbClient.BeginTransaction();

                func?.Invoke();

                DbClient.CommitTransaction();
            }
            catch (Exception ex)
            {
                DbClient.Rollback();
                throw ex;
            }
            finally
            {
                DbClient.Close();
            }
        }

        /// <summary>
        /// 执行事务返回结果
        /// </summary>
        /// <param name="func"></param>
        public T ExecuteTransaction<T>(Func<T> func)
        {

            try
            {
                DbClient.Open();
                DbClient.BeginTransaction();

                var t = default(T);

                if (func != null)
                {
                    t = func();
                }

                DbClient.CommitTransaction();

                return t;
            }
            catch (Exception ex)
            {
                DbClient.Rollback();
                throw ex;
            }
            finally
            {
                DbClient.Close();
            }
        }
        #endregion

        #region Query
        /// <summary>
        /// 根据id查询记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public T QueryFirst<T>(string id) where T : BaseEntity, new()
        {
            var t = new T();
            var tableName = t.EntityMap.FullQualifiedName;
            var primaryKeyName = t.PrimaryColumn.DbPropertyMap.Name;
            var sql = $"SELECT * FROM {tableName} WHERE {primaryKeyName} = {Driver.SqlBuilder.ParameterPrefix}id";
            return QueryFirst<T>(sql, new { id });
        }

        /// <summary>
        /// 执行SQL，返回查询记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public T QueryFirst<T>(string sql, object? param = null) where T : BaseEntity, new()
        {
            return DbClient.QueryFirst<T>(sql, param);
        }

        /// <summary>
        /// 根据筛选条件查询返回记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public T QueryFirst<T>(object? param = null) where T : BaseEntity, new()
        {
            var entityMap = new T().EntityMap;
            var sql = new StringBuilder($"SELECT * FROM {entityMap.FullQualifiedName} WHERE 1 = 1");
            param
                .ToDictionary()
                .Each(item => sql.Append($" AND {item.Key} = {Driver.SqlBuilder.ParameterPrefix}{item.Key}"));

            return DbClient.QueryFirst<T>(sql.ToString(), param);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public DataTable Query(string sql, object? param = null)
        {
            return DbClient.Query(sql, param);
        }

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public int QueryCount(string sql, object? param = null)
        {
            return ConvertUtil.ConToInt(this.ExecuteScalar(sql, param));
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object? param = null)
        {
            return DbClient.Query<T>(sql, param);
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(object? param = null) where T : BaseEntity, new()
        {
            var entityMap = new T().EntityMap;
            var sql = new StringBuilder($"select * from {entityMap.FullQualifiedName} where 1 = 1");
            param
                .ToDictionary()
                .Each(item => sql.Append($" and {item.Key} = {Driver.SqlBuilder.ParameterPrefix}{item.Key}"));

            return DbClient.Query<T>(sql.ToString(), param);
        }

        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object param, string orderby, int pageSize, int pageIndex) where T : BaseEntity, new()
        {
            if (!string.IsNullOrEmpty(orderby))
            {
                if (!orderby.Contains("order by", StringComparison.OrdinalIgnoreCase))
                    sql += $" ORDER BY {orderby}";
                else
                    sql += $" {orderby}";
            }

            sql += $" {DbClient.Driver.SqlBuilder.BuildPageSql(pageIndex, pageSize)}";
            return Query<T>(sql, param);
        }

        /// <summary>
        /// 查询多条记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <param name="orderby"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(string sql, object param, string orderby, int pageSize, int pageIndex, out int recordCount) where T : BaseEntity, new()
        {
            var recordCountSql = $"SELECT COUNT(1) FROM ({sql}) AS table1";
            recordCount = Convert.ToInt32(this.ExecuteScalar(recordCountSql, param));
            var data = Query<T>(sql, param, orderby, pageSize, pageIndex);
            return data;
        }

        /// <summary>
        /// 根据 id 批量查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IEnumerable<T> Query<T>(IList<string> ids) where T : BaseEntity, new()
        {
            var paramList = new Dictionary<string, object>();
            var tableName = new T().EntityMap.FullQualifiedName;
            var primaryKey = new T().PrimaryColumn?.DbPropertyMap.Name;
            var inClause = string.Join(",", ids.Select((id, index) => $"{Driver.SqlBuilder.ParameterPrefix}id" + index));
            var sql = $"SELECT * FROM {tableName} WHERE {primaryKey} IN ({inClause})";
            var count = 0;
            ids.Each((id) => paramList.Add($"{Driver.SqlBuilder.ParameterPrefix}id{count++}", id));
            return Query<T>(sql, paramList);
        }
        #endregion

        #region Execute
        /// <summary>
        /// 执行Sql
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        public int Execute(string sql, object? param = null)
        {
            return DbClient.Execute(sql, param);
        }

        /// <summary>
        /// 执行Sql返回第一行第一列记录
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, object? param = null)
        {
            return DbClient.ExecuteScalar(sql, param);
        }


        #endregion

        #region Bulk CRUD
        /// <summary>
        /// 批量创建
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dataList"></param>
        public void BulkCreate<TEntity>(List<TEntity> dataList) where TEntity : BaseEntity, new()
        {
            if (dataList.IsEmpty()) return;

            var t = new TEntity();
            var tableName = t.EntityMap.FullQualifiedName;
            var primaryKey = t.PrimaryColumn?.DbPropertyMap.Name;
            var dt = Query($"select * from {tableName} WHERE 1 <> 1");
            dataList.ForEach(entity =>
            {
                var context = new EntityManagerPluginContext() { EntityManager = this, Entity = entity, EntityName = tableName, Action = EntityAction.PreCreate };
                _entityManagerBeforeCreateOrUpdates?.Value?.Each(item => item.Execute(context));
            });
            var data = EntityCommon.ParseToDataTable(dataList, dt.Columns);
            BulkCreate(tableName, primaryKey, data);
        }

        /// <summary>
        /// 批量创建
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="primaryKeyName">主键名</param>
        /// <param name="dataTable">数据</param>
        public void BulkCreate(string tableName, string primaryKeyName, DataTable dataTable)
        {
            if (dataTable.IsEmpty())
            {
                return;
            }

            ExecuteTransaction(() =>
            {
                // 1. 创建临时表
                var tempName = DbClient.CreateTemporaryTable(tableName);

                // 2. 拷贝数据到临时表
                DbClient.BulkCopy(dataTable, tempName);

                // 3. 将临时表数据插入到目标表中
                DbClient.Execute(string.Format("INSERT INTO {0} SELECT * FROM {1} WHERE NOT EXISTS(SELECT 1 FROM {0} WHERE {0}.{2} = {1}.{2})", tableName, tempName, primaryKeyName));

                // 4. 删除临时表
                DbClient.DropTable(tempName);
            });
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dataList"></param>
        public void BulkUpdate<TEntity>(List<TEntity> dataList) where TEntity : BaseEntity, new()
        {
            if (dataList.IsEmpty()) return;

            var t = new TEntity();
            var mainKeyName = t.PrimaryColumn?.DbPropertyMap.Name; // 主键
            var tableName = t.EntityMap.FullQualifiedName; // 表名
            var dt = DbClient.Query($"SELECT * FROM {tableName} WHERE 1 <> 1");
            dataList.ForEach(entity =>
            {
                var context = new EntityManagerPluginContext() { EntityManager = this, Entity = entity, EntityName = tableName, Action = EntityAction.PreUpdate };
                _entityManagerBeforeCreateOrUpdates?.Value?.Each(item => item.Execute(context));
            });
            BulkUpdate(tableName, mainKeyName, EntityCommon.ParseToDataTable(dataList, dt.Columns));
        }

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="dataTable"></param>
        public void BulkUpdate(string tableName, string primaryKeyName, DataTable dataTable)
        {
            if (dataTable.IsEmpty())
            {
                return;
            }

            ExecuteTransaction(() =>
            {
                // 1. 创建临时表
                var tempTableName = DbClient.CreateTemporaryTable(tableName);

                // 2. 拷贝数据到临时表
                DbClient.BulkCopy(dataTable, tempTableName);

                // 3. 获取更新字段
                var updateFieldList = new List<string>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    // 主键去除
                    if (!column.ColumnName.Equals(primaryKeyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        updateFieldList.Add(column.ColumnName);
                    }
                }

                // 4. 拼接Set语句
                var updateFieldSql = updateFieldList.Select(item => string.Format(" {1} = {0}.{1} ", tempTableName, item)).Aggregate((a, b) => a + " , " + b);

                // 5. 更新
                DbClient.Execute($@"
UPDATE {tableName}
SET {updateFieldSql} FROM {tempTableName}
WHERE {tableName}.{primaryKeyName} = {tempTableName}.{primaryKeyName}
AND {tempTableName}.{primaryKeyName} IS NOT NULL
");

                // 6. 删除临时表
                DbClient.DropTable(tempTableName);
            });
        }

        /// <summary>
        /// 批量创建或更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dataList"></param>
        /// <param name="updateFieldList"></param>
        public void BulkCreateOrUpdate<TEntity>(List<TEntity> dataList, List<string> updateFieldList = null) where TEntity : BaseEntity, new()
        {
            if (dataList.IsEmpty()) return;

            var primaryKeyName = new TEntity().PrimaryColumn?.DbPropertyMap.Name; // 主键
            var tableName = new TEntity().EntityMap.FullQualifiedName; // 表名
            var dt = DbClient.Query($"SELECT * FROM {tableName} WHERE 1 <> 1");

            BulkCreateOrUpdate(tableName, primaryKeyName, EntityCommon.ParseToDataTable(dataList, dt.Columns), updateFieldList);
        }

        /// <summary>
        /// 批量创建或更新
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="dataTable"></param>
        /// <param name="updateFieldList"></param>
        public void BulkCreateOrUpdate(string tableName, string primaryKeyName, DataTable dataTable, List<string> updateFieldList = null)
        {
            if (dataTable.IsEmpty()) return;

            // 1. 创建临时表
            var tempTableName = DbClient.CreateTemporaryTable(tableName);

            // 2. 拷贝数据到临时表
            DbClient.BulkCopy(dataTable, tempTableName);

            // 3. 获取更新字段
            if (updateFieldList.IsEmpty())
            {
                updateFieldList = new List<string>();
                foreach (DataColumn column in dataTable.Columns)
                {
                    // 主键去除
                    if (!column.ColumnName.Equals(primaryKeyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        updateFieldList.Add(column.ColumnName);
                    }
                }
            }

            // 4. 拼接Set语句
            var updateFieldSql = updateFieldList.Select(item => string.Format(" {1} = {0}.{1} ", tempTableName, item)).Aggregate((a, b) => a + " , " + b);

            // 5. 更新
            DbClient.Execute($@"
UPDATE {tableName}
SET {updateFieldSql} FROM {tempTableName}
WHERE {tableName}.{primaryKeyName} = {tempTableName}.{primaryKeyName}
AND {tempTableName}.{primaryKeyName} IS NOT NULL
");
            // 6. 新增
            DbClient.Execute($@"
INSERT INTO {tableName}
SELECT * FROM {tempTableName}
WHERE NOT EXISTS(SELECT 1 FROM {tableName} WHERE {tableName}.{primaryKeyName} = {tempTableName}.{primaryKeyName})
AND {tempTableName}.{primaryKeyName} IS NOT NULL
");

            // 7. 删除临时表
            DbClient.DropTable(tempTableName);
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void BulkDelete<TEntity>(List<TEntity> dataList) where TEntity : BaseEntity, new()
        {
            if (dataList.IsEmpty())
            {
                return;
            }

            var t = new TEntity();
            var sqlBuilder = Driver.SqlBuilder;
            var tableName = t.EntityMap.FullQualifiedName;
            var primaryKeyName = t.PrimaryColumn.DbPropertyMap.Name;
            var idList = dataList.Select(item => item.PrimaryColumn.Value.ToString()).ToArray();
            var inSqlResult = sqlBuilder.BuildInClauseSql("ids", 1, idList.Cast<object>().ToList());
            var sql = $"DELETE FROM {tableName} WHERE {primaryKeyName} {inSqlResult.sql}";
            DbClient.Execute(sql, inSqlResult.param);
        }
        #endregion
    }
}
