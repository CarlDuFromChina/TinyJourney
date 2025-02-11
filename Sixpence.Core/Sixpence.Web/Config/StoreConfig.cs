using Sixpence.Common.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Sixpence.Web.Config
{
    /// <summary>
    /// 存储方案配置
    /// </summary>
    public class StoreConfig : BaseAppConfig<StoreConfig>
    {
        /// <summary>
        /// 存储方式（SystemLocalStore、MinIOStore）
        /// </summary>
        public string Type { get; set; }

        private string _temp;
        
        /// <summary>
        /// 临时文件路径
        /// </summary>
        public string Temp
        {
            get
            {
                if (string.IsNullOrEmpty(_temp))
                {
                    _temp = GetCurrentSystemPath("temp");
                }

                return _temp;
            }
            set
            {
                _temp = value;
            }
        }

        private string _storage;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string Storage
        {
            get
            {
                if (string.IsNullOrEmpty(_storage))
                {
                    _storage = GetCurrentSystemPath("storage");
                }
                
                return _storage;
            }
            set
            {
                _storage = value;
            }
        }


        private static string GetCurrentSystemPath(string folderName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return $"C://{folderName}";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folderName);
            }

            return "";
        }

        public static bool Resolve(IStorage storage)
        {
            return Config.Type == storage.GetType().Name;
        }
    }
}
