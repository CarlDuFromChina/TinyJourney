using Microsoft.Extensions.DependencyInjection;
using Sixpence.Common;
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
        private readonly IEnumerable<IRole> _roles;

        public SysRolePrivilegePlugin(IServiceProvider provider)
        {
            _roles = provider.GetServices<IRole>();
        }

        public void Execute(EntityManagerPluginContext context)
        {
            switch (context.Action)
            {
                case EntityAction.PostCreate:
                case EntityAction.PostDelete:
                case EntityAction.PostUpdate:
                    UserPrivilegesCache.Clear();
                    _roles.Each(e => e.RebuildCache());
                    break;
                default:
                    break;
            }
        }
    }
}
