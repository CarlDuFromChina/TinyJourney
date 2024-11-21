using Sixpence.EntityFramework.Entity;
using Sixpence.Web;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sixpence.PortalEntity
{
    [Table("category", "博客分类")]
    public partial class Category : AuditEntity
    {
        /// <summary>
        /// 实体id
        /// </summary>
        [PrimaryColumn, Description("id")]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column, Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 编码
        /// </summary>
        [Column, Description("编码")]
        public string Code { get; set; }

        /// <summary>
        /// 索引
        /// </summary>
        [Column, Description("索引")]
        public int Index { get; set; }
    }
}

