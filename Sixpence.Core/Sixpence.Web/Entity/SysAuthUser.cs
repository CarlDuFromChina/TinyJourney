using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sixpence.Web.Entity
{
    [Table(Description: "用户授权")]
    public partial class SysAuthUser : TrackedBaseEntity
    {
        [PrimaryColumn]
        public string Id { get; set; }

        [Column, Description("名称")]
        public string Name { get; set; }

        [Column, Description("编码")]
        public string Code { get; set; }

        [Column, Description("密码")]
        public string Password { get; set; }

        [Column, Description("角色权限id")]
        public string RoleId { get; set; }

        [Column, Description("角色权限名")]
        public string RoleName { get; set; }

        [Column, Description("用户id")]
        public string UserId { get; set; }

        [Column, Description("锁定")]
        public bool? IsLock { get; set; }

        [Column, Description("上次登录时间")]
        public DateTime? LastLoginTime { get; set; }

        [Column, Description("尝试登录次数")]
        public int? TryTimes { get; set; }
    }
}