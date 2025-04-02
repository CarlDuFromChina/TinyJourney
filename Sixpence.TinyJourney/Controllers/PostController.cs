using System;
using System.Collections.Generic;
using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sixpence.Common.Utils;
using Sixpence.TinyJourney.Service;
using Sixpence.TinyJourney.Model;
using Sixpence.TinyJourney.Entity;
using Sixpence.Web.Entity;
using Newtonsoft.Json;
using Sixpence.Web.Model;
using System.Text.Json;
using StackExchange.Redis;
using Sixpence.Common.Cache;
using Sixpence.Web.Config;
using System.Linq;

namespace Sixpence.TinyJourney.Controller
{
    public class PostController : EntityBaseController<Post, PostService>
    {
        private readonly ICacheService _cacheService;
        public PostController(PostService service, ICacheService cacheService) : base(service)
        {
            _cacheService = cacheService;
        }

        /// <summary>
        /// 查询所有博客菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet("routers")]
        public IEnumerable<string> GetPostRouters()
        {
            return _service.GetPostRouters();
        }

        /// <summary>
        /// 分页获取博客
        /// </summary>
        /// <param name="searchList"></param>
        /// <param name="orderBy"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="viewId"></param>
        /// <returns></returns>
        [HttpGet("search"), AllowAnonymous]
        public override DataModel<Post> GetViewData(string? pageSize, string? pageIndex, string? searchList, string? viewId, string? searchValue)
        {
            return base.GetViewData(pageSize, pageIndex, searchList, viewId, searchValue);
        }

        /// <summary>
        /// 获取博客分类数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("categories"), AllowAnonymous]
        public PostCategories categories()
        {
            return _service.GetCategories();
        }

        /// <summary>
        /// 获取博客
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}"), AllowAnonymous]
        public override Post GetData(string id)
        {
            return _cacheService.GetOrCreate(id, () => base.GetData(id), TimeSpan.FromHours(2));
        }

        /// <summary>
        /// 导出Markdown
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("export/markdown/{id}")]
        public IActionResult ExportMarkdown(string id)
        {
            HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
            var result = _service.ExportMarkdown(id);
            return File(result.bytes, result.ContentType, result.fileName);
        }

        /// <summary>
        /// 生成摘要
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        [HttpPost, Route("generate_summary")]
        public async Task<string> GenerateSummary()
        {
            var content = "";
            using (var reader = new System.IO.StreamReader(Request.Body))
            {
                content = await reader.ReadToEndAsync();
            }
            return await _service.GenerateSummary(content);
        }

        /// <summary>
        /// 生成 Markdown 文章
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost, Route("generate_markdown")]
        public async Task<string> GenerateMarkdownContent(AIInputDto dto)
        {
            return await _service.GenerateMarkdownContent(dto.Prompt);
        }
    }
}
