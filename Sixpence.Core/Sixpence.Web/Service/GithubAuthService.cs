using Newtonsoft.Json;
using Sixpence.Common;
using Sixpence.Common.Http;
using System;
using System.Collections.Generic;
using System.Text;
using Sixpence.Web.Model.Github;
using Sixpence.Web.Entity;
using Sixpence.Common.Crypto;
using Sixpence.Common.Extensions;
using Sixpence.ORM;
using Microsoft.Extensions.Logging;
using Sixpence.ORM.Entity;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Sixpence.Web.Service
{
    public class GithubAuthService : BaseService
    {
        #region 构造函数
        public GithubAuthService() : base() { }

        public GithubAuthService(IEntityManager manager) : base(manager) { }
        #endregion

        public GithubConfig GetConfig()
        {
            var configService = new SysConfigService(_manager);
            var data = configService.GetValue("github_oauth")?.ToString();
            return JsonConvert.DeserializeObject<GithubConfig>(data);
        }

        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="code">临时码，有效期十分钟</param>
        public async Task<GithubAccessToken> GetAccessToken(string code)
        {
            var config = GetConfig();
            var clientId = config.client_id;
            var clientSecret = config.client_secret;

            var param = new
            {
                client_id = clientId,
                client_secret = clientSecret,
                code
            };
            var paramString = JsonConvert.SerializeObject(param);
            _logger.LogDebug("GetAccessToken 请求入参：" + paramString);
            var response = await HttpHelper.PostAsync("https://github.com/login/oauth/access_token", param);
            var result = await response.Content.ReadAsStringAsync();
            _logger.LogDebug("GetAccessToken 返回参数：" + result);
            var data = new GithubAccessToken();
            var arr = result.Split("&");
            foreach (var item in arr)
            {
                var key = item.Split("=")[0];
                var value = item.Split("=")[1];
                data.GetType().GetProperty(key).SetValue(data, value);
            }
            return data;
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<GithubUserInfo> GetUserInfo(GithubAccessToken model)
        {
            var token = $"{model.token_type} {model.access_token}";
            _logger.LogDebug("GetUserInfo 请求头：" + token);
            var headers = new Dictionary<string, string> { { "Authorization", token } };
            var response = await HttpHelper.GetAsync("https://api.github.com/user", bearerToken: token);
            var data = await response.Content.ReadFromJsonAsync<GithubUserInfo>();
            _logger.LogDebug("GetUserInfo 返回参数：" + response);
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
            var response = await new HttpClient().GetAsync(url);
            var stream = await response.Content.ReadAsStreamAsync();
            var contentType = response.Content.Headers.ContentType.ToString();
            var hash_code = SHAUtil.GetFileSHA1(stream);

            var id = Guid.NewGuid().ToString();
            var fileName = $"{EntityCommon.GenerateGuidNumber()}.jpg";
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
