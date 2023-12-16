using Sixpence.ORM.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Entity
{
    /// <summary>
    /// 元信息实体（包含创建人和修改人）
    /// </summary>
    public class MetaEntity : SormEntity
    {
        [Column]
        [Description("创建人 Id")]
        public string CreatedBy { get; set; }

        [Column]
        [Description("创建人姓名")]
        public string CreatedByName { get; set; }

        [Column]
        [Description("修改人 Id")]
        public string UpdatedBy { get; set; }

        [Column]
        [Description("修改人姓名")]
        public string UpdatedByName { get; set; }
    }
}
