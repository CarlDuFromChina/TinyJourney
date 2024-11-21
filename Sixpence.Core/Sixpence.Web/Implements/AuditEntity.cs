using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    public class AuditEntity : SormEntity
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
