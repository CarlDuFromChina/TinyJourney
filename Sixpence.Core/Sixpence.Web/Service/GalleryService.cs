using Sixpence.Web.Config;
using Sixpence.Common;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.ORM.Repository;
using Newtonsoft.Json;
using Sixpence.Web.Model.Pixabay;
using Sixpence.Web.Model.Gallery;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.Web.Utils;
using Sixpence.ORM;
using Sixpence.Common.Extensions;
using Sixpence.Common.Crypto;
using Sixpence.ORM.Entity;

namespace Sixpence.Web.Service
{
    public class GalleryService : EntityService<Gallery>
    {
        #region 构造函数
        public GalleryService() : base() { }

        public GalleryService(IEntityManager manager) : base(manager) { }
        #endregion

        public override IList<EntityView> GetViewList()
        {
            return new List<EntityView>() {
                new EntityView()
                {
                    ViewId = "0F0DC786-CF7D-4997-B42C-47FB09B12AAE",
                    Sql = "select * from gallery where 1 = 1",
                    CustomFilter = new List<string>(){ "tags" },
                    Name = "本地图库",
                    OrderBy = "created_at desc"
                }
            };
        }

        /// <summary>
        /// 下载指定Url图片
        /// </summary>
        /// <param name="url"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        private string DownloadImage(string url, string objectid, string source = "gallery")
        {
            var result = HttpUtil.DownloadImage(url, out var contentType);
            using (var stream = result.ToStream())
            {
                var hash_code = SHAUtil.GetFileSHA1(stream);

                var config = StoreConfig.Config;
                var id = Guid.NewGuid().ToString();
                var originFileName = url.Substring(url.LastIndexOf("/") + 1); // 原始图片名
                var fileType = originFileName.GetFileType(); // 文件类型
                var fileName = $"{EntityCommon.GenerateGuidNumber()}.{fileType}"; // 新文件名
                var filePath = AppContext.Storage.UploadAsync(stream, fileName).Result;

                var data = new SysFile()
                {
                    Id = id,
                    Name = fileName,
                    RealName = fileName,
                    HashCode = hash_code,
                    FileType = source,
                    ContentType = contentType,
                    ObjectId = objectid
                };
                return Manager.Create(data);
            }
        }

        /// <summary>
        /// 上传图片（Pixabay）
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public List<string> UploadImage(ImageModel image)
        {
            return Manager.ExecuteTransaction(() =>
            {
                var data = new Gallery()
                {
                    Id = Guid.NewGuid().ToString(),
                    Tags = image.tags
                };
                data.PreviewId = DownloadImage(image.previewURL, data.Id);
                data.ImageId = DownloadImage(image.largeImageURL, data.Id);
                data.PreviewUrl = SysFileService.GetDownloadUrl(data.PreviewId);
                data.ImageUrl = SysFileService.GetDownloadUrl(data.ImageId);
                base.CreateData(data);
                return new List<string>() { data.PreviewId, data.ImageId };
            });
        }

        /// <summary>
        /// 获取随机图片
        /// </summary>
        public Gallery GetRandomImage()
        {
            var result = HttpUtil.Get("https://api.ixiaowai.cn/api/api.php?return=json");
            var model = JsonConvert.DeserializeObject<RandomImageModel>(result);
            var originFileName = model.imgurl.Substring(model.imgurl.LastIndexOf("/") + 1); // 原始图片名
            var imgBytes = HttpUtil.DownloadImage(model.imgurl, out var contentType);
            var imgStream = imgBytes.ToStream();
            var suffix = model.imgurl.GetFileType();
            var sysFileService = new SysFileService(Manager);

            return Manager.ExecuteTransaction(() =>
            {
                var galleryid = Guid.NewGuid().ToString();
                var image = sysFileService.UploadFile(imgStream, suffix, "gallery", contentType, galleryid, originFileName);
                var thumbStream = ImageHelper.GetThumbnail(image.GetFilePath());
                var image2 = sysFileService.UploadFile(thumbStream, suffix, "gallery", contentType, galleryid, sysFileService.GetPreviewImageFileName(originFileName));

                var gallery = new Gallery()
                {
                    Id = galleryid,
                    PreviewId = image2.Id,
                    PreviewUrl = image2.DownloadUrl,
                    ImageId = image.Id,
                    ImageUrl = image.DownloadUrl
                };
                Manager.Create(gallery);

                return gallery;
            });
        }
    }
}
