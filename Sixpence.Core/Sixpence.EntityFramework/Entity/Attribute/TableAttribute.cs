using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework.Entity
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// 与数据库表映射
        /// </summary>
        /// <param name="TableName"></param>
        /// <param name="Schema"></param>
        public TableAttribute(string TableName = "", string Schema = "")
        {
            this.TableName = TableName;
            this.Schema = Schema;
        }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 模式
        /// </summary>
        public string Schema { get; set; }
    }
}
