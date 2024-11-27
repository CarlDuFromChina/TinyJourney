using Sixpence.EntityFramework.Entity;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace Sixpence.Web.Entity
{
    [Table(Description: "角色权限")]
    public partial class SysRolePrivilege : TrackedBaseEntity
    {
        /// <summary>
        /// 实体id
        /// </summary>
        [PrimaryColumn]
        public string Id { get; set; }

        /// <summary>
        /// 角色id
        /// </summary>
        [Column, Description("角色id")]
        public string RoleId { get; set; }

        /// <summary>
        /// 角色名
        /// </summary>
        [Column, Description("角色名")]
        public string RoleName { get; set; }

        /// <summary>
        /// 权限值
        /// </summary>
        [Column, Description("权限值")]
        public int? Privilege { get; set; }

        /// <summary>
        /// 对象id
        /// </summary>
        [Column, Description("对象id")]
        public string ObjectId { get; set; }

        /// <summary>
        /// 对象名
        /// </summary>
        [Column, Description("对象名")]
        public string ObjectName { get; set; }

        /// <summary>
        /// 对象类型
        /// </summary>
        [Column, Description("对象类型")]
        public string ObjectType { get; set; }
    }
}

