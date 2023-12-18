using Sixpence.Web;
using Sixpence.Web.Entity;
using Sixpence.Web.Module.SysMenu;
using System.ComponentModel;

namespace Sixpence.Portal.Implements
{
    public class InitDbBusinessData : IInitDbData
    {
        public List<SysAuthUser> GetSysAuthUsers()
        {
            return new List<SysAuthUser>();
        }

        public List<SysMenu> GetSysMenus()
        {
            var dashboard = Guid.NewGuid().ToString();
            var blog = Guid.NewGuid().ToString();
            var container = Guid.NewGuid().ToString();

            return new List<SysMenu>()
            {
                new SysMenu() { Id = dashboard, Name = "分析中心", Router = "", Icon = "dashboard", MenuIndex = 100, IsEnable = true },
                new SysMenu() {Id = Guid.NewGuid().ToString(), Name = "工作空间", Router = "workplace", Icon = "", MenuIndex = 105, ParentId = dashboard, ParentName = "分析中心", IsEnable = true},
                new SysMenu() { Id = blog, Name = "博客管理", Router = "", Icon = "book", MenuIndex = 200, IsEnable = true },
                new SysMenu() { Id = container, Name = "创作平台", Router = "", Icon = "container", MenuIndex = 300, IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "系列管理", Router = "series", Icon = "", MenuIndex = 305, ParentId = container, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "文章管理", Router = "post", Icon = "", MenuIndex = 310, ParentId = container, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "专栏管理", Router = "category", Icon = "", MenuIndex = 315, ParentId = container, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "草稿管理", Router = "drafts", Icon = "", MenuIndex = 320, ParentId = container, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "想法管理", Router = "idea", Icon = "", MenuIndex = 325, ParentId = container, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "链接信息", Router = "link", Icon = "", MenuIndex = 330, ParentId = container, ParentName = "创作平台", IsEnable = true },
            };
        }

        public List<SysUser> GetSysUsers()
        {
            return new List<SysUser>();
        }
    }
}
