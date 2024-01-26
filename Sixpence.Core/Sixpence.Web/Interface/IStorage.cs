using Microsoft.AspNetCore.Mvc;
using Sixpence.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    /// <summary>
    /// 存储策略接口
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        Task<string> UploadAsync(Stream stream, string fileName);

        /// <summary>
        /// 下载文件
        /// </summary>
        Task<IActionResult> DownloadAsync(string objectId);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileName"></param>
        Task DeleteAsync(IList<string> fileName);

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Stream> GetStreamAsync(string id);
    }
}
