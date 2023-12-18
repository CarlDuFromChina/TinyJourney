﻿using Sixpence.ORM;
using Sixpence.ORM.Repository;
using Sixpence.PortalModel;
using Sixpence.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.PortalService
{
    public class AnalysisService : BaseService
    {
        public AnalysisService() : base() { }

        public AnalysisService(IEntityManager manager) : base(manager) { }

        public IEnumerable<TimelineModel> GetTimeline()
        {
            var post = _manager.Query<TimelineModel>(@"SELECT created_at, title, created_by_name FROM post");
            return new List<TimelineModel>()
                .Concat(post)
                .OrderByDescending(item => item.created_at);
        }
    }
}
