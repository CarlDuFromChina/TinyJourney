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
}
