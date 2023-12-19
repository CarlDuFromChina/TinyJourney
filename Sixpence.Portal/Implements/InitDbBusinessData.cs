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
            var workspace = Guid.NewGuid().ToString();

            return new List<SysMenu>()
            {
                new SysMenu() { Id = dashboard, Name = "分析中心", Router = "dashboard", Icon = "dashboard", MenuIndex = 100, IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "工作空间", Router = "workplace", Icon = "", MenuIndex = 105, ParentId = dashboard, ParentName = "分析中心", IsEnable = true},
                new SysMenu() { Id = blog, Name = "博客管理", Router = "blog", Icon = "book", MenuIndex = 200, IsEnable = true },
                new SysMenu() { Id = workspace, Name = "创作平台", Router = "workspace", Icon = "container", MenuIndex = 300, IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "系列管理", Router = "series", Icon = "", MenuIndex = 305, ParentId = workspace, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "文章管理", Router = "post", Icon = "", MenuIndex = 310, ParentId = workspace, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "专栏管理", Router = "category", Icon = "", MenuIndex = 315, ParentId = workspace, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "草稿管理", Router = "drafts", Icon = "", MenuIndex = 320, ParentId = workspace, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "想法管理", Router = "idea", Icon = "", MenuIndex = 325, ParentId = workspace, ParentName = "创作平台", IsEnable = true },
                new SysMenu() { Id = Guid.NewGuid().ToString(), Name = "链接信息", Router = "link", Icon = "", MenuIndex = 330, ParentId = workspace, ParentName = "创作平台", IsEnable = true },
            };
        }

        public List<SysUser> GetSysUsers()
        {
            return new List<SysUser>();
        }
    }
}
