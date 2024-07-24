using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sixpence.Common.Utils
{
    public static class SchemaHelper
    {
        public static string RemoveSchemaName(string input)
        {
            // 正则表达式模式匹配 "schema_name." 或 "schema_name"." 格式
            string pattern = @"^(\w+\.|\w+""\.)";

            // 使用正则表达式检查是否存在模式名
            if (Regex.IsMatch(input, pattern))
            {
                // 如果存在,则移除模式名
                return Regex.Replace(input, pattern, "");
            }

            // 如果不存在模式名,则返回原始输入
            return input;
        }
    }
}
