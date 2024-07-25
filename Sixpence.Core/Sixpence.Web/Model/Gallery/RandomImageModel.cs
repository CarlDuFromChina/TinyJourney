using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Model.Gallery
{
    public class RandomImageModel
    {
        public string FileName { get; set; }
        public string Suffix { get; set; }
        public string ContentType { get; set; }
        public Stream Stream { get; set; }
    }
}
