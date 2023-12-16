using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.PortalModel;
using Sixpence.PortalService;

namespace Sixpence.PortalController
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
