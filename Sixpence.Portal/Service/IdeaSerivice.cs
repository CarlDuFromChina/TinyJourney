using Sixpence.ORM;
using Sixpence.PortalEntity;
using Sixpence.Web;

namespace Sixpence.PortalService
{
    public class IdeaSerivice : EntityService<Idea>
    {
        #region 构造函数
        public IdeaSerivice() : base() { }

        public IdeaSerivice(IEntityManager manager) : base(manager) { }
        #endregion

    }
}
