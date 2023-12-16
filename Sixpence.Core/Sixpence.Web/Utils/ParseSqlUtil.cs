using Newtonsoft.Json;
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
        private char ParameterPrefix = AppContext.DB.DbDialect.ParameterPrefix;

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
            switch (type)
            {
                case SearchType.Equals:
                    return ($"= @{paramName}{count}", new Dictionary<string, object>() { { $"@{paramName}{count++}", value } });
                case SearchType.Like:
                    return ($"LIKE @{paramName}{count}", new Dictionary<string, object>() { { $"@{paramName}{count++}", value } });
                case SearchType.Greater:
                    return ($"> @{paramName}{count}", new Dictionary<string, object>() { { $"@{paramName}{count++}", value } });
                case SearchType.Less:
                    return ($"< @{paramName}{count}", new Dictionary<string, object>() { { $"@{paramName}{count++}", value } });
                case SearchType.Between:
                    var param1 = $"@{paramName}{count++}";
                    var param2 = $"@{paramName}{count++}";
                    var arr = JsonConvert.DeserializeObject<List<object>>(value?.ToString());
                    return ($"BETWEEN {param1} AND {param2}", new Dictionary<string, object>() { { param1, arr[0] }, { param2, arr[1] } });
                case SearchType.Contains:
                    var param = JsonConvert.DeserializeObject<List<object>>(value?.ToString());
                    return ($"= ANY({paramName}{count})", new Dictionary<string, object>() { { $"@{paramName}{count++}", param } });
                case SearchType.NotContains:
                    param = JsonConvert.DeserializeObject<List<object>>(value?.ToString());
                    return ($"<> ANY({paramName}{count})", new Dictionary<string, object>() { { $"@{paramName}{count++}", param } });
                default:
                    return ("", new Dictionary<string, object>() { });
            }
        }
    }
}
