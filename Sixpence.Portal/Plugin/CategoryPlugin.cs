using Sixpence.Web.Module.SysMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixpence.PortalEntity;
using Sixpence.ORM;
using Sixpence.ORM.Entity;

namespace Sixpence.PortalPlugin
{
    public class CategoryPlugin : IEntityManagerPlugin
    {
        public void Execute(EntityManagerPluginContext context)
        {
            var data = context.Entity as Category;
            switch (context.Action)
            {
                case EntityAction.PostCreate:
                case EntityAction.PostUpdate:
                    CreateOrUpdateMenu(context.EntityManager, data);
                    break;
                case EntityAction.PostDelete:
                    {
                        var menu = context.EntityManager.QueryFirst<SysMenu>("SELECT * FROM sys_menu WHERE router = @code", new { code = $"post/{data.Code}" });
                        if (menu != null)
                        {
                            context.EntityManager.Delete(menu);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private void CreateOrUpdateMenu(IEntityManager manager, Category data)
        {
            var menu = manager.QueryFirst<SysMenu>("SELECT * FROM sys_menu WHERE router = @code", new Dictionary<string, object>() { { "@code", $"post/{data.Code}" } });
            if (menu != null)
            {
                menu.MenuIndex = data.Index;
                menu.Name = data.Name;
                manager.Update(menu);
            }
            else
            {
                menu = new SysMenu()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = data.Name,
                    ParentId = "8201EFED-76E2-4CD1-A522-4803D52D4D92",
                    ParentName = "博客管理",
                    Router = $"post/{data.Code}",
                    MenuIndex = data.Index,
                    IsEnable = true,
                    IsEnableName = "启用"
                };
                manager.Create(menu);
            }
        }
    }
}
