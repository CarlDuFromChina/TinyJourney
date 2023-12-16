using Sixpence.Common;
using Sixpence.Common.Utils;
using System.Collections.Generic;
using Sixpence.ORM.Repository;
using System.Linq;
using Sixpence.Web.Model;
using Sixpence.Web.Auth;
using Sixpence.Web.Entity;
using Sixpence.ORM;

namespace Sixpence.Web.Service
{
    public class SysUserService : EntityService<SysUser>
    {
        #region 构造函数
        public SysUserService() : base() { }

        public SysUserService(IEntityManager manager) : base(manager) { }
        #endregion

        public override IList<EntityView> GetViewList()
        {
            var sql = @"
SELECT
    is_lock,
	user_info.*
FROM
	sys_user
LEFT JOIN (
    SELECT
        user_infoid,
        is_lock
    FROM auth_user
) au ON user_info.id = au.user_infoid
";
            var customFilter = new List<string>() { "name" };
            return new List<EntityView>()
            {
                new EntityView()
                {
                    Sql = sql,
                    CustomFilter = customFilter,
                    OrderBy = "name, created_at",
                    ViewId = "59F908EB-A353-4205-ABE4-FA9DB27DD434",
                    Name = "所有的用户信息"
                }
            };
        }

        public SysUser GetData()
        {
            return Repository.FindOne(UserIdentityUtil.GetCurrentUserId());
        }

        /// <summary>
        /// 通过Code查询用户信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public SysUser GetDataByCode(string code)
        {
            return Repository.FindOne(new Dictionary<string, object>() { { "code", code } });
        }

        /// <summary>
        /// 是否需要填充信息
        /// </summary>
        /// <returns></returns>
        public bool InfoFilled()
        {
            var user = Repository.FindOne(UserIdentityUtil.GetCurrentUserId());
            AssertUtil.IsNull(user, "未查询到用户");
            if (user.Id == UserIdentityUtil.ADMIN_ID)
            {
                return false;
            }
            return !user.Gender.HasValue || CheckEmpty(user.Mailbox, user.Cellphone, user.Realname);
        }

        private bool CheckEmpty(params string[] args)
        {
            return args.Any(item => string.IsNullOrEmpty(item));
        }
    }
}
