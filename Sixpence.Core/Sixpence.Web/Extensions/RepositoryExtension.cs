﻿using Sixpence.Common;
using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Entity;
using Sixpence.EntityFramework.Repository;
using Sixpence.Web.Model;
using Sixpence.Web.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sixpence.Web.Extensions
{
    public static class RepositoryExtension
    {
        /// <summary>
        ///  获取所有实体记录
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<E> GetAllEntity<E>(this IRepository<E> repository)
            where E : BaseEntity, new()
        {
            var sql = $"SELECT *  FROM {new E().EntityMap.FullQualifiedName}";
            var data = repository.Manager.Query<E>(sql);
            return data;
        }

        /// <summary>
        /// 根据搜索条件分页查询
        /// </summary>
        /// <param name="view"></param>
        /// <param name="searchList"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static IEnumerable<E> GetDataList<E>(this IRepository<E> repository, EntityView view, IList<SearchCondition> searchList, int pageSize, int pageIndex, out int recordCount, string searchValue = "")
            where E : BaseEntity, new()
        {
            var sql = view.Sql;
            var paramList = new Dictionary<string, object>();

            GetSql<E>(ref sql, searchList, ref paramList, view, searchValue);

            var recordCountSql = $"SELECT COUNT(1) FROM ({sql}) AS table1";
            recordCount = repository.Manager.QueryCount(recordCountSql, paramList);
            sql += repository.Manager.Driver.SqlBuilder.BuildPageSql(pageIndex, pageSize);
            var data = repository.Manager.FilteredQuery<E>(sql, paramList);
            return data;
        }

        /// <summary>
        /// 根据搜索条件查询
        /// </summary>
        /// <param name="view">视图</param>
        /// <param name="searchList">搜索条件</param>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public static IEnumerable<E> GetDataList<E>(this IRepository<E> repository, EntityView view, IList<SearchCondition> searchList, string searchValue = "")
            where E : BaseEntity, new()
        {
            var sql = view.Sql;
            var paramList = new Dictionary<string, object>();

            GetSql<E>(ref sql, searchList, ref paramList, view, searchValue);

            var data = repository.Manager.FilteredQuery<E>(sql, paramList);
            return data;
        }

        #region 权限CRUD
        /// <summary>
        /// 创建实体记录
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string FilteredCreate<E>(this IRepository<E> repository, E entity)
            where E : BaseEntity, new()
        {
            return repository.Manager.FilteredCreate(entity);
        }

        /// <summary>
        /// 创建或更新历史记录
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string FilteredCreateOrUpdateData<E>(this IRepository<E> repository, E entity)
            where E : BaseEntity, new()
        {
            var id = entity.PrimaryColumn.Value?.ToString();

            if (!string.IsNullOrEmpty(id) && repository.Manager.QueryFirst<E>(id) != null)
            {
                repository.Update(entity);
            }
            else
            {
                if (string.IsNullOrEmpty(id))
                {
                    EntityCommon.SetDbColumnValue(entity, entity.PrimaryColumn.Name, entity.NewId());
                }
                id = repository.Create(entity);
            }

            return id;
        }

        /// <summary>
        /// 删除历史记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        public static void FilteredDelete<E>(this IRepository<E> repository, IEnumerable<string> ids)
            where E : BaseEntity, new()
        {
            repository.Manager.ExecuteTransaction(() =>
            {
                ids.Each(id =>
                {
                    var data = repository.Manager.FilteredQueryFirst<E>(id);
                    repository.Manager.FilteredDelete(new E().EntityMap.Table, id);
                });
            });
        }


        /// <summary>
        /// 获取实体记录
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public static IEnumerable<E> FilteredQuery<E>(this IRepository<E> repository)
            where E : BaseEntity, new()
        {
            return repository.Manager.FilteredQuery<E>($"select * from {new E().EntityMap.FullQualifiedName}");
        }

        /// <summary>
        /// 查询单条记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static E FilteredQueryFirst<E>(this IRepository<E> repository, string id)
            where E : BaseEntity, new()
        {
            return repository.Manager.FilteredQueryFirst<E>(id);
        }

        /// <summary>
        /// 更新实体记录
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="entity"></param>
        public static void FilteredUpdate<E>(this IRepository<E> repository, E entity)
            where E : BaseEntity, new()
        {
            if (string.IsNullOrEmpty(entity?.PrimaryColumn.Value?.ToString()))
            {
                return;
            }

            repository.Manager.FiltededUpdate(entity);
        }
        #endregion

        /// <summary>
        /// 格式化Sql
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="searchList"></param>
        /// <param name="paramList"></param>
        /// <param name="orderBy"></param>
        /// <param name="view"></param>
        private static void GetSql<E>(ref string sql, IList<SearchCondition> searchList, ref Dictionary<string, object> paramList, EntityView view, string searchValue)
            where E : BaseEntity, new()
        {
            var entityName = new E().EntityMap.FullQualifiedName;
            var count = 0;

            var index = sql.IndexOf("where", StringComparison.CurrentCultureIgnoreCase);
            if (index == -1)
            {
                sql += " WHERE 1=1 ";
            }

            if (!string.IsNullOrEmpty(searchValue) && view.CustomFilter != null)
            {
                foreach (var item in view.CustomFilter)
                {
                    sql += $" AND {entityName}.{item} LIKE @params{count}";
                    paramList.Add($"@params{count++}", $"%{searchValue}%");
                }
            }
            if (searchList != null && searchList.Count() > 0)
            {
                foreach (var search in searchList)
                {
                    var searchCondition = ParseSqlUtil.GetSearchCondition(search.Type, "params", search.Value, ref count);
                    sql += $" AND {entityName}.{search.Name} {searchCondition.sql}";
                    foreach (var item in searchCondition.paramsList)
                    {
                        paramList.Add(item.Key, item.Value);
                    }
                }
            }

            if (!string.IsNullOrEmpty(view.OrderBy))
            {
                sql += $" ORDER BY {view.OrderBy}";
            }
        }
    }
}
