using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Sixpence.Web.Entity
{
    [Table(Description: "系统配置")]
    public class SysConfig : TrackedBaseEntity
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
        /// 描述
        /// </summary>
        [Column, Description("描述")]
        public string Description { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [Column, Description("值")]
        public string Value { get; set; }
    }
}