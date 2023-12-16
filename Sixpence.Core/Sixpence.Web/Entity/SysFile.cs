using Sixpence.ORM.Entity;
using Sixpence.Web.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Web;

namespace Sixpence.Web.Entity
{
    [Table(Description: "系统文件")]
    public partial class SysFile : MetaEntity
    {
        [PrimaryColumn]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column, Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 文件对象
        /// </summary>
        [Column, Description("文件对象")]
        public string ObjectId { get; set; }

        /// <summary>
        /// 真实文件名
        /// </summary>
        [Column, Description("真实文件名")]
        public string RealName { get; set; }

        /// <summary>
        /// 文件路径
        /// </summary>
        [Column, Description("文件路径")]
        public string FilePath { get; set; }

        /// <summary>
        /// 哈希值
        /// </summary>
        [Column, Description("哈希值")]
        public string HashCode { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        [Column, Description("文件类型")]
        public string FileType { get; set; }

        /// <summary>
        /// 内容类型
        /// </summary>
        [Column, Description("内容类型")]
        public string ContentType { get; set; }
    }

    public partial class SysFile
    {
        public string DownloadUrl { get; set; }

        public string GetFilePath() => Path.Combine(FolderType.Storage.GetPath(), this.RealName);

        public static string GetFilePath(string fileName) => Path.Combine(FolderType.Storage.GetPath(), fileName);

        public FileInfoModel ToFileInfoModel()
        {
            return new FileInfoModel()
            {
                downloadUrl = this.DownloadUrl,
                id = this.Id,
                name = this.Name,
            };
        }
    }
}