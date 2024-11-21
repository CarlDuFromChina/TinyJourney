using Sixpence.EntityFramework.Entity;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Sixpence.Web.Module.SysAttrs
{
    [Table(Description: "实体字段")]
    public partial class SysAttrs : SormEntity
    {
        /// <summary>
        /// 实体id
        /// </summary>
        [PrimaryColumn]
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
        /// 实体id
        /// </summary>
        [Column, Description("实体id")]
        public string EntityId { get; set; }

        /// <summary>
        /// 实体名
        /// </summary>
        [Column, Description("实体名")]
        public string EntityName { get; set; }

        /// <summary>
        /// 字段类型
        /// </summary>
        [Column, Description("字段类型")]
        public string AttrType { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        [Column, Description("字段长度")]
        public int? AttrLength { get; set; }

        /// <summary>
        /// 是否必填
        /// </summary>
        [Column, Description("是否必填")]
        public bool? IsRequire { get; set; }

        /// <summary>
        /// 默认值
        /// </summary>
        [Column, Description("默认值")]
        public string DefaultValue { get; set; }
    }

    public partial class SysAttrs
    {
        public string EntityCode { get; set; }
    }
}