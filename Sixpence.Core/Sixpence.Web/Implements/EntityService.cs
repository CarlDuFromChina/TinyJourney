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
    /// <typeparam name="T"></typeparam>
    public abstract class EntityService<T>
        where T : BaseEntity, new()
    {
        public EntityService()
        {
            _logger = AppContext.GetLogger<T>();
            Repository = new Repository<T>(new EntityManager());
        }

        public EntityService(IEntityManager manager)
        {
            _logger = AppContext.GetLogger<T>();
            Repository = new Repository<T>(manager);
        }

        /// <summary>
        /// 实体操作
        /// </summary>
        protected IRepository<T> Repository;

        /// <summary>
        /// 数据库持久化
        /// </summary>
        protected IEntityManager Manager => Repository.Manager;

        protected ILogger _logger;

        #region 实体表单

        /// <summary>
        /// 获取视图
        /// </summary>
        /// <returns></returns>
        public virtual IList<EntityView> GetViewList()
        {
            var sql = $"SELECT * FROM {new T().EntityMap.FullQualifiedName} WHERE 1=1";
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
        public IEnumerable<T> GetAllData()
        {
            return Repository.FindAll();
        }

        /// <summary>
        /// 获取所有实体记录
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<T> GetDataList(IList<SearchCondition> searchList, string viewId = "", string searchValue = "")
        {
            var view = string.IsNullOrEmpty(viewId) ? GetViewList().ToList().FirstOrDefault() : GetViewList().ToList().Find(item => item.ViewId == viewId);
            return Repository.GetDataList(view, searchList);
        }

        /// <summary>
        /// 获取所有实体记录
        /// </summary>
        /// <returns></returns>
        public virtual DataModel<T> GetDataList(IList<SearchCondition> searchList, int pageSize, int pageIndex, string viewId = "", string searchValue = "")
        {
            var view = string.IsNullOrEmpty(viewId) ? GetViewList().ToList().FirstOrDefault() : GetViewList().ToList().Find(item => item.ViewId == viewId);
            var data = Repository.GetDataList(view, searchList, pageSize, pageIndex, out var recordCount, searchValue);
            return new DataModel<T>()
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
        public virtual T GetData(string id)
        {
            var obj = Repository.FilteredQueryFirst(id);
            return obj;
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual string CreateData(T t)
        {
            return Repository.FilteredCreate(t);
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <param name="t"></param>
        public virtual void UpdateData(T t)
        {
            Repository.FilteredUpdate(t);
        }

        /// <summary>
        /// 创建或更新记录
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual string CreateOrUpdateData(T t)
        {
            return Repository.FilteredCreateOrUpdateData(t);
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="ids"></param>
        public virtual void DeleteData(List<string> ids)
        {
            Repository.FilteredDelete(ids);
        }

        /// <summary>
        /// 导出CSV文件
        /// </summary>
        /// <returns></returns>
        public virtual string Export()
        {
            var fileName = $"{new T().EntityMap.Table}.csv";
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
            var user = Manager.QueryFirst<SysUser>(UserIdentityUtil.GetCurrentUserId());
            var param = new
            {
                role_id = user.RoleId,
                object_type = "sys_entity",
                object_id = EntityCache.GetEntity(new T().EntityMap.Table)?.PrimaryColumn.Value?.ToString()
            };
            var data = Manager.QueryFirst<SysRolePrivilege>(param);

            return new EntityPrivilegeResponse()
            {
                read = data.Privilege >= 1,
                create = data.Privilege >= 3,
                delete = data.Privilege >= 7
            };
        }
    }
}
