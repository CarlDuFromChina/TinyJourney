using Sixpence.ORM.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace Sixpence.Web.Entity
{
    [Table(Description: "作业执行记录")]
    public class JobHistory : SormEntity
    {
        [PrimaryColumn]
        public string Id { get; set; }

        /// <summary>
        /// 作业名
        /// </summary>
        [Column, Description("作业名")]
        public string JobName { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [Column, Description("开始时间")]
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [Column, Description("结束时间")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Column, Description("状态")]
        public string Status { get; set; }

        /// <summary>
        /// 错误原因
        /// </summary>
        [Column, Description("错误原因")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 执行人
        /// </summary>
        [Column, Description("执行人")]
        public string ExecuteUserId { get; set; }

        /// <summary>
        /// 执行人名称
        /// </summary>
        [Column, Description("执行人名称")]
        public string ExecuteUserName { get; set; }
    }
}
