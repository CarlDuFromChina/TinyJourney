using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Common
{
    public static class DataTableExtension
    {
        /// <summary>
        /// 是否是个空表
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static bool IsEmpty(this DataTable dt)
        {
            return dt == null || dt.Rows.Count == 0;
        }

        public static string ToCSV(this DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count == 0)
            {
                throw new Exception("无数据可导出");
            }

            var sb = new StringBuilder();

            // 写表头
            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                sb.Append(dataTable.Columns[i].ColumnName);
                if (i < dataTable.Columns.Count - 1)
                    sb.Append(",");
            }
            sb.AppendLine();

            // 写数据行
            foreach (System.Data.DataRow row in dataTable.Rows)
            {
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    var value = row[i]?.ToString()?.Replace("\"", "\"\""); // 转义双引号
                    sb.Append($"\"{value}\"");
                    if (i < dataTable.Columns.Count - 1)
                        sb.Append(",");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
