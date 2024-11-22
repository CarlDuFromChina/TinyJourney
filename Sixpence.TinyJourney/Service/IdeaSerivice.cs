using Sixpence.EntityFramework;
using Sixpence.TinyJourney.Entity;
using Sixpence.Web;

namespace Sixpence.TinyJourney.Service
{
    public class IdeaSerivice : EntityService<Idea>
    {
        #region 构造函数
        public IdeaSerivice() : base() { }

        public IdeaSerivice(IEntityManager manager) : base(manager) { }
        #endregion

    }
}
