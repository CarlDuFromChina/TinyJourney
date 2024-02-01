using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Utils
{
    public static class ImageHelper
    {
        public static Stream CreateThumbnail(string inputFilePath, int thumbWidth, int thumbHeight)
        {
            // 加载原始图像
            using (var inputStream = File.OpenRead(inputFilePath))
            using (var original = SKBitmap.Decode(inputStream))
            {
                // 计算缩放比例
                float scale = Math.Min((float)thumbWidth / original.Width, (float)thumbHeight / original.Height);

                // 计算新的尺寸
                var newWidth = (int)(original.Width * scale);
                var newHeight = (int)(original.Height * scale);

                // 创建新的图像
                using (var resizedBitmap = new SKBitmap(newWidth, newHeight))
                using (var canvas = new SKCanvas(resizedBitmap))
                {
                    // 使用高质量的缩放
                    canvas.SetMatrix(SKMatrix.CreateScale(scale, scale));
                    canvas.DrawBitmap(original, 0, 0, new SKPaint { FilterQuality = SKFilterQuality.High });

                    // 将缩略图保存为文件
                    using (var image = SKImage.FromBitmap(resizedBitmap))
                    using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 90)) // 使用较高的 JPEG 质量
                    {
                        return data.AsStream();
                    }
                }
            }
        }


        public static Stream GetThumbnail(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                using (var bitmap = SKBitmap.Decode(stream))
                {
                    var width = 150;
                    var height = width * bitmap.Height / bitmap.Width;
                    var scaledBitmap = bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
                    var memoryStream = new MemoryStream();
                    scaledBitmap.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(memoryStream);
                    memoryStream.Position = 0;
                    return memoryStream;
                }
            }
        }
    }
}
