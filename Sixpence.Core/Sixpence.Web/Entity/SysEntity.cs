using Sixpence.EntityFramework.Entity;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sixpence.Web.Entity
{
    [Table(Description: "实体")]
    public partial class SysEntity : TrackedBaseEntity
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
        [Column(IsUnique: true), Description("编码")]
        public string Code { get; set; }

        [Column, Description("模式")]
        public string Schema { get; set; }
    }
}