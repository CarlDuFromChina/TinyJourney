using Sixpence.Web.Model.Gallery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.ImageResource
{
    public interface IThirdPartyImageResourceDriver
    {
        /// <summary>
        /// 下载随机图片
        /// </summary>
        /// <returns></returns>
        Task<RandomImageModel> DownloadRandomImage();
    }
}
