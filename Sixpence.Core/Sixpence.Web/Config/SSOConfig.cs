using Sixpence.Common.Config;

namespace Sixpence.Web.Config;

/// <summary>
/// SSO 配置
/// </summary>
public class SSOConfig : BaseAppConfig<SSOConfig>
{
    /// <summary>
    /// Github 单点登录配置
    /// </summary>
    public OAuthConfig Github { get; set; }
    
    /// <summary>
    /// Gitee 单点登录配置
    /// </summary>
    public OAuthConfig Gitee { get; set; }

}

public class OAuthConfig
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
}