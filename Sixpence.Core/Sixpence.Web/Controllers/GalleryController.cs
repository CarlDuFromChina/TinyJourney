using Sixpence.Web.WebApi;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Sixpence.Web.Model.Pixabay;
using Sixpence.Web.Entity;
using Sixpence.Web.Service;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Sixpence.Web.Controllers
{
    public class GalleryController : EntityBaseController<Gallery, GalleryService>
    {
        protected readonly PixabayService _pixabayService;
        public GalleryController(GalleryService service, PixabayService pixabayService) : base(service)
        {
            _pixabayService = pixabayService;
        }

        /// <summary>
        /// 搜索云图库
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [HttpGet("cloud/search")]
        public ImagesModel GetImages(string searchValue, int pageIndex, int pageSize)
        {
            return _pixabayService.GetImages(searchValue, pageIndex, pageSize);
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        [HttpPost("upload")]
        public List<string> UploadImage(ImageModel image)
        {
            return _service.UploadImage(image);
        }

        /// <summary>
        /// 获取随机图片
        /// </summary>
        /// <returns></returns>
        [HttpGet("random_image"), AllowAnonymous]
        public async Task<Gallery> RandomImage()
        {
            return await _service.GetRandomImage();
        }
    }
}
