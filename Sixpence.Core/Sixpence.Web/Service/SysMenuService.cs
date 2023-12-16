using Sixpence.Web.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sixpence.Web.Module.SysMenu;
using Sixpence.Web.Extensions;
using Sixpence.ORM;
using Sixpence.Web.Model;

namespace Sixpence.Web.Service
{
    public class SysMenuService : EntityService<SysMenu>
    {
        #region 构造函数
        public SysMenuService() : base() { }
        public SysMenuService(IEntityManager manager) : base(manager) { }
        #endregion

        public override IEnumerable<SysMenu> GetDataList(IList<SearchCondition> searchList, string orderBy, string viewId = "", string searchValue = "")
        {
            var data = base.GetDataList(searchList, orderBy, viewId).Filter().ToList();
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

        public override DataModel<SysMenu> GetDataList(IList<SearchCondition> searchList, string orderBy, int pageSize, int pageIndex, string viewId = "", string searchValue = "")
        {
            var model = base.GetDataList(searchList, orderBy, pageSize, pageIndex, viewId);
            var data = model.DataList.Filter().ToList();
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
                DataList = firstMenu,
                RecordCount = data.Count()
            };
        }

        public IList<SysMenu> GetFirstMenu()
        {
            var sql = @"
SELECT * FROM sys_menu
WHERE parent_id IS NULL OR parent_id = ''
ORDER BY menu_index
";
            var data = Manager.Query<SysMenu>(sql).ToList();
            return data;
        }
    }
}