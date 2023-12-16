﻿using Sixpence.ORM.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;

namespace Sixpence.Web.Entity
{
    /// <summary>
    /// 版本脚本执行日志
    /// </summary>
    [Table]
    public class VersionScriptExecutionLog : SormEntity
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
        /// 是否执行成功
        /// </summary>
        [Column, Description("是否执行成功")]
        public bool? IsSuccess { get; set; }
    }
}
