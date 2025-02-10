using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Entity;
using Sixpence.Web.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Cache
{
    public static class EntityCache
    {
        private const string EntityCachePrefix = "entity";
        private static readonly ConcurrentDictionary<string, SysEntity> Entities = new ConcurrentDictionary<string, SysEntity>();

        public static SysEntity GetEntity(IEntityManager manager, string entityName)
        {
            return Entities.GetOrAdd(EntityCachePrefix + entityName, (key) =>
            {
                var data = manager.QueryFirst<SysEntity>("select * from sys_entity where code = @name", new Dictionary<string, object>() { { "@name", entityName } });
                return data;
            });
        }

        public static IEnumerable<SysEntity> GetEntityList() => Entities.Values;
    }
}
