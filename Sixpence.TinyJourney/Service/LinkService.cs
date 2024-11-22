using Sixpence.EntityFramework;
using Sixpence.TinyJourney.Entity;
using Sixpence.Web;

namespace Sixpence.TinyJourney.Service
{
    public class LinkService : EntityService<Link>
    {
        #region 构造函数
        public LinkService() : base() { }
        public LinkService(IEntityManager manager) : base(manager) { }
        #endregion
    }
}
