using Sixpence.Common;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sixpence.EntityFramework;
using Sixpence.Web.Module.SysAttrs;
using Sixpence.Web.Entity;

namespace Sixpence.Web.Service
{
    public class SysAttrsService : EntityService<SysAttrs>
    {
        #region 构造函数
        public SysAttrsService() : base() { }

        public SysAttrsService(IEntityManager manager) : base(manager) { }
        #endregion
    }
}