using Sixpence.PortalService;
using Sixpence.Common.Utils;
using Sixpence.ORM;
using Sixpence.PortalEntity;

namespace Sixpence.PortalPlugin
{
    public class PostPlugin : IEntityManagerPlugin
    {
        public void Execute(EntityManagerPluginContext context)
        {
            var post = context.Entity as Post;
            switch (context.Action)
            {
                case EntityAction.PreCreate:
                case EntityAction.PreUpdate:
                    post.IsShowName = post.IsShow == true ? "是" : "否";
                    post.IsSeriesName = post.IsSeries == true ? "是" : "否";
                    post.IsPopName = post.IsPop == true ? "是" : "否";
                    break;
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
