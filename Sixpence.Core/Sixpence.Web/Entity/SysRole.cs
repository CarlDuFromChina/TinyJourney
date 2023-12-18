using Sixpence.ORM.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace Sixpence.Web.Entity
{
    [Table(Description: "角色")]
    public partial class SysRole : SormEntity
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
        /// 描述
        /// </summary>
        [Column, Description("描述")]
        public string Description { get; set; }

        /// <summary>
        /// 是否基础角色
        /// </summary>
        [Column, Description("是否基础角色")]
        public bool? IsBasic { get; set; }

        /// <summary>
        /// 继承角色
        /// </summary>
        [Column, Description("继承角色")]
        public string InheritedRoleId { get; set; }
    }
}

