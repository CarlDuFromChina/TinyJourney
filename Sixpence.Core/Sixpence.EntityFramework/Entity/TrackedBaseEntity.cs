﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework.Entity
{
    /// <summary>
    /// 标准实体
    /// </summary>
    public class TrackedBaseEntity : BaseEntity
    {
        /// <summary>
        /// 创建日期
        /// </summary>
        [Column(CanBeNull: false), Description("创建时间")]
        public DateTime? CreatedAt { get; set; }

        /// <summary>
        /// 修改日期
        /// </summary>
        [Column(CanBeNull: false), Description("修改时间")]
        public DateTime? UpdatedAt { get; set; }
    }
}
