using Sixpence.Common;
using Sixpence.ORM;
using Sixpence.ORM.Entity;
using Sixpence.Web.Cache;
using Sixpence.Web.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sixpence.Web.Service
{
    public class SysConfigService : EntityService<SysConfig>
    {
        #region 构造函数
        public SysConfigService() : base() { }

        public SysConfigService(IEntityManager manager) : base(manager) { }
        #endregion

        public object GetValue(string code)
        {
            return SysConfigCache.GetValue(code);
        }

        public void CreateMissingConfig(IEnumerable<ISysConfig> settings)
        {
            settings.Each(item =>
            {
                var data = Manager.QueryFirst<SysConfig>(new { code = item.Code });
                if (data == null)
                {
                    data = new SysConfig()
                    {
                        Id = EntityCommon.GenerateGuid(),
                        Name = item.Name,
                        Code = item.Code,
                        Value = item.DefaultValue.ToString(),
                        Description = item.Description
                    };
                    Manager.Create(data);
                }
            });
        }
    }
}