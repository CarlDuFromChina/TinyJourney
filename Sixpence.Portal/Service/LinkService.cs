using Sixpence.EntityFramework;
using Sixpence.PortalEntity;
using Sixpence.Web;

namespace Sixpence.PortalService
{
    public class LinkService : EntityService<Link>
    {
        #region 构造函数
        public LinkService() : base() { }
        public LinkService(IEntityManager manager) : base(manager) { }
        #endregion
    }
}
