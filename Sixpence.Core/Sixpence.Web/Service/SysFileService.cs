using Sixpence.Web.Config;
using Sixpence.Common;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.Common.Crypto;
using Sixpence.Web.Utils;
using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Entity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Sixpence.Web.Service
{
    public class SysFileService : EntityService<SysFile>
    {
        private readonly Lazy<IStorage> _storage;

        public SysFileService(IEntityManager manager, ILogger<EntityService<SysFile>> logger, IRepository<SysFile> repository, IServiceProvider provider) : base(manager, logger, repository)
        {
            _storage = new Lazy<IStorage>(() => provider.GetServices<IStorage>().FirstOrDefault(StoreConfig.Resolve));
        }

        public override IList<EntityView> GetViewList()
        {
            return new List<EntityView>()
            {
                new EntityView()
                {
                    Name = "所有文件",
                    ViewId = "DD1D72FB-D7DE-49AC-B387-273375E6A7BA",
                    Sql = @"
SELECT
	id,
	NAME,
	file_type,
	content_type,
	created_at,
	created_by_name
FROM
	sys_file
",
                    OrderBy = "created_at desc",
                    CustomFilter = new List<string>(){ "name" }
                },
                new EntityView()
                {
                    Name = "图库",
                    ViewId = "3BCF6C07-2B49-4D69-9EB1-A3D5B721C976",
                    Sql = $@"
SELECT
	id,
	NAME,
	file_type,
	created_at,
	created_by_name,
	concat('{GetDownloadUrl("")}', id) AS downloadUrl
FROM
	sys_file
",
                    OrderBy = "",
                    CustomFilter = new List<string>(){ "name" }
                }
            };
        }

        public IEnumerable<SysFile> GetDataByCode(string code)
        {
            return _manager.Query<SysFile>(new { hash_code = code });
        }

        public SysFile UploadFile(Stream stream, string fileSuffix, string fileType, string contentType, string objectId, string fileName = "")
        {
            try
            {
                var id = Guid.NewGuid().ToString();
                var hash_code = SHAUtil.GetFileSHA1(stream);
                var newFileName = $"{EntityCommon.GenerateGuidNumber()}.{fileSuffix}"; // GUID 生成文件名

                // 保存图片到本地
                // TODO：执行失败回滚操作
                _storage.Value.UploadAsync(stream, newFileName).Wait();

                var sysFile = new SysFile()
                {
                    Id = id,
                    Name = fileName,
                    RealName = newFileName,
                    HashCode = hash_code,
                    FileType = fileType,
                    ObjectId = objectId,
                    ContentType = contentType,
                    DownloadUrl = GetDownloadUrl(id)
                };
                CreateData(sysFile);

                return sysFile;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new SpException("文件上传失败");
            }
        }

        /// <summary>
        /// 上传大图并自动生成缩略图
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileType"></param>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public IEnumerable<FileInfoModel> UploadBigImage(IFormFile file, string fileType, string objectId = "")
        {
            var stream = file.OpenReadStream();
            var contentType = file.ContentType;
            var suffix = file.FileName.GetFileType();

            return _manager.ExecuteTransaction(() =>
            {
                // 上传大图
                var image = UploadFile(stream, suffix, fileType, contentType, objectId, file.FileName);
                var thumbStream = ImageHelper.CreateThumbnail(image.GetFilePath(), 240, 160);
                var image2 = UploadFile(thumbStream, suffix, fileType, contentType, objectId, GetPreviewImageFileName(file.FileName));
                return new List<FileInfoModel>()
                {
                    image.ToFileInfoModel(),
                    image2.ToFileInfoModel()
                };
            });
        }

        /// <summary>
        /// 获取本地下载地址
        /// </summary>
        /// <param name="fileid"></param>
        /// <returns></returns>
        public static string GetDownloadUrl(string fileid, bool isRelative = true)
        {
            if (isRelative)
            {
                return $"/api/sys_file/download?objectId={fileid}";
            }
            var config = SystemConfig.Config;
            return $"{config.Protocol}://{config.Domain}/api/sys_file/download?objectId={fileid}";
        }

        /// <summary>
        /// 生成预览图文件名
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string GetPreviewImageFileName(string fileName)
        {
            AssertUtil.IsNullOrEmpty(fileName, "上传文件文件名不能为空");
            var fileExtension = Path.GetExtension(fileName);
            var newFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_small.{fileExtension}";
            return newFileName;
        }
    }
}