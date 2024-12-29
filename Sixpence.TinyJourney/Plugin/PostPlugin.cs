using Sixpence.TinyJourney.Service;
using Sixpence.Common.Utils;
using Sixpence.EntityFramework;
using Sixpence.TinyJourney.Entity;

namespace Sixpence.TinyJourney.Plugin
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
                    if (post.IsPop == null)
                    {
                        post.IsPop = false;
                    }
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
