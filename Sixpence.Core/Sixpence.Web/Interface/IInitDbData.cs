using Sixpence.Web.Entity;
using Sixpence.Web.Module.SysMenu;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.Web
{
    public interface IInitDbData
    {
        /// <summary>
        /// 初始化系统角色
        /// </summary>
        /// <returns></returns>
        List<SysAuthUser> GetSysAuthUsers();

        /// <summary>
        /// 初始化系统用户
        /// </summary>
        /// <returns></returns>
        List<SysUser> GetSysUsers();

        /// <summary>
        /// 初始化系统菜单
        /// </summary>
        /// <returns></returns>
        List<SysMenu> GetSysMenus();
    }
}
