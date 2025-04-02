using Sixpence.Common.Cache;
using Sixpence.Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Config
{
    public class RedisConfig : BaseAppConfig<RedisConfig>
    {
        public string ConnectionString { get; set; }
    }
}
