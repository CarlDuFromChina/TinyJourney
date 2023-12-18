using Sixpence.Web.Auth.Role;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web.Extensions
{
    public static class RoleExtensions
    {
        public static string GetIdentifier(this Role role)
        {
            switch (role)
            {
                case Role.Admin:
                    return "00000000-0000-0000-0000-000000000000";
                case Role.System:
                    return "11111111-1111-1111-1111-111111111111";
                case Role.User:
                    return "22222222-2222-2222-2222-222222222222";
                case Role.Guest:
                    return "33333333-3333-3333-3333-333333333333";
                default:
                    return Guid.NewGuid().ToString();
            }
        }
    }
}
