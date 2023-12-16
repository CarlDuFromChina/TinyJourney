using Sixpence.PortalEntity;
using Sixpence.ORM.Entity;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.ORM;
using Sixpence.Web;

namespace Sixpence.PortalService
{
    public class DraftService : EntityService<Draft>
    {
        #region 构造函数
        public DraftService() : base() { }

        public DraftService(IEntityManager manager) : base(manager) { }
        #endregion

        public IList<Draft> GetDrafts()
        {
            var sql = @"
SELECT * FROM draft
WHERE post_id NOT IN (
	SELECT post_id FROM post
)
";
            return Manager.Query<Draft>(sql).ToList();
        }

        /// <summary>
        /// 根据博客id获取草稿
        /// </summary>
        /// <param name="post_id"></param>
        /// <returns></returns>
        public Draft GetDataByPostId(string post_id)
        {
            var sql = @"
SELECT * FROM draft
WHERE post_id = @post_id
";
            return Manager.QueryFirst<Draft>(sql, new Dictionary<string, object>() { { "@post_id", post_id } });
        }

        /// <summary>
        /// 根据博客id删除草稿
        /// </summary>
        /// <param name="post_id"></param>
        public void DeleteDataByPostId(string post_id)
        {
            var draft = Manager.QueryFirst<Draft>("select * from draft where post_id = @id or id = @id", new { id = post_id });
            if (draft != null)
                Manager.Delete(draft);
        }

        public override string CreateOrUpdateData(Draft t)
        {
            var data = Repository.FindOne(t.Id);
            if (data != null)
            {
                t.CreatedBy = data.CreatedBy;
                t.CreatedByName = data.CreatedByName;
                t.CreatedAt = data.CreatedAt;
                Repository.Update(t);
            }
            else
            {
                return Repository.Create(t);
            }
            return t.Id;
        }
    }
}
