using Sixpence.ORM.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Sixpence.Web.Module.SysMenu
{
    [Table(Description: "系统菜单")]
    public partial class SysMenu : SormEntity
    {
        /// <summary>
        /// 实体id
        /// </summary>
        [PrimaryColumn]
        public string Id { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [Column, Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 上级菜单
        /// </summary>
        [Column, Description("上级菜单")]
        public string ParentId { get; set; }

        /// <summary>
        /// 上级菜单
        /// </summary>
        [Column, Description("上级菜单")]
        public string ParentName { get; set; }

        /// <summary>
        /// 路由地址
        /// </summary>
        [Column, Description("路由地址")]
        public string Router { get; set; }

        /// <summary>
        /// 菜单索引
        /// </summary>
        [Column, Description("菜单索引")]
        public int? MenuIndex { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [Column, Description("状态")]
        public bool? IsEnable { get; set; }

        /// <summary>
        /// 状态名称
        /// </summary>
        [Column, Description("状态名称")]
        public string IsEnableName { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        [Column, Description("图标")]
        public string Icon { get; set; }
    }

    public partial class SysMenu
    {
        public IList<SysMenu> Children { get; set; }
    }
}