using Sixpence.Common.Current;
using Sixpence.Web.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Extensions
{
    public static class AuthUserExtension
    {
        public static CurrentUserModel ToCurrentUserModel(this SysAuthUser user)
        {
            return new CurrentUserModel()
            {
                Code = user.Code,
                Id = user.UserId,
                Name = user.Name
            };
        }
    }
}
