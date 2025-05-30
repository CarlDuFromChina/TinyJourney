﻿using Jdenticon.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Sixpence.Common.Crypto;
using Sixpence.Common.Current;
using Sixpence.Common.Http;
using Sixpence.Common.Utils;
using Sixpence.EntityFramework;
using Sixpence.Web.Auth;
using Sixpence.Web.Config;
using Sixpence.Web.Entity;
using Sixpence.Web.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sixpence.Web.Service
{
    public class SystemService : BaseService<SystemService>
    {
        private readonly MailVertificationService _mailVertificationService;
        private readonly IStorage _storage;
        private readonly Lazy<IEnumerable<IThirdPartyLoginStrategy>> _thirdPartyLoginStrategies;
        public SystemService(IEntityManager manager, ILogger<SystemService> logger, MailVertificationService mailVertificationService, IStorage storage, IServiceProvider provider) : base(manager, logger)
        {
            _mailVertificationService = mailVertificationService;
            _storage = storage;
            _thirdPartyLoginStrategies = new Lazy<IEnumerable<IThirdPartyLoginStrategy>>(() => provider.GetServices<IThirdPartyLoginStrategy>());
        }

        /// <summary>
        /// 获取公钥
        /// </summary>
        /// <returns></returns>
        public string GetPublicKey()
        {
            return RSAUtil.GetKey();
        }

        /// <summary>
        /// 获取随机图片
        /// </summary>
        /// <returns></returns>
        public string GetRandomImage()
        {
            var result = HttpHelper.GetAsync("https://api.ixiaowai.cn/api/api.php?return=json").Result;
            return result.Content.ReadAsStringAsync().Result;
        }

        /// <summary>
        /// 获取头像
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetAvatar(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            var user = _manager.QueryFirst<SysUser>(id);
            if (!string.IsNullOrEmpty(user?.Avatar))
            {
                return _storage.DownloadAsync(user.Avatar).Result;
            }
            return IdenticonResult.FromValue(id, 64);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public LoginResponse Login(LoginRequest model)
        {
            UserIdentityUtil.SetCurrentUser(UserIdentityUtil.GetSystem());

            // 联合第三方登录
            if (model.ThirdPartyLogin != null)
            {
                var loginStrategy = _thirdPartyLoginStrategies.Value
                    .First(item => item.GetName().Equals(model.ThirdPartyLogin.Type, StringComparison.OrdinalIgnoreCase));
                AssertUtil.IsNull(loginStrategy, $"根据{model.ThirdPartyLogin.Type}未找到登录策略");
                return loginStrategy.Login(model.ThirdPartyLogin.Param);
            }

            var code = model.Code;
            var pwd = model.Password;
            var publicKey = model.PublicKey;

            var user = _manager.QueryFirst<SysUser>(new { code });
            if (user == null)
            {
                return new LoginResponse() { result = false, message = "用户名或密码错误" };
            }

            if (user.isActive == false)
            {
                return new LoginResponse() { result = false, message = "用户已被锁定，请联系管理员" };
            }

            var authUser = _manager.QueryFirst<SysAuthUser>(new { code });

            if (authUser == null)
            {
                return new LoginResponse() { result = false, message = "用户名或密码错误" };
            }

            if (authUser.IsLock.Value)
            {
                return new LoginResponse() { result = false, message = "用户已被锁定，请联系管理员" };
            }

            if (string.IsNullOrEmpty(pwd) ||
                string.IsNullOrEmpty(publicKey) ||
                !string.Equals(authUser.Password, RSAUtil.Decrypt(pwd, publicKey))
                )
            {
                var message = "用户名或密码错误";
                if (!authUser.TryTimes.HasValue)
                {
                    authUser.TryTimes = 1;
                }
                else
                {
                    authUser.TryTimes += 1;
                    if (authUser.TryTimes > 1)
                    {
                        message = $"用户名或密码已连续错误{authUser.TryTimes}次，超过五次账号锁定";
                    }
                }

                if (authUser.TryTimes >= 5)
                {
                    authUser.IsLock = true;
                    message = $"用户已被锁定，请联系管理员";
                }

                _manager.Update(authUser);
                return new LoginResponse() { result = false, message = message };
            }

            if (authUser.TryTimes > 0)
            {
                authUser.TryTimes = 0;
            }
            authUser.LastLoginTime = DateTime.Now;
            _manager.Update(authUser);

            // 返回登录结果、用户信息、用户验证票据信息
            var oUser = new LoginResponse
            {
                result = true,
                userName = code,
                token = JwtHelper.CreateToken(new JwtTokenModel() { Code = authUser.Code, Name = authUser.Name, Role = authUser.Code, Uid = authUser.Id }),
                userId = authUser.UserId,
                message = "登录成功"
            };
            return oUser;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public LoginResponse Signup(LoginRequest model)
        {
            AssertUtil.IsNullOrEmpty(model.Code, "账号不能为空");
            AssertUtil.IsNullOrEmpty(model.Password, "密码不能为空");

            return _manager.ExecuteTransaction(() =>
            {
                if (!model.Code.Contains("@"))
                    return new LoginResponse(false, "注册失败，请使用邮箱作为账号");

                var vertification = _mailVertificationService.GetDataByMailAdress(model.Code);
                if (vertification != null)
                    return new LoginResponse(false, "激活邮件已发送，请前往邮件激活账号，请勿重复注册", LoginMesageLevel.Warning);

                var id = Guid.NewGuid().ToString();
                model.Password = RSAUtil.Decrypt(model.Password, model.PublicKey);
                var data = new MailVertification()
                {
                    Id = id,
                    Name = "账号激活邮件",
                    Content = $@"你好,<br/><br/>
请在两小时内点击该<a href=""{SystemConfig.Config.Protocol}://{SystemConfig.Config.Domain}/api/mail_vertification/ActivateUser?id={id}"">链接</a>激活，失效请重新登录注册
",
                    ExpireTime = DateTime.Now.AddHours(2),
                    IsActive = false,
                    LoginRequest = JsonConvert.SerializeObject(model),
                    MailAddress = model.Code,
                    MailType = MailType.Activation.ToString()
                };
                _manager.Create(data);

                // 返回登录结果、用户信息、用户验证票据信息
                return new LoginResponse()
                {
                    result = false,
                    message = $"已向{data.MailAddress}发送激活邮件，请在两个小时内激活",
                    level = LoginMesageLevel.Warning.ToString()
                };
            });
        }

        /// <summary>
        /// 登录或注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public LoginResponse SignInOrSignUp(LoginRequest model)
        {
            var resp = Login(model);
            if (resp.result)
                return resp;

            return Signup(model);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="password"></param>
        public void EditPassword(string password)
        {
            var sql = $@"
UPDATE sys_auth_user
SET password = @password
WHERE user_id = @id;
";
            var user = UserIdentityUtil.GetCurrentUser();
            var paramList = new Dictionary<string, object>() { { "@id", user.Id }, { "@password", password } };
            _manager.Execute(sql, paramList);
        }

        /// <summary>
        /// 充值密码
        /// </summary>
        /// <param name="id"></param>
        public void ResetPassword(string id)
        {
            var sql = $@"
UPDATE sys_auth_user
SET password = @password
WHERE user_id = @id;
";
            var paramList = new Dictionary<string, object>() { { "@id", id }, { "@password", SystemConfig.Config.DefaultPassword } };
            _manager.Execute(sql, paramList);
        }

        /// <summary>
        /// 忘记密码
        /// </summary>
        /// <param name="code"></param>
        public void ForgetPassword(string code)
        {
            var user = _manager.QueryFirst<SysUser>("SELECT * FROM user_info WHERE code = @mail OR mailbox = @mail", new Dictionary<string, object>() { { "@mail", code } });
            AssertUtil.IsNull(user, "用户不存在");
            UserIdentityUtil.SetCurrentUser(new CurrentUserModel() { Id = user.Id, Code = user.Code, Name = user.Name });
            var id = Guid.NewGuid().ToString();
            var sms = new MailVertification()
            {
                Id = id,
                Name = "重置密码",
                Content = $@"你好,<br/><br/>
请在两小时内点击该<a href=""{SystemConfig.Config.Protocol}://{SystemConfig.Config.Domain}/api/mail_vertification/ResetPassword?id={id}"">链接</a>重置密码
",
                ExpireTime = DateTime.Now.AddHours(2),
                IsActive = false,
                MailAddress = user.Mailbox,
                MailType = MailType.ResetPassword.ToString()
            };
            _manager.Create(sms);
        }

        /// <summary>
        /// 是否有进入后台权限
        /// </summary>
        /// <returns></returns>
        public bool GetShowAdmin()
        {
            var userId = UserIdentityUtil.GetCurrentUserId();
            if (string.IsNullOrEmpty(userId))
                return false;

            var user = _manager.QueryFirst<SysUser>(userId);
            if (user == null)
                return false;

            if (user.RoleId != UserIdentityUtil.ANONYMOUS_ID)
                return true;

            return false;
        }
    }
}