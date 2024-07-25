using Newtonsoft.Json;
using Sixpence.Common;
using Sixpence.Common.Extensions;
using Sixpence.Common.Utils;
using Sixpence.Web.Model.Gallery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sixpence.Web.ImageResource
{
    /// <summary>
    /// 二次元图片资源驱动
    /// </summary>
    public class ACGImageResourceDriver : IThirdPartyImageResourceDriver
    {
        public async Task<RandomImageModel> DownloadRandomImage()
        {
            var result = await HttpUtil.GetAsync("https://api.aixiaowai.cn/api/api.php?return=json", new Dictionary<string, string>());
            var model = JsonConvert.DeserializeObject<XiaoWaiImageModel>(result);
            var originFileName = HttpUtility.ParseQueryString(new Uri(model.imgurl).Query).Get("url");
            var imgBytes = HttpUtil.DownloadImage(model.imgurl, out var contentType);
            var imgStream = imgBytes.ToStream();
            var suffix = model.imgurl.GetFileType();
            return new RandomImageModel()
            {
                FileName = originFileName,
                Suffix = suffix,
                ContentType = contentType,
                Stream = imgStream,
            };
        }
    }
}
