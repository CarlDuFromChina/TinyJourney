using Sixpence.EntityFramework.Entity;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sixpence.Web.Entity
{
    [Table(Description: "用户")]
    public partial class SysUser : SormEntity
    {
        /// <summary>
        /// 用户id
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

        /// <summary>
        /// 性别，1男，2女，9未知
        /// </summary>
        [Column, Description("性别")]
        public int? Gender { get; set; }

        /// <summary>
        /// 性别
        /// </summary>
        [Column, Description("性别")]
        public string GenderName { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [Column, Description("真实姓名")]
        public string Realname { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        [Column(IsUnique: true), Description("邮箱")]
        public string Mailbox { get; set; }

        /// <summary>
        /// 个人介绍
        /// </summary>
        [Column, Description("个人介绍")]
        public string Introduction { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [Column(IsUnique: true), Description("手机号码")]
        public string Cellphone { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [Column, Description("头像")]
        public string Avatar { get; set; }

        /// <summary>
        /// 生活照
        /// </summary>
        [Column, Description("生活照")]
        public string LifePhoto { get; set; }

        /// <summary>
        /// 角色权限id
        /// </summary>
        [Column, Description("角色权限id")]
        public string RoleId { get; set; }

        /// <summary>
        /// Github ID
        /// </summary>
        [Column, Description("Github ID")]
        public string GithubId { get; set; }

        /// <summary>
        /// Gitee ID
        /// </summary>
        [Column, Description("Gitee ID")]
        public string GiteeId { get; set; }

        /// <summary>
        /// 角色权限名
        /// </summary>
        [Column, Description("角色权限名")]
        public string RoleName { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [Column, Description("启用")]
        public bool? isActive { get; set; }

        /// <summary>
        /// 启用
        /// </summary>
        [Column, Description("启用")]
        public string isActiveName { get; set; }
    }

    public partial class SysUser
    {
        public string Password { get; set; }
    }
}
