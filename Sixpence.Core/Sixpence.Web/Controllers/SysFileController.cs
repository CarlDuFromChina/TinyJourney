using Sixpence.Web.Config;
using Sixpence.Common.Utils;
using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Sixpence.Common;
using Sixpence.Web.Service;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;

namespace Sixpence.Web.Controllers
{
    public class SysFileController : EntityBaseController<SysFile, SysFileService>
    {
        /// <summary>
        /// 通用下载接口
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route("Download")]
        public async Task<IActionResult> DownloadAsync(string objectId)
        {
            return await AppContext.Storage.DownloadAsync(objectId);
        }

        /// <summary>
        /// 通用上传接口
        /// </summary>
        /// <param name="files"></param>
        /// <param name="fileType"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [Route("Upload")]
        public List<FileInfoModel> Upload([FromForm] List<IFormFile> files, [FromQuery] string fileType, [FromQuery] string objectId = "")
        {
            if (files == null || !files.Any())
                throw new SpException("上传文件不能为空");

            var fileList = new List<FileInfoModel>();

            foreach (var file in files)
            {
                var sysFile = new SysFileService().UploadFile(file.OpenReadStream(), file.FileName.GetFileType(), fileType, file.ContentType, objectId, file.FileName);
                fileList.Add(sysFile.ToFileInfoModel());
            }

            return fileList;
        }

        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [Route("upload_image")]
        public FileInfoModel UploadImage([FromForm] IFormFile file, [FromQuery] string fileType, [FromQuery] string objectId = "")
        {
            if (file == null)
                return null;

            var stream = file.OpenReadStream();
            var contentType = file.ContentType;
            var suffix = file.FileName.GetFileType();
            var image = new SysFileService().UploadFile(stream, suffix, fileType, contentType, objectId, file.FileName);

            return image.ToFileInfoModel();
        }

        /// <summary>
        /// 上传图片，自动生成预览图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(100 * 1024 * 1024)]
        [Route("upload_big_image")]
        public IEnumerable<FileInfoModel> UploadBigImage([FromForm] IFormFile file, [FromQuery] string fileType, [FromQuery] string objectId = "")
        {
            if (file == null)
                return null;

            return new SysFileService().UploadBigImage(file, fileType, objectId);
        }
    }
}