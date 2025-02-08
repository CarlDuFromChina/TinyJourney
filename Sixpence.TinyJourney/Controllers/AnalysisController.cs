using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.TinyJourney.Model;
using Sixpence.TinyJourney.Service;

namespace Sixpence.TinyJourney.Controller
{
    public class AnalysisController : BaseApiController
    {
        protected readonly AnalysisService _service;
        public AnalysisController(AnalysisService analysisService)
        {
            _service = analysisService;
        }

        [HttpGet("timeline")]
        public IEnumerable<TimelineModel> GetTimeline()
        {
            return _service.GetTimeline();
        }
    }
}
