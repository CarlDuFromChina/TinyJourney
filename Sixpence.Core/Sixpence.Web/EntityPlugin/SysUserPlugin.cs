using Sixpence.Web.Config;
using Sixpence.EntityFramework.Entity;
using Sixpence.Common;
using Sixpence.Common.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sixpence.Web.Service;
using Sixpence.Web.Entity;
using Sixpence.EntityFramework;

namespace Sixpence.Web.EntityPlugin
{
    public class SysUserPlugin : IEntityManagerPlugin
    {
        private readonly SysAuthUserService _sysAuthUserService;
        private readonly SysRoleService _sysRoleService;
        public SysUserPlugin(SysAuthUserService sysAuthUserService, SysRoleService sysRoleService)
        {
            _sysAuthUserService = sysAuthUserService;
            _sysRoleService = sysRoleService;
        }

        public void Execute(EntityManagerPluginContext context)
        {
            var entity = context.Entity as SysUser;
            switch (context.Action)
            {
                case EntityAction.PreCreate:
                case EntityAction.PreUpdate:
                    CheckUserInfo(entity, context.EntityManager);
                    break;
                case EntityAction.PostCreate:
                    CreateAuthInfo(entity, context.EntityManager);
                    break;
                case EntityAction.PostUpdate:
                    UpdateAuthInfo(entity, context.EntityManager);
                    break;
                case EntityAction.PostDelete:
                    DeleteAuthInfo(entity, context.EntityManager);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 创建用户认证信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="manager"></param>
        private void CreateAuthInfo(SysUser entity, IEntityManager manager)
        {
            var authInfo = new SysAuthUser()
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                Password = SystemConfig.Config.DefaultPassword,
                UserId = entity.Id,
                RoleId = entity.RoleId,
                RoleName = entity.RoleName,
                IsLock = false,
            };
            _sysAuthUserService.CreateData(authInfo);
        }

        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="manager"></param>
        private void UpdateAuthInfo(SysUser entity, IEntityManager manager)
        {
            var authInfo = manager.QueryFirst<SysAuthUser>(new { user_id = entity.Id });
            AssertUtil.IsNull(authInfo, "用户Id不能为空");
            authInfo.Name = entity.Name;
            authInfo.RoleId = entity.RoleId;
            authInfo.RoleName = entity.RoleName;
            _sysAuthUserService.UpdateData(authInfo);
        }

        /// <summary>
        /// 检查用户信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="broker"></param>
        private void CheckUserInfo(SysUser entity, IEntityManager manager)
        {
            var allowUpdateRole = _sysRoleService.AllowCreateOrUpdateRole(entity.RoleId);
            AssertUtil.IsTrue(!allowUpdateRole, $"你没有权限修改角色为[{entity.RoleName}]");
            AssertUtil.IsTrue(entity.PrimaryColumn.Value?.ToString() == "00000000-0000-0000-0000-000000000000", "系统管理员信息禁止更新");
        }

        /// <summary>
        /// 删除用户认证信息
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="broker"></param>
        private void DeleteAuthInfo(SysUser entity, IEntityManager manager)
        {
            manager.Delete<SysAuthUser>(new { user_id = entity.Id });
        }
    }
}