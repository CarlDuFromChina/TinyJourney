using Sixpence.ORM.Entity;
using Sixpence.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.PortalEntity
{
    [Table("idea", "想法")]
    public partial class Idea : AuditEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryColumn]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column, Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Column, Description("内容")]
        public string Content { get; set; }
    }
}
