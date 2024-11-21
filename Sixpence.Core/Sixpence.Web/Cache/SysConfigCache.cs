using System;
using Sixpence.Web.Entity;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Sixpence.EntityFramework;

namespace Sixpence.Web.Cache
{
    /// <summary>
    /// 系统参数缓存
    /// </summary>
	public class SysConfigCache
    {
        private const string CACHE_PREFIX = "SysConfig";
        private static readonly ConcurrentDictionary<string, Entity.SysConfig> settings = new ConcurrentDictionary<string, Entity.SysConfig>();

        public static object GetValue<T>() where T : ISysConfig, new()
        {
            var t = new T();

            var config = settings.GetOrAdd(CACHE_PREFIX + t.Code, (key) =>
            {
                var sql = @"select * from sys_config where code = @code;";
                using (var manager = new EntityManager())
                {
                    var data = manager.QueryFirst<Entity.SysConfig>(sql, new Dictionary<string, object>() { { "@code", t.Code } });
                    return data;
                }
            });

            return config?.Value ?? t.DefaultValue;
        }

        public static object GetValue(string code)
        {
            var config = settings.GetOrAdd(CACHE_PREFIX + code, (key) =>
            {
                var sql = @"select * from sys_config where code = @code;";
                using (var manager = new EntityManager())
                {
                    var data = manager.QueryFirst<SysConfig>(sql, new Dictionary<string, object>() { { "@code", code } });
                    return data;
                }
            });

            return config?.Value;
        }
    }
}

