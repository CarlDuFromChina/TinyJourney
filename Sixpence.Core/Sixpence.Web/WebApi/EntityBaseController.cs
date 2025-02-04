﻿using Sixpence.EntityFramework.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixpence.Common.Utils;
using System.IO;
using Sixpence.Web.Model;

namespace Sixpence.Web.WebApi
{
    [Authorize(Policy = "Api")]
    public class EntityBaseController<E, S> : BaseApiController
        where E : BaseEntity, new()
        where S : EntityService<E>, new()
    {
        /// <summary>
        /// 获取视图
        /// </summary>
        /// <returns></returns>
        [HttpGet("views")]
        public virtual IList<EntityView> GetViewList()
        {
            return new S().GetViewList();
        }

        /// <summary>
        /// 分页获取筛选数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="searchList"></param>
        /// <param name="orderBy"></param>
        /// <param name="viewId"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        [HttpGet("search")]
        public virtual DataModel<E> GetViewData(string? pageSize, string? pageIndex, string? searchList, string? viewId, string? searchValue)
        {
            var _searchList = string.IsNullOrEmpty(searchList) ? null : JsonConvert.DeserializeObject<IList<SearchCondition>>(searchList);

            if (string.IsNullOrEmpty(pageSize) || string.IsNullOrEmpty(pageIndex))
            {
                var list = new S().GetDataList(_searchList, viewId, searchValue).ToList();
                return new DataModel<E>()
                {
                    Data = list,
                    Count = list.Count
                };
            }

            var size = ConvertUtil.ConToInt(pageSize);
            var index = ConvertUtil.ConToInt(pageIndex);
            return new S().GetDataList(_searchList, size, index, viewId, searchValue);
        }

        /// <summary>
        /// 获取所有数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual DataModel<E> GetDataList()
        {
            var list = new S().GetAllData().ToList();
            return new DataModel<E>()
            {
                Data = list,
                Count = list.Count
            };
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        public virtual E GetData(string id)
        {
            return new S().GetData(id);
        }

        /// <summary>
        /// 创建数据
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual string CreateData(E entity)
        {
            return new S().CreateData(entity);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="entity"></param>
        [HttpPut]
        public virtual void UpdateData(E entity)
        {
            new S().UpdateData(entity);
        }

        /// <summary>
        /// 创建或更新
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("save")]
        public virtual string CreateOrUpdateData(E entity)
        {
            return new S().CreateOrUpdateData(entity);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="ids"></param>
        [HttpDelete("{id}")]
        public virtual void DeleteData(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var ids = id.Split(",").ToList();
                new S().DeleteData(ids);
            }
        }

        [HttpGet]
        [Route("privilege")]
        public virtual EntityPrivilegeResponse GetPrivilege()
        {
            return new S().GetPrivilege();
        }

        [HttpGet, Route("export/csv")]
        public virtual IActionResult ExportCsv()
        {
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            var fileName = new S().Export();
            return File(FileUtil.GetFileStream(fileName), "application/octet-stream", Path.GetFileName(fileName));
        }
    }
}
