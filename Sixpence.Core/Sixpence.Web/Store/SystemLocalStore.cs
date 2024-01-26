using Sixpence.Common.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using Sixpence.Web.Entity;
using Sixpence.ORM;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;

namespace Sixpence.Web.Store
{
    public class SystemLocalStore : IStorage
    {
        private readonly IEntityManager manager;
        private readonly ILogger<SystemLocalStore> logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SystemLocalStore(IEntityManager manager, ILogger<SystemLocalStore> logger, IHttpContextAccessor contextAccessor)
        {
            this.manager = manager;
            this.logger = logger;
            _httpContextAccessor = contextAccessor;
        }

        /// <summary>
        /// 批量删除文件
        /// </summary>
        /// <param name="fileName"></param>
        public async Task DeleteAsync(IList<string> fileName)
        {
            foreach (var item in fileName)
            {
                var filePath = SysFile.GetFilePath(item);
                await FileUtil.DeleteFileAsync(filePath);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath"></param>
        public async Task<IActionResult> DownloadAsync(string objectId)
        {
            var data = manager.QueryFirst<SysFile>(objectId) ?? manager.QueryFirst<SysFile>(new { hash_code = objectId } );
            var fileInfo = new FileInfo(data.GetFilePath());
            if (fileInfo.Exists)
            {
                var stream = await FileUtil.GetFileStreamAsync(fileInfo.FullName);
                _httpContextAccessor.HttpContext.Response.Headers.Add("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileInfo.Name, System.Text.Encoding.UTF8));
                return new FileStreamResult(stream, "application/octet-stream");
            }
            logger.LogError($"文件{fileInfo.Name}未找到，文件路径：{fileInfo.FullName}");
            throw new FileNotFoundException();
        }

        /// <summary>
        /// 获取文件流
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Stream> GetStreamAsync(string id)
        {
            var data = manager.QueryFirst<SysFile>(id);
            return await FileUtil.GetFileStreamAsync(data.GetFilePath());
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileName"></param>
        public async Task<string> UploadAsync(Stream stream, string fileName)
        {
            var filePath = $"{Path.AltDirectorySeparatorChar}storage{Path.AltDirectorySeparatorChar}{fileName}"; // 相对路径
            await FileUtil.SaveFileAsync(stream, SysFile.GetFilePath(fileName));
            return filePath;
        }
    }
}
