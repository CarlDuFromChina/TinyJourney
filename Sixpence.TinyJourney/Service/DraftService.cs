using Sixpence.TinyJourney.Entity;
using Sixpence.EntityFramework.Entity;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.EntityFramework;
using Sixpence.Web;

namespace Sixpence.TinyJourney.Service
{
    public class DraftService : EntityService<Draft>
    {
        public DraftService(IEntityManager manager, ILogger<EntityService<Draft>> logger, IRepository<Draft> repository) : base(manager, logger, repository)
        {
        }

        public IList<Draft> GetDrafts()
        {
            var sql = @"
SELECT * FROM draft
WHERE post_id NOT IN (
	SELECT post_id FROM post
)
";
            return _manager.Query<Draft>(sql).ToList();
        }

        /// <summary>
        /// 根据博客id获取草稿
        /// </summary>
        /// <param name="post_id"></param>
        /// <returns></returns>
        public Draft GetDataByPostId(string post_id) =>
            _repository.FindOne(new { post_id });

        /// <summary>
        /// 根据博客id删除草稿
        /// </summary>
        /// <param name="post_id"></param>
        public void DeleteDataByPostId(string post_id) =>
            _manager.Delete<Draft>(new { post_id });

        public override string CreateOrUpdateData(Draft t)
        {
            var data = _repository.FindOne(t.Id);
            if (data != null)
            {
                t.CreatedBy = data.CreatedBy;
                t.CreatedByName = data.CreatedByName;
                t.CreatedAt = data.CreatedAt;
                _repository.Update(t);
            }
            else
            {
                return _repository.Create(t);
            }
            return t.Id;
        }
    }
}
