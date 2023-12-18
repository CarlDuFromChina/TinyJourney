using Sixpence.ORM.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Sixpence.Web.Entity
{
    [Table(Description: "选项")]
    public partial class SysParam : SormEntity
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
        /// 选项集
        /// </summary>
        [Column, Description("选项集")]
        public string SysParamGroupId { get; set; }

        /// <summary>
        /// 选项集名
        /// </summary>
        [Column, Description("选项集名")]
        public string SysParamGroupName { get; set; }
    }
}