using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    /// <summary>
    /// 流扩展
    /// </summary>
    public static class StreamExtension
    {
        /// <summary>
        /// 文件流转Byte[]
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ToByteArray(this Stream stream)
        {
            var bytes = new byte[stream.Length];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        /// 转String
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string ToString(this Stream stream)
            => new StreamReader(stream).ReadToEnd();
    }
}
