using Sixpence.Web.Auth;
using Sixpence.Web.Extensions;
using Sixpence.Web.Utils;
using Sixpence.EntityFramework.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sixpence.EntityFramework.Entity;
using Sixpence.Web.Model;
using Sixpence.Web.Cache;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;
using Microsoft.Extensions.Logging;
using Quartz.Logging;

namespace Sixpence.Web
{
    /// <summary>
    /// 实体服务类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityService<TEntity> : BaseService<EntityService<TEntity>>
        where TEntity : BaseEntity, new()
    {
        protected readonly IRepository<TEntity> _repository;

        public EntityService(IEntityManager manager, ILogger<EntityService<TEntity>> logger, IRepository<TEntity> repository) : base(manager, logger)
        {
            _repository = repository;
        }

        #region 实体表单

        /// <summary>
        /// 获取视图
        /// </summary>
        /// <returns></returns>
        public virtual IList<EntityView> GetViewList()
        {
            var sql = $"SELECT * FROM {new TEntity().EntityMap.FullQualifiedName} WHERE 1=1";
            return new List<EntityView>()
            {
                new EntityView()
                {
                    Sql = sql,
                    CustomFilter = new List<string>() { "name" }, // name 是每个实体必须要添加字段
                    OrderBy = "created_at DESC",
                    ViewId = ""
                }
            };
        }

        /// <summary>
        /// 获取所有记录
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TEntity> GetAllData()
        {
            return _repository.FindAll();
        }

        /// <summary>
        /// 获取所有实体记录
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetDataList(IList<SearchCondition> searchList, string viewId = "", string searchValue = "")
        {
            var view = string.IsNullOrEmpty(viewId) ? GetViewList().ToList().FirstOrDefault() : GetViewList().ToList().Find(item => item.ViewId == viewId);
            return _repository.GetDataList(view, searchList);
        }

        /// <summary>
        /// 获取所有实体记录
        /// </summary>
        /// <returns></returns>
        public virtual DataModel<TEntity> GetDataList(IList<SearchCondition> searchList, int pageSize, int pageIndex, string viewId = "", string searchValue = "")
        {
            var view = string.IsNullOrEmpty(viewId) ? GetViewList().ToList().FirstOrDefault() : GetViewList().ToList().Find(item => item.ViewId == viewId);
            var data = _repository.GetDataList(view, searchList, pageSize, pageIndex, out var recordCount, searchValue);
            return new DataModel<TEntity>()
            {
                Data = data.ToList(),
                Count = recordCount
            };
        }

        /// <summary>
        /// 获取实体记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetData(string id)
        {
            var obj = _repository.FilteredQueryFirst(id);
            return obj;
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual string CreateData(TEntity t)
        {
            return _repository.FilteredCreate(t);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="t"></param>
        public virtual void UpdateData(TEntity t)
        {
            _repository.FilteredUpdate(t);
        }

        /// <summary>
        /// 创建或更新记录
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual string CreateOrUpdateData(TEntity t)
        {
            return _repository.FilteredCreateOrUpdateData(t);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="ids"></param>
        public virtual void DeleteData(List<string> ids)
        {
            _repository.FilteredDelete(ids);
        }

        /// <summary>
        /// 导出CSV文件
        /// </summary>
        /// <returns></returns>
        public virtual string Export()
        {
            var fileName = $"{new TEntity().EntityMap.Table}.csv";
            var fullFilePath = Path.Combine(FolderType.Temp.GetPath(), fileName);
            var dataList = GetAllData();
            CsvUtil.Write(dataList, fullFilePath);
            return fullFilePath;
        }
        #endregion

        /// <summary>
        /// 获取用户对实体的权限
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public EntityPrivilegeResponse GetPrivilege()
        {
            var user = _manager.QueryFirst<SysUser>(UserIdentityUtil.GetCurrentUserId());
            var param = new
            {
                role_id = user.RoleId,
                object_type = "sys_entity",
                object_id = EntityCache.GetEntity(_manager, new TEntity().EntityMap.Table)?.PrimaryColumn.Value?.ToString()
            };
            var data = _manager.QueryFirst<SysRolePrivilege>(param);

            return new EntityPrivilegeResponse()
            {
                read = data.Privilege >= 1,
                create = data.Privilege >= 3,
                delete = data.Privilege >= 7
            };
        }
    }
}
