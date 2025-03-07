﻿using Sixpence.Web.Config;
using Sixpence.Common;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.Web;
using System.Linq;
using Sixpence.Web.Entity;
using Sixpence.Web.Auth;
using Sixpence.EntityFramework;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Sixpence.Web.Service
{
    public class SysAuthUserService : EntityService<SysAuthUser>
    {
        private Lazy<IEnumerable<IThirdPartyBindStrategy>> _thirdPartyBindStrategies;
        public SysAuthUserService(IEntityManager manager, ILogger<EntityService<SysAuthUser>> logger, IRepository<SysAuthUser> repository, IServiceProvider serviceProvider) : base(manager, logger, repository)
        {
            _thirdPartyBindStrategies = new Lazy<IEnumerable<IThirdPartyBindStrategy>>(() => serviceProvider.GetServices<IThirdPartyBindStrategy>());
        }

        /// <summary>
        /// 获取用户登录信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pwd">MD5密码</param>
        /// <returns></returns>
        public SysAuthUser GetData(string code, string password)
            => _manager.QueryFirst<SysAuthUser>(new { code, password });

        public SysAuthUser GetDataByCode(string code)
            => _manager.QueryFirst<SysAuthUser>(new { code });

        public SysAuthUser GetDataByUserId(string userId)
            => _manager.QueryFirst<SysAuthUser>(new { user_id = userId });

        /// <summary>
        /// 锁定用户
        /// </summary>
        /// <param name="id"></param>
        public void LockUser(string id)
        {
            _manager.ExecuteTransaction(() =>
            {
                var userId = UserIdentityUtil.GetCurrentUserId();
                AssertUtil.IsTrue(userId == id, "请勿锁定自己");
                var data = GetDataByUserId(id);
                data.IsLock = true;
                UpdateData(data);
            });
        }

        /// <summary>
        /// 解锁用户
        /// </summary>
        /// <param name="id"></param>
        public void UnlockUser(string id)
        {
            _manager.ExecuteTransaction(() =>
            {
                var data = GetDataByUserId(id);
                data.IsLock = false;
                UpdateData(data);
            });
        }

        /// <summary>
        /// 绑定用户
        /// </summary>
        /// <param name="type"></param>
        /// <param name="id"></param>
        /// <param name="code"></param>
        public void BindThirdPartyAccount(string type, string id, string code)
        {
            AssertUtil.IsNullOrEmpty(id, "用户id不能为空");
            AssertUtil.IsNullOrEmpty(code, "编码不能为空");
            AssertUtil.IsNull(type, "绑定类型不能为空");
            _thirdPartyBindStrategies.Value.First(item => item.GetName().Equals(type, StringComparison.OrdinalIgnoreCase))?.Bind(code, id);
        }

        public void CreateMissingAuthUser(IEnumerable<SysAuthUser> users)
        {
            users.Each(user =>
            {
                var data = _repository.FindOne(new { code = user.Code });
                if (data == null)
                    _manager.Create(user, false);
            });
        }
    }
}