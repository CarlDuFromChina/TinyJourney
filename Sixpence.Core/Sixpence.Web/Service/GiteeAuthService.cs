using Microsoft.Extensions.Logging;
using Sixpence.Common.Crypto;
using Sixpence.Common.Extensions;
using Sixpence.Common.Http;
using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Entity;
using Sixpence.Web.Config;
using Sixpence.Web.Entity;
using Sixpence.Web.Model.Gitee;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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

        public async Task<GiteeAccessToken> GetAccessToken(string code, string userid = "")
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
            var url = $"https://gitee.com/oauth/token?grant_type=authorization_code&code={code}&client_id={client_id}&client_secret={client_secret}&redirect_uri={redirect_uri}";
            var response = await new HttpClient().PostAsync(url, null);
            var result = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("GetAccessToken 返回参数：" + result);
            return JsonSerializer.Deserialize<GiteeAccessToken>(result);
        }

        public async Task<GiteeUserInfo> GetGiteeUserInfo(GiteeAccessToken model)
        {
            _logger.LogDebug("GetGiteeUserInfo 请求参数：" + model.access_token);
            var response = await new HttpClient().GetAsync("https://gitee.com/api/v5/user?access_token=" + model.access_token);
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<GiteeUserInfo>(result);
            _logger.LogDebug("GetGiteeUserInfo 返回参数：" + result);
            return data;
        }

        /// <summary>
        /// 下载头像
        /// </summary>
        /// <param name="url"></param>
        /// <param name="objectid"></param>
        /// <returns></returns>
        public async Task<string> DownloadImage(string url, string objectid)
        {
            var resp = await new HttpClient().GetAsync(url);
            resp.EnsureSuccessStatusCode();
            var contentType = resp.Content.Headers.ContentType.ToString();
            var result = await resp.Content.ReadAsByteArrayAsync();
            using (var stream = result.ToStream())
            {
                var hash_code = SHAUtil.GetFileSHA1(stream);

                var config = StoreConfig.Config;
                var id = Guid.NewGuid().ToString();
                var fileName = $"{EntityCommon.GenerateGuidNumber()}.png";
                AppContext.Storage.UploadAsync(stream, fileName).Wait();

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
