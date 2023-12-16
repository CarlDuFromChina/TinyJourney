using Sixpence.ORM.Entity;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sixpence.Web.Entity
{
    [Table(Description: "消息提醒")]
    public partial class MessageRemind : SormEntity
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
        /// 接收人id
        /// </summary>
        [Column, Description("接收人id")]
        public string ReceiverId { get; set; }

        /// <summary>
        /// 接收人名称
        /// </summary>
        [Column, Description("接收人名称")]
        public string ReceiverName { get; set; }

        /// <summary>
        /// 是否阅读
        /// </summary>
        [Column, Description("是否阅读")]
        public bool? IsRead { get; set; }

        /// <summary>
        /// 消息内容
        /// </summary>
        [Column, Description("消息内容")]
        public string Content { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        [Column, Description("消息类型")]
        public string MessageType { get; set; }
    }
}

