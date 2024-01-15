using Microsoft.Extensions.Logging;
using Sixpence.Common.Crypto;
using Sixpence.Common.Extensions;
using Sixpence.Common.Utils;
using Sixpence.ORM;
using Sixpence.ORM.Entity;
using Sixpence.Web.Config;
using Sixpence.Web.Entity;
using Sixpence.Web.Model.Gitee;
using System;
using System.Text.Json;

namespace Sixpence.Web.Service
{
    public class GiteeAuthService : BaseService
    {
        public GiteeAuthService() : base() { }
        public GiteeAuthService(IEntityManager manager) : base(manager) { }

        public GiteeConfig GetConfig()
        {
            var configService = new SysConfigService(_manager);
            var data = configService.GetValue("gitee_oauth")?.ToString();
            return JsonSerializer.Deserialize<GiteeConfig>(data);
        }

        public GiteeAccessToken GetAccessToken(string code, string userid = "")
        {
            var config = GetConfig();
            var client_id = config.client_id;
            var client_secret = config.client_secret;
            var redirect_uri = $"{SystemConfig.Config.Protocol}://{SystemConfig.Config.Domain}/gitee-oauth";
            if (!string.IsNullOrEmpty(userid))
            {
                redirect_uri += $"?id={userid}";
            }

            _logger.LogDebug("GetAccessToken 请求入参：" + $"grant_type=authorization_code&code={code}&client_id={client_id}&client_secret={client_secret}&redirect_uri={redirect_uri}");
            var response = HttpUtil.Post($"https://gitee.com/oauth/token?grant_type=authorization_code&code={code}&client_id={client_id}&client_secret={client_secret}&redirect_uri={redirect_uri}", "");
            _logger.LogDebug("GetAccessToken 返回参数：" + response);
            return JsonSerializer.Deserialize<GiteeAccessToken>(response);
        }

        public GiteeUserInfo GetGiteeUserInfo(GiteeAccessToken model)
        {
            _logger.LogDebug("GetGiteeUserInfo 请求参数：" + model.access_token);
            var response = HttpUtil.Get("https://gitee.com/api/v5/user?access_token=" + model.access_token);
            var data = JsonSerializer.Deserialize<GiteeUserInfo>(response);
            _logger.LogDebug("GetGiteeUserInfo 返回参数：" + response);
            return data;
        }

        /// <summary>
        /// 下载头像
        /// </summary>
        /// <param name="url"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        public string DownloadImage(string url, string objectid)
        {
            var result = HttpUtil.DownloadImage(url, out var contentType);
            using (var stream = result.ToStream())
            {
                var hash_code = SHAUtil.GetFileSHA1(stream);

                var config = StoreConfig.Config;
                var id = Guid.NewGuid().ToString();
                var fileName = $"{EntityCommon.GenerateGuidNumber()}.png";
                ServiceFactory.Resolve<IStoreStrategy>(config?.Type).Upload(stream, fileName, out var filePath);

                var data = new SysFile()
                {
                    Id = id,
                    Name = fileName,
                    RealName = fileName,
                    HashCode = hash_code,
                    FileType = "avatar",
                    ContentType = contentType,
                    ObjectId = objectid
                };

                return _manager.Create(data);
            }
        }
    }
}
