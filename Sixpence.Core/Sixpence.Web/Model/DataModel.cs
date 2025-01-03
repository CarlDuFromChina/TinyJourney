﻿using Sixpence.EntityFramework.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Model
{
    /// <summary>
    /// 数据模型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataModel<T>
        where T : BaseEntity, new()
    {
        public IList<T> Data { get; set; }
        public int Count { get; set; }
    }

    /// <summary>
    /// 搜索条件
    /// </summary>
    public class SearchCondition
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public SearchType Type { get; set; }
    }

    /// <summary>
    /// 筛选字段逻辑类型
    /// </summary>
    public enum SearchType
    {
        [Description("相等")]
        Equals,
        [Description("相似")]
        Like,
        [Description("大于")]
        Greater,
        [Description("小于")]
        Less,
        [Description("之间")]
        Between,
        [Description("包含")]
        Contains,
        [Description("不包含")]
        NotContains
    }
}
