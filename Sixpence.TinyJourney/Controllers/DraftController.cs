﻿using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.TinyJourney.Entity;
using Sixpence.TinyJourney.Service;
using Newtonsoft.Json;

namespace Sixpence.TinyJourney.Controller
{
    public class DraftController : EntityBaseController<Draft, DraftService>
    {
        public DraftController(DraftService service) : base(service)
        {
        }

        /// <summary>
        /// 根据博客id获取草稿
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("post/{postid}")]
        public Draft GetDataByPostId(string postid)
        {
            return _service.GetDataByPostId(postid);
        }

        /// <summary>
        /// 获取博客草稿（新建）
        /// </summary>
        /// <returns></returns>
        [HttpGet("drafts")]
        public IList<Draft> GetDrafts()
        {
            return _service.GetDrafts();
        }

        [HttpPost]
        public override string CreateData(Draft entity)
        {
            var id = base.CreateData(entity);
            return JsonConvert.SerializeObject(id);
        }

        [HttpPost("save")]
        public override string CreateOrUpdateData(Draft entity)
        {
            var id = base.CreateOrUpdateData(entity);
            return JsonConvert.SerializeObject(id);
        }
    }
}
