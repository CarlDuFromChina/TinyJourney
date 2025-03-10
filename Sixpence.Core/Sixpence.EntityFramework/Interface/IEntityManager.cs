﻿using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    public interface IEntityManager : IDisposable
    {
        DbClient DbClient { get; }
        IDbDriver Driver { get; }

        #region CRUD
        /// <summary>
        /// 保存实体，系统根据ID自动判断更新还是新建
        /// </summary>
        /// <param name="entity">要保存到数据库的实体对象实例</param>
        /// <returns>穿件或者更新的记录的Id</returns>
        string Save(BaseEntity entity);

        /// <summary>
        /// 创建实体记录
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        /// <param name="usePlugin">是否使用Plugin</param>
        /// <returns></returns>
        string Create(BaseEntity entity, bool usePlugin = true);

        /// <summary>
        /// 更新实体记录
        /// </summary>
        /// <param name="entity">实体对象实例</param>
        /// <returns>影响的书库的行数</returns>
        int Update(BaseEntity entity);

        /// <summary>
        /// 删除数据的实体记录
        /// </summary>
        /// <param name="tableName">表的名字</param>
        /// <param name="id">记录的主键Id</param>
        /// <returns>影响的数据库行数</returns>
        int Delete(string tableName, string id);

        /// <summary>
        /// 删除数据库的实体记录
        /// </summary>
        /// <param name="obj">实体对象</param>
        /// <returns>影响的数据库的行数</returns>
        int Delete(BaseEntity obj);

        /// <summary>
        /// 删除实体记录
        /// </summary>
        /// <param name="objArray">实体数组</param>
        /// <returns>影响的记录行数</returns>
        int Delete(BaseEntity[] objArray);

        /// <summary>
        /// 根据条件删除数据库的实体记录
        /// </summary>
        int DeleteByWhere(string typeName, string where, Dictionary<string, object> paramList = null);

        /// <summary>
        /// 删除数据库的实体记录
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        int Delete<TEntity>(string id) where TEntity : BaseEntity, new();

        /// <summary>
        /// 删除数据库的实体记录
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        int Delete<TEntity>(object param) where TEntity : BaseEntity, new();
        #endregion

        #region Transcation
        /// <summary>
        /// 执行事务
        /// </summary>
        /// <param name="func"></param>
        void ExecuteTransaction(Action func);

        /// <summary>
        /// 执行事务返回结果
        /// </summary>
        /// <param name="func"></param>
        T ExecuteTransaction<T>(Func<T> func);
        #endregion

        #region Query
        /// <summary>
        /// 根据 实体T 和 实体Id 获取实体对象实例
        /// </summary>
        T QueryFirst<T>(string id) where T : BaseEntity, new();

        /// <summary>
        /// 根据查询条件查询实体对象
        /// </summary>
        T QueryFirst<T>(string sql, object? param = null) where T : BaseEntity, new();

        /// <summary>
        /// 根据查询条件查询实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="param"></param>
        /// <returns></returns>
        T QueryFirst<T>(object param) where T : BaseEntity, new();

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        DataTable Query(string sql, object? param = null);

        /// <summary>
        /// 查询数量
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        int QueryCount(string sql, object? param = null);

        /// <summary>
        /// 根据SQL查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(string sql, object? param = null);

        /// <summary>
        /// 根据SQL查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(object? param = null) where T : BaseEntity, new();

        /// <summary>
        /// 根据查询条件查询实体的对象列表 (分页查询）
        /// </summary>
        IEnumerable<T> Query<T>(string sql, object param, string orderby, int pageSize, int pageIndex) where T : BaseEntity, new();

        /// <summary>
        /// 根据查询条件查询实体的对象列表 (分页查询）
        /// </summary>
        IEnumerable<T> Query<T>(string sql, object param, string orderby, int pageSize, int pageIndex, out int recordCount) where T : BaseEntity, new();

        /// <summary>
        /// 根据 id 批量查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        IEnumerable<T> Query<T>(IList<string> ids) where T : BaseEntity, new();
        #endregion

        #region Execute
        /// <summary>
        /// 执行Sql
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        int Execute(string sql, object? param = null);

        /// <summary>
        /// 执行Sql返回第一行第一列记录
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        object ExecuteScalar(string sql, object? param = null);
        #endregion

        #region Bulk CURD
        /// <summary>
        /// 批量创建
        /// </summary>
        /// <param name="dataList"></param>
        public void BulkCreate<TEntity>(List<TEntity> dataList) where TEntity : BaseEntity, new();

        /// <summary>
        /// 批量创建
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="dataTable"></param>
        public void BulkCreate(string tableName, string primaryKeyName, DataTable dataTable);

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="dataList"></param>
        public void BulkUpdate<TEntity>(List<TEntity> dataList) where TEntity : BaseEntity, new();

        /// <summary>
        /// 批量更新
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="primaryKeyName"></param>
        /// <param name="dataTable"></param>
        public void BulkUpdate(string tableName, string primaryKeyName, DataTable dataTable);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="dataList"></param>
        public void BulkDelete<TEntity>(List<TEntity> dataList) where TEntity : BaseEntity, new();

        /// <summary>
        /// 批量创建或更新
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="dataList"></param>
        public void BulkCreateOrUpdate<TEntity>(List<TEntity> dataList, List<string> updateFieldList = null) where TEntity : BaseEntity, new();

        /// <summary>
        /// 批量创建或更新
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="dt"></param>
        /// <param name=""></param>
        /// <param name="updateFieldList"></param>
        public void BulkCreateOrUpdate(string tableName, string primaryKeyName, DataTable dataTable, List<string> updateFieldList = null);
        #endregion
    }
}
