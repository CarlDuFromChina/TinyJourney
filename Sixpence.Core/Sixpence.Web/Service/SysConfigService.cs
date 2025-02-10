using Microsoft.Extensions.Logging;
using Sixpence.Common;
using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Entity;
using Sixpence.Web.Cache;
using Sixpence.Web.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sixpence.Web.Service
{
    public class SysConfigService : EntityService<SysConfig>
    {
        private const string CACHE_PREFIX = "SysConfig";
        private static readonly ConcurrentDictionary<string, Entity.SysConfig> settings = new ConcurrentDictionary<string, Entity.SysConfig>();

        public SysConfigService(IEntityManager manager, ILogger<EntityService<SysConfig>> logger, IRepository<SysConfig> repository) : base(manager, logger, repository)
        {
        }

        public object GetValue(string code)
        {
            return GetCacheValue(code);
        }

        public void CreateMissingConfig(IEnumerable<ISysConfig> settings)
        {
            settings.Each(item =>
            {
                var data = _manager.QueryFirst<SysConfig>(new { code = item.Code });
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
                    _manager.Create(data);
                }
            });
        }

        private object GetCacheValue(string code)
        {
            var config = settings.GetOrAdd(CACHE_PREFIX + code, (key) =>
            {
                var sql = @"select * from sys_config where code = @code;";
                var data = _manager.QueryFirst<SysConfig>(sql, new Dictionary<string, object>() { { "@code", code } });
                return data;
            });

            return config?.Value;
        }
    }
}