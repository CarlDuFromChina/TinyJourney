﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace Sixpence.Common.Utils
{
    public class ImageUtil
    {
        public static Stream GetThumbnail(string fullFileName)
        {
            using (var image = Image.FromFile(fullFileName))
            {
                var width = 150;
                var height = width * image.Height / image.Width;
                var thumb = image.GetThumbnailImage(width, height, () => false, IntPtr.Zero);
                var mStream = new MemoryStream();
                thumb.Save(mStream, image.RawFormat);
                return mStream;
            }
        }
    }
}
