using Sixpence.PortalService;
using Sixpence.Common.Utils;
using Sixpence.ORM;

namespace Sixpence.PortalPlugin
{
    public class PostPlugin : IEntityManagerPlugin
    {
        public void Execute(EntityManagerPluginContext context)
        {
            switch (context.Action)
            {
                case EntityAction.PostCreate:
                case EntityAction.PostUpdate:
                    var id = context.Entity.PrimaryColumn.Value?.ToString();
                    new DraftService(context.EntityManager).DeleteDataByPostId(id); // 删除草稿
                    MemoryCacheUtil.RemoveCacheItem(id);
                    break;
                default:
                    break;
            }
        }
    }
}
