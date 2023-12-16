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
        public static Stream GetThumbnail(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var bitmap = SKBitmap.Decode(stream);
                var width = 150;
                var height = width * bitmap.Height / bitmap.Width;
                var scaledBitmap = bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
                var memoryStream = new MemoryStream();
                scaledBitmap.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }

        public static Stream GetThumbnail(string fileName, int width, int height)
        {
            using (var stream = File.OpenRead(fileName))
            {
                var bitmap = SKBitmap.Decode(stream);
                var scaledBitmap = bitmap.Resize(new SKImageInfo(width, height), SKFilterQuality.High);
                var memoryStream = new MemoryStream();
                scaledBitmap.Encode(SKEncodedImageFormat.Jpeg, 100).SaveTo(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
        }
    }
}
