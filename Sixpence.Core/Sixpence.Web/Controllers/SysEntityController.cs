using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sixpence.Common;
using Sixpence.Web.Entity;
using Sixpence.Web.Module.SysAttrs;
using Sixpence.Web.Service;
using Sixpence.Web.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;

namespace Sixpence.Web.Controllers
{
    public class SysEntityController : EntityBaseController<SysEntity, SysEntityService>
    {
        public SysEntityController(SysEntityService service) : base(service)
        {
        }

        /// <summary>
        /// 根据实体 id 查询字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("attrs")]
        public IList<SysAttrs> GetEntityAttrs(string id)
        {
            return _service.GetEntityAttrs(id);
        }

        [HttpGet("export")]
        public IActionResult Export(string id)
        {
            var dataTable = _service.GetEntityAllData(id);

            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                return NotFound("无数据可导出");
            }

            var bytes = Encoding.UTF8.GetBytes(dataTable.ToCSV());
            var fileName = $"Entity_{id}_{DateTime.Now:yyyyMMddHHmmss}.csv";
            return File(bytes, "text/csv", fileName);
        }
    }
}