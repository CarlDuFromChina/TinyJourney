using Sixpence.EntityFramework.Entity;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.ComponentModel;

namespace Sixpence.Web.Entity
{
    [Table(Description: "邮箱验证")]
    public class MailVertification : TrackedEntity
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
        /// 邮箱地址
        /// </summary>
        [Column, Description("邮箱地址")]
        public string MailAddress { get; set; }

        /// <summary>
        /// 登录请求信息
        /// </summary>
        [Column, Description("登录请求信息")]
        public string LoginRequest { get; set; }

        /// <summary>
        /// 过期时间
        /// </summary>
        [Column, Description("过期时间")]
        public DateTime? ExpireTime { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Column, Description("消息内容")]
        public string Content { get; set; }

        /// <summary>
        /// 是否激活
        /// </summary>
        [Column, Description("是否激活")]
        public bool? IsActive { get; set; }

        /// <summary>
        /// 激活类型
        /// </summary>
        [Column, Description("激活类型")]
        public string MailType { get; set; }
    }

    public enum MailType
    {
        Activation,
        ResetPassword
    }
}
