using Sixpence.EntityFramework.Entity;
using Sixpence.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.TinyJourney.Entity
{
    [Table("draft", "草稿")]
    public partial class Draft : TrackedEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryColumn(primaryType: PrimaryType.GUIDNumber)]
        public string Id { get; set; }

        /// <summary>
        /// 博客id
        /// </summary>
        [Column, Description("博客id")]
        public string PostId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Column, Description("标题")]
        public string Content { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Column, Description("标题")]
        public string Title { get; set; }
    }
}
