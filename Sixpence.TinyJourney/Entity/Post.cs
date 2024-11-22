using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json.Linq;
using Sixpence.EntityFramework.Entity;
using Sixpence.Web;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Sixpence.TinyJourney.Entity
{
    [Table("post", "博客")]
    public partial class Post : AuditEntity
    {
        /// <summary>
        /// 实体id
        /// </summary>
        [DataMember]
        [PrimaryColumn(primaryType: PrimaryType.GUIDNumber)]
        public string? Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column, Description("名称")]
        public string? Name { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Column, Description("类型")]
        public string? PostType { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        [Column, Description("类型")]
        public string? PostTypeName { get; set; }

        /// <summary>
        /// Markdown内容
        /// </summary>
        [Column, Description("Markdown内容")]
        public string? Content { get; set; }

        /// <summary>
        /// html内容
        /// </summary>
        [Column, Description("html内容")]
        public string? HtmlContent { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [Column, Description("标题")]
        public string? Title { get; set; }

        /// <summary>
        /// 阅读次数
        /// </summary>
        [Column, Description("阅读次数")]
        public int? ReadingTimes { get; set; }

        /// <summary>
        /// 是否是系列
        /// </summary>
        [Column(DefaultValue: false), Description("是否是系列")]
        public bool? IsSeries { get; set; }

        /// <summary>
        /// 是否是系列
        /// </summary>
        [Column(DefaultValue: "否"), Description("是否是系列")]
        public string? IsSeriesName { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        [Column, Description("标签")]
        public string? Tags { get; set; }

        /// <summary>
        /// 禁止评论
        /// </summary>
        [Column(DefaultValue: false), Description("禁止评论")]
        public bool? DisableComment { get; set; }

        /// <summary>
        /// 封面
        /// </summary>
        [Column, Description("封面")]
        public string? SurfaceId { get; set; }

        /// <summary>
        /// 封面地址
        /// </summary>
        [Column, Description("封面地址")]
        public string? SurfaceUrl { get; set; }

        /// <summary>
        /// 大封面
        /// </summary>
        [Column, Description("大封面")]
        public string? BigSurfaceId { get; set; }

        /// <summary>
        /// 大封面地址
        /// </summary>
        [Column, Description("大封面地址")]
        public string? BigSurfaceUrl { get; set; }

        /// <summary>
        /// 是否展示
        /// </summary>
        [Column(DefaultValue: false), Description("是否展示")]
        public bool? IsShow { get; set; }

        /// <summary>
        /// 是否展示
        /// </summary>
        [Column(DefaultValue: "否"), Description("是否展示")]
        public string? IsShowName { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        [Column, Description("摘要")]
        public string? Brief { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        [Column(DefaultValue: false), Description("是否置顶")]
        public bool? IsPop { get; set; }

        /// <summary>
        /// 是否置顶
        /// </summary>
        [Column(DefaultValue: "否"), Description("是否置顶")]
        public string? IsPopName { get; set; }
    }
}

