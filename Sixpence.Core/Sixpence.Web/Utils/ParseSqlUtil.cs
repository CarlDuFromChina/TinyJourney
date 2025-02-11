using Newtonsoft.Json;
using Sixpence.EntityFramework;
using Sixpence.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Utils
{
    public class ParseSqlUtil
    {
        /// <summary>
        /// 获取筛选SQL
        /// </summary>
        /// <param name="type"></param>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static (string sql, Dictionary<string, object> paramsList) GetSearchCondition(SearchType type, string paramName, object value, ref int count)
        {
            var sqlBuilder = ServiceCollectionExtensions.Options.DbSetting.Driver.SqlBuilder;
            var ParameterPrefix = sqlBuilder.ParameterPrefix;

            switch (type)
            {
                case SearchType.Equals:
                    return ($"= {ParameterPrefix}{paramName}{count}", new Dictionary<string, object>() { { $"{ParameterPrefix}{paramName}{count++}", value } });
                case SearchType.Like:
                    return ($"LIKE {ParameterPrefix}{paramName}{count}", new Dictionary<string, object>() { { $"{ParameterPrefix}{paramName}{count++}", value } });
                case SearchType.Greater:
                    return ($"> {ParameterPrefix}{paramName}{count}", new Dictionary<string, object>() { { $"{ParameterPrefix}{paramName}{count++}", value } });
                case SearchType.Less:
                    return ($"< {ParameterPrefix}{paramName}{count}", new Dictionary<string, object>() { { $"{ParameterPrefix}{paramName}{count++}", value } });
                case SearchType.Between:
                    var param1 = $"{ParameterPrefix}{paramName}{count++}";
                    var param2 = $"{ParameterPrefix}{paramName}{count++}";
                    var arr = JsonConvert.DeserializeObject<List<object>>(value?.ToString());
                    return ($"BETWEEN {param1} AND {param2}", new Dictionary<string, object>() { { param1, arr[0] }, { param2, arr[1] } });
                case SearchType.Contains:
                    {
                        var param = JsonConvert.DeserializeObject<List<object>>(value?.ToString());
                        var result = sqlBuilder.BuildInClauseSql(paramName, count, param, true);
                        return (result.sql, result.param);
                    }
                case SearchType.NotContains:
                    {
                        var param = JsonConvert.DeserializeObject<List<object>>(value?.ToString());
                        var result = sqlBuilder.BuildInClauseSql(paramName, count, param, false);
                        return (result.sql, result.param);
                    }
                default:
                    return ("", new Dictionary<string, object>() { });
            }
        }
    }
}
