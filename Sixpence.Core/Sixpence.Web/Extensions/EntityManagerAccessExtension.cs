using Sixpence.Web.Auth;
using Sixpence.Common;
using Sixpence.Common.Utils;
using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Sixpence.Web.Cache;
using Sixpence.EntityFramework;

namespace Sixpence.Web.Extensions
{
    /// <summary>
    /// PersistBroker 权限扩展
    /// </summary>
    public static class EntityManagerAccessExtension
    {
        /// <summary>
        /// 权限创建
        /// </summary>
        /// <param name="broker"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string FilteredCreate(this IEntityManager manager, BaseEntity entity)
        {
            var sysEntity = EntityCache.GetEntity(entity.EntityMap.Table);
            if (!AuthAccess.CheckWriteAccess(sysEntity.PrimaryColumn.Value?.ToString()))
            {
                throw new InvalidCredentialException($"用户没有实体[{sysEntity.Name}]的创建权限");
            }
            if (string.IsNullOrEmpty(entity.PrimaryColumn.Value?.ToString()))
            {
                EntityCommon.SetDbColumnValue(entity, entity.PrimaryColumn.Name, entity.NewId());
            }
            return manager.Create(entity);
        }

        /// <summary>
        /// 权限更新
        /// </summary>
        /// <param name="broker"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int FiltededUpdate(this IEntityManager manager, BaseEntity entity)
        {
            var sysEntity = EntityCache.GetEntity(entity.EntityMap.Table);
            if (!AuthAccess.CheckWriteAccess(sysEntity.PrimaryColumn.Value?.ToString()))
            {
                throw new InvalidCredentialException($"用户没有实体[{sysEntity.Name}]的更新权限");
            }
            return manager.Update(entity);
        }

        /// <summary>
        /// 创建或更新历史记录
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string FilteredCreateOrUpdateData(this IEntityManager manager, BaseEntity entity)
        {
            var id = entity.PrimaryColumn.Value?.ToString();
            var isExist = manager.QueryCount($"select count(1) from {entity.EntityMap.FullQualifiedName} where {entity.PrimaryColumn.Name?.ToString()} = @id", new Dictionary<string, object>() { { "@id", entity.PrimaryColumn.Value?.ToString() } }) > 0;
            if (isExist)
            {
                manager.Update(entity);
            }
            else
            {
                id = manager.Create(entity);
            }
            return id;
        }

        /// <summary>
        /// 权限查询
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="broker"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T FilteredQueryFirst<T>(this IEntityManager manager, string id) where T : BaseEntity, new()
        {
            var sysEntity = EntityCache.GetEntity(new T().EntityMap.Table);
            if (!AuthAccess.CheckReadAccess(sysEntity.PrimaryColumn.Value?.ToString()))
            {
                throw new InvalidCredentialException($"用户没有实体[{sysEntity.Name}]的查询权限");
            }
            return manager.QueryFirst<T>(id);
        }

        /// <summary>
        /// 权限差选
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="broker"></param>
        /// <param name="sql"></param>
        /// <param name="paramList"></param>
        /// <returns></returns>
        public static IEnumerable<T> FilteredQuery<T>(this IEntityManager manager, string sql, Dictionary<string, object> paramList = null) where T : BaseEntity, new()
        {
            var sysEntity = EntityCache.GetEntity(new T().EntityMap.Table);
            if (!AuthAccess.CheckReadAccess(sysEntity.PrimaryColumn.Value?.ToString()))
            {
                throw new InvalidCredentialException($"用户没有实体[{sysEntity.Name}]的查询权限");
            }
            return manager.Query<T>(sql, paramList);
        }

        /// <summary>
        /// 权限删除
        /// </summary>
        /// <param name="broker"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static int FilteredDelete(this IEntityManager manager, BaseEntity entity)
        {
            var sysEntity = EntityCache.GetEntity(entity.EntityMap.Table);
            if (!AuthAccess.CheckDeleteAccess(sysEntity.PrimaryColumn.Value?.ToString()))
            {
                throw new InvalidCredentialException($"用户没有实体[{sysEntity.Name}]的删除权限");
            }
            return manager.Delete(entity);
        }

        /// <summary>
        /// 权限删除
        /// </summary>
        /// <param name="broker"></param>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static int FilteredDelete(this IEntityManager manager, string entityName, string id)
        {
            var sysEntity = EntityCache.GetEntity(entityName);
            if (!AuthAccess.CheckDeleteAccess(sysEntity.PrimaryColumn.Value?.ToString()))
            {
                throw new InvalidCredentialException($"用户没有实体[{sysEntity.Name}]的删除权限");
            }
            return manager.Delete(entityName, id);
        }


        /// <summary>
        /// 删除历史记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        public static void FilteredDelete(this IEntityManager manager, string entityName, IEnumerable<string> ids)
        {
            manager.ExecuteTransaction(() =>
            {
                ids.Each(id =>
                {
                    manager.FilteredDelete(entityName, id);
                });
            });
        }
    }
}
