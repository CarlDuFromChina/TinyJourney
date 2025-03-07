﻿using Sixpence.EntityFramework.Entity;
using Sixpence.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Sixpence.TinyJourney.Entity
{
    [Table, Description("连接")]
    public class Link : TrackedEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [PrimaryColumn]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column, Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 链接地址
        /// </summary>
        [Column, Description("链接地址")]
        public string LinkUrl { get; set; }

        /// <summary>
        /// 链接类型
        /// </summary>
        [Column, Description("链接类型")]
        public string LinkType { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [Column, Description("摘要")]
        public string Brief { get; set; }
    }
}
