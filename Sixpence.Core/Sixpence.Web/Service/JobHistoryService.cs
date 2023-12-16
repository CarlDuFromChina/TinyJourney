using Sixpence.ORM;
using Sixpence.Web.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.Service
{
    public class JobHistoryService : EntityService<JobHistory>
    {
        #region 构造函数
        public JobHistoryService() : base() { }

        public JobHistoryService(IEntityManager manager) : base(manager) { }
        #endregion
    }
}
