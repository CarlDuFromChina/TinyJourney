using Sixpence.Web.Config;
using Sixpence.Common;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.EntityFramework.Repository;
using Newtonsoft.Json;
using Sixpence.Web.Model.Pixabay;
using Sixpence.Web.Model.Gallery;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.Web.Utils;
using Sixpence.EntityFramework;
using Sixpence.Common.Extensions;
using Sixpence.Common.Crypto;
using Sixpence.EntityFramework.Entity;
using System.Web;
using Sixpence.Web.ImageResource;
using Sixpence.Common.Http;

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
        private async Task<string> DownloadImage(string url, string objectid, string source = "gallery")
        {
            var result = await HttpHelper.GetAsync(url);
            using (var stream = result.Content.ReadAsStream())
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
                    ContentType = result.Content.Headers.ContentType.MediaType,
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
                data.PreviewId = DownloadImage(image.previewURL, data.Id).Result;
                data.ImageId = DownloadImage(image.largeImageURL, data.Id).Result;
                data.PreviewUrl = SysFileService.GetDownloadUrl(data.PreviewId);
                data.ImageUrl = SysFileService.GetDownloadUrl(data.ImageId);
                base.CreateData(data);
                return new List<string>() { data.PreviewId, data.ImageId };
            });
        }

        /// <summary>
        /// 获取随机图片
        /// </summary>
        public async Task<Gallery> GetRandomImage()
        {
            var result = await ServiceFactory.Resolve<IThirdPartyImageResourceDriver>("LandscapeImageResourceDriver").DownloadRandomImage();
            if (result == null)
            {
                throw new SpException("图片下载失败，请重试");
            }
            var sysFileService = new SysFileService(Manager);

            return Manager.ExecuteTransaction(() =>
            {
                var galleryid = Guid.NewGuid().ToString();
                var image = sysFileService.UploadFile(result.Stream, result.Suffix, "gallery", result.ContentType, galleryid, result.FileName);
                //var thumbStream = ImageHelper.CreateThumbnail(image.GetFilePath(), 240, 160);
                //var image2 = sysFileService.UploadFile(thumbStream, result.Suffix, "gallery", result.ContentType, galleryid, sysFileService.GetPreviewImageFileName(result.FileName));

                var gallery = new Gallery()
                {
                    Id = galleryid,
                    PreviewId = image.Id,
                    PreviewUrl = image.DownloadUrl,
                    ImageId = image.Id,
                    ImageUrl = image.DownloadUrl
                };
                Manager.Create(gallery);

                return gallery;
            });
        }
    }
}
