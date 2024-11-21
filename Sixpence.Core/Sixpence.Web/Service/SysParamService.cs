using Sixpence.EntityFramework;
using Sixpence.Web.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sixpence.Web.Service
{
    public class SysParamService : EntityService<SysParam>
    {
        #region 构造函数
        public SysParamService() : base() { }

        public SysParamService(IEntityManager manger) : base(manger) { }
        #endregion
    }
}