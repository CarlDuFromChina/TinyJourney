using Sixpence.Web.Module.SysMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sixpence.TinyJourney.Entity;
using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Entity;

namespace Sixpence.TinyJourney.Plugin
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
                var parentMenu = manager.QueryFirst<SysMenu>(new { router = "blog" });
                menu = new SysMenu()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = data.Name,
                    ParentId = parentMenu.Id,
                    ParentName = parentMenu.Name,
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
