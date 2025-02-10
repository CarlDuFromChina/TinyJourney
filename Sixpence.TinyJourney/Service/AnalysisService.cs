using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Repository;
using Sixpence.TinyJourney.Model;
using Sixpence.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.TinyJourney.Service
{
    public class AnalysisService : BaseService<AnalysisService>
    {
        public AnalysisService(IEntityManager manager, ILogger<AnalysisService> logger) : base(manager, logger)
        {
        }

        public IEnumerable<TimelineModel> GetTimeline()
        {
            var post = _manager.Query<TimelineModel>(@"SELECT created_at, title, created_by_name FROM post");
            return new List<TimelineModel>()
                .Concat(post)
                .OrderByDescending(item => item.created_at);
        }
    }
}
