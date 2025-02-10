using Sixpence.Web.Auth;
using Sixpence.Web.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Sixpence.Web.Model;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;
using Microsoft.Extensions.Logging;

namespace Sixpence.Web.Service
{
    public class SysRoleService : EntityService<SysRole>
    {
        public SysRoleService(IEntityManager manager, ILogger<EntityService<SysRole>> logger, IRepository<SysRole> repository) : base(manager, logger, repository)
        {
        }

        public IEnumerable<SelectOption> GetBasicRole()
        {
            var roles = _repository.GetAllEntity();
            var currentRoleId = _manager.QueryFirst<SysUser>(UserIdentityUtil.GetCurrentUserId())?.RoleId;
            var role = roles.FirstOrDefault(item => item.Id == currentRoleId);

            return roles.Where(item => UserIdentityUtil.IsOwner(role.IsBasic.Value ? role.Id : role.InheritedRoleId, item.IsBasic.Value ? item.Id : item.InheritedRoleId))
                .Where(item => item.IsBasic.Value)
                .Select(item => new SelectOption(item.Name, item.Id));
        }


        public IEnumerable<SelectOption> GetRoles()
        {
            var roles = _repository.GetAllEntity();
            var currentRoleId = _manager.QueryFirst<SysUser>(UserIdentityUtil.GetCurrentUserId())?.RoleId;
            var role = roles.FirstOrDefault(item => item.Id == currentRoleId);

            return roles.Where(item => UserIdentityUtil.IsOwner(role.IsBasic.Value ? role.Id : role.InheritedRoleId, item.IsBasic.Value ? item.Id : item.InheritedRoleId))
                .Select(item => new SelectOption(item.Name, item.Id));
        }

        public SysRole GetGuest() => _manager.QueryFirst<SysRole>("222222222-22222-2222-2222-222222222222");

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
                    curRoleId = _manager.QueryFirst<SysUser>(UserIdentityUtil.GetCurrentUserId())?.RoleId;
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
