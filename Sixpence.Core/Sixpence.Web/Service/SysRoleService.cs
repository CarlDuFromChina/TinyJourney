using Sixpence.Web.Auth;
using Sixpence.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;

namespace Sixpence.Web.Service
{
    public class SysRoleService : EntityService<SysRole>
    {
        #region 构造函数
        public SysRoleService() : base() { }

        public SysRoleService(IEntityManager manager) : base(manager) { }
        #endregion

        public IEnumerable<SelectOption> GetBasicRole()
        {
            var roles = Repository.GetAllEntity();
            var currentRoleId = Manager.QueryFirst<SysUser>(UserIdentityUtil.GetCurrentUserId())?.RoleId;
            var role = roles.FirstOrDefault(item => item.Id == currentRoleId);

            return roles.Where(item => UserIdentityUtil.IsOwner(role.IsBasic.Value ? role.Id : role.InheritedRoleId, item.IsBasic.Value ? item.Id : item.InheritedRoleId))
                .Where(item => item.IsBasic.Value)
                .Select(item => new SelectOption(item.Name, item.Id));
        }


        public IEnumerable<SelectOption> GetRoles()
        {
            var roles = Repository.GetAllEntity();
            var currentRoleId = Manager.QueryFirst<SysUser>(UserIdentityUtil.GetCurrentUserId())?.RoleId;
            var role = roles.FirstOrDefault(item => item.Id == currentRoleId);

            return roles.Where(item => UserIdentityUtil.IsOwner(role.IsBasic.Value ? role.Id : role.InheritedRoleId, item.IsBasic.Value ? item.Id : item.InheritedRoleId))
                .Select(item => new SelectOption(item.Name, item.Id));
        }

        public SysRole GetGuest() => Manager.QueryFirst<SysRole>("222222222-22222-2222-2222-222222222222");

        public bool AllowCreateOrUpdateRole(string roleid)
        {
            var curid = UserIdentityUtil.GetCurrentUserId();
            var curRoleId = string.Empty;

            switch (curid)
            {
                case UserIdentityUtil.SYSTEM_ID:
                    curRoleId = curid;
                    break;
                case UserIdentityUtil.ANONYMOUS_ID:
                    curRoleId = curid;
                    break;
                case UserIdentityUtil.ADMIN_ID:
                    curRoleId = curid;
                    break;
                case UserIdentityUtil.USER_ID:
                    curRoleId = curid;
                    break;
                default:
                    curRoleId = Manager.QueryFirst<SysUser>(UserIdentityUtil.GetCurrentUserId())?.RoleId;
                    break;
            }

            if (string.IsNullOrEmpty(curRoleId))
            {
                return false;
            }
            var toRoleId = roleid;

            return Convert.ToInt32(toRoleId.FirstOrDefault().ToString()) >= Convert.ToInt32(curRoleId.FirstOrDefault().ToString());
        }
    }
}
