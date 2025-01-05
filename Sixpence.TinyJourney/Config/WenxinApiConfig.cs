using Sixpence.Common.Config;

namespace Sixpence.TinyJourney.Config
{
    public class WenxinApiConfig : BaseAppConfig<WenxinApiConfig>
    {
        public string ApiKey { get; set; }
        public string SecretKey { get; set; }
    }
}