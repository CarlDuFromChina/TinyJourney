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
        [HttpGet("timeline")]
        public IEnumerable<TimelineModel> GetTimeline()
        {
            return new AnalysisService().GetTimeline();
        }
    }
}
