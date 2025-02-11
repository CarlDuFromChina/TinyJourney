using Sixpence.EntityFramework.Entity;
using System;
using System.ComponentModel;
using System.Runtime.Serialization;


namespace Sixpence.Web.Entity
{
    [Table, Description("图库")]
    public partial class Gallery : TrackedBaseEntity
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
        /// 标签
        /// </summary>
        [Column, Description("标签")]
        public string Tags { get; set; }

        /// <summary>
        /// 预览图
        /// </summary>
        [Column, Description("预览图")]
        public string PreviewUrl { get; set; }

        /// <summary>
        /// 大图
        /// </summary>
        [Column, Description("大图")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// 预览图片id
        /// </summary>
        [Column, Description("预览图片id")]
        public string PreviewId { get; set; }

        /// <summary>
        /// 大图id
        /// </summary>
        [Column, Description("大图id")]
        public string ImageId { get; set; }
    }
}

