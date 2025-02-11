using Microsoft.Extensions.Logging;
using Sixpence.Common.Utils;
using Sixpence.Web.Model.Gallery;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.ImageResource
{
    public class LandscapeImageResourceDriver : IThirdPartyImageResourceDriver
    {
        private readonly ILogger<LandscapeImageResourceDriver> _logger;
        public LandscapeImageResourceDriver(ILogger<LandscapeImageResourceDriver> logger)
        {
            this._logger = logger;
        }

        public async Task<RandomImageModel> DownloadRandomImage()
        {
            string url = "https://picsum.photos/240/160";
            using (var client = new HttpClient())
            {
                try
                {
                    client.Timeout = TimeSpan.FromSeconds(10); // 设置超时时间为10秒
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    var imageBytes = await response.Content.ReadAsByteArrayAsync();
                    var contentType = response.Content.Headers.ContentType.MediaType;
                    var suffix = contentType.Split('/').Last();
                    var stream = new System.IO.MemoryStream(imageBytes);
                    return new RandomImageModel
                    {
                        Stream = stream,
                        ContentType = contentType,
                        Suffix = suffix,
                        FileName = Guid.NewGuid().ToString() + "." + suffix
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError($"下载图片失败，url：{url}", ex);
                    return null;
                }
            }
        }
    }
}
