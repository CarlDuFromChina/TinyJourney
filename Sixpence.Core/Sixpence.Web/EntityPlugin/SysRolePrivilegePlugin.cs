using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Entity;
using Sixpence.Web.Cache;
using Sixpence.Web.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sixpence.Web.EntityPlugin
{
    public class SysRolePrivilegePlugin : IEntityManagerPlugin
    {
        public void Execute(EntityManagerPluginContext context)
        {
            switch (context.Action)
            {
                case EntityAction.PostCreate:
                case EntityAction.PostDelete:
                case EntityAction.PostUpdate:
                    UserPrivilegesCache.Clear(context.EntityManager);
                    break;
                default:
                    break;
            }
        }
    }
}
