using Sixpence.PortalEntity;
using Sixpence.ORM.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.ORM;
using Sixpence.Web;

namespace Sixpence.PortalService
{
    public class CategoryService : EntityService<Category>
    {
        #region 构造函数
        public CategoryService() : base() { }

        public CategoryService(IEntityManager manager) : base(manager) { }
        #endregion
    }
}
