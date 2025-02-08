using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sixpence.Web.Service;
using Sixpence.Web.Model;

namespace Sixpence.Web.Controllers
{
    [Authorize(Policy = "Api")]
    public class JobController : BaseApiController
    {
        private readonly JobService _jobService;
        public JobController(JobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public IList<JobModel> GetDataList()
        {
            return _jobService.GetDataList();
        }

        [HttpPost("run")]
        public void RunOnceNow(string name)
        {
            _jobService.RunOnceNow(name);
        }

        [HttpPost("pause")]
        public void Pause(string name)
        {
            _jobService.Pause(name);
        }

        [HttpPost("resume")]
        public void Resume(string name)
        {
            _jobService.Resume(name);
        }
    }
}