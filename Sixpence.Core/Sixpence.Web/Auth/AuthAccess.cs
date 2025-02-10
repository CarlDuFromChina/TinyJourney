using Sixpence.EntityFramework;
using Sixpence.Web.Auth.Role;
using Sixpence.Web.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sixpence.Web.Auth
{
    /// <summary>
    /// 权限检查
    /// </summary>
    public static class AuthAccess
    {
        /// <summary>
        /// 检查权限
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="operationType"></param>
        /// <returns></returns>
        private static bool CheckAccess(IEntityManager manager, string objectid, OperationType operationType, string userId)
        {
            var data = UserPrivilegesCache.GetUserPrivileges(manager, string.IsNullOrEmpty(userId) ? UserIdentityUtil.GetCurrentUser()?.Id : userId)
                .Where(item => item.ObjectId == objectid)
                .FirstOrDefault();
            return data != null && (data.Privilege & (int)operationType) == (int)operationType;
        }

        /// <summary>
        /// 检查实体读权限
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool CheckReadAccess(IEntityManager manager, string objectid, string userId = "")
        {
            return CheckAccess(manager, objectid, OperationType.Read, userId);
        }

        /// <summary>
        /// 检查实体写权限
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool CheckWriteAccess(IEntityManager manager, string objectid, string userId = "")
        {
            return CheckAccess(manager, objectid, OperationType.Write, userId);
        }

        /// <summary>
        /// 检查实体删权限
        /// </summary>
        /// <param name="objectid"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static bool CheckDeleteAccess(IEntityManager manager, string objectid, string userId = "")
        {
            return CheckAccess(manager, objectid, OperationType.Delete, userId);
        }
    }
}
