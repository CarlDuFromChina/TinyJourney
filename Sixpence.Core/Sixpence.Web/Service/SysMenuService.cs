using Sixpence.Web.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sixpence.Web.Module.SysMenu;
using Sixpence.Web.Extensions;
using Sixpence.EntityFramework;
using Sixpence.Web.Model;
using Sixpence.Common;
using Microsoft.Extensions.Logging;

namespace Sixpence.Web.Service
{
    public class SysMenuService : EntityService<SysMenu>
    {
        public SysMenuService(IEntityManager manager, ILogger<EntityService<SysMenu>> logger, IRepository<SysMenu> repository) : base(manager, logger, repository)
        {
        }

        public override IEnumerable<SysMenu> GetDataList(IList<SearchCondition> searchList, string viewId = "", string searchValue = "")
        {
            var data = base.GetDataList(searchList, viewId).Filter(_manager).ToList();
            var firstMenu = data
                .Where(e => string.IsNullOrEmpty(e.ParentId))
                .Select(item =>
                {
                    item.Children = data
                        .Where(e => e.ParentId == item.Id)
                        .OrderBy(e => e.MenuIndex)
                        .ToList();
                    return item;
                })
                .OrderBy(e => e.MenuIndex)
                .ToList();
            return firstMenu;
        }

        public override DataModel<SysMenu> GetDataList(IList<SearchCondition> searchList, int pageSize, int pageIndex, string viewId = "", string searchValue = "")
        {
            var model = base.GetDataList(searchList, pageSize, pageIndex, viewId);
            var data = model.Data.Filter(_manager).ToList();
            var firstMenu = data.Where(e => string.IsNullOrEmpty(e.ParentId)).ToList();
            firstMenu.ForEach(item =>
            {
                item.Children = new List<SysMenu>();
                data.ForEach(item2 =>
                {
                    if (item2.ParentId == item.Id)
                    {
                        item.Children.Add(item2);
                    }
                });
                item.Children = item.Children.OrderBy(e => e.MenuIndex).ToList();
            });
            firstMenu = firstMenu.OrderBy(e => e.MenuIndex).ToList();
            return new DataModel<SysMenu>()
            {
                Data = firstMenu,
                Count = data.Count()
            };
        }

        public IList<SysMenu> GetFirstMenu()
        {
            var sql = @"
SELECT * FROM sys_menu
WHERE parent_id IS NULL OR parent_id = ''
ORDER BY menu_index
";
            var data = _manager.Query<SysMenu>(sql).ToList();
            return data;
        }

        public void CreateMissingMenu(IEnumerable<SysMenu> menus)
        {
            foreach (var item in menus)
            {
                SysMenu data;

                if (!string.IsNullOrEmpty(item.Router))
                    data = _repository.FindOne(new { router = item.Router });
                else
                    data = _repository.FindOne(new { id = item.Id });

                if (data == null)
                    _repository.Create(item);
            }
        }
    }
}