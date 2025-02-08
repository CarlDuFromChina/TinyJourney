using Sixpence.EntityFramework.Entity;
using Sixpence.EntityFramework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    /// <summary>
    /// 主键
    /// </summary>
    public interface IPrimaryEntityColumn : IEntityColumn
    {
        /// <summary>
        /// 主键类型
        /// </summary>
        public PrimaryType PrimaryType { get; set; }
    }

    /// <summary>
    /// 主键类型
    /// </summary>
    public enum PrimaryType
    {
        /// <summary>
        /// GUID，字符串类型
        /// </summary>
        GUID,
        /// <summary>
        /// GUID ，字符串类型
        /// 注意：C#和数据库字段类型为string，因为JS的Number类型有精度问题，会导致GUID转换为数字后丢失精度
        /// </summary>
        GUIDNumber,
    }
}
