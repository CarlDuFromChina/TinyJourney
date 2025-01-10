using Sixpence.AI;
using Sixpence.AI.DeepSeek;
using Sixpence.AI.Wenxin;
using Sixpence.Common;
using Sixpence.EntityFramework;
using Sixpence.TinyJourney.Config;
using Sixpence.TinyJourney.Entity;
using Sixpence.TinyJourney.Model;
using Sixpence.Web;
using Sixpence.Web.Entity;
using Sixpence.Web.Model;
using Sixpence.Web.Service;
using System.ComponentModel.Design;

namespace Sixpence.TinyJourney.Service
{
    public class PostService : EntityService<Post>
    {
        #region 构造函数
        public PostService() : base() { }

        public PostService(IEntityManager manager) : base(manager) { }
        #endregion

        public override IList<EntityView> GetViewList()
        {
            var sql = $@"
SELECT
	post.id,
	post.title,
	post.post_type,
	post.post_type_name,
	post.created_by,
	post.created_by_name,
	post.updated_by,
	post.updated_by_name,
	post.created_at,
	post.updated_at,
	post.is_series,
	post.tags,
	COALESCE(post.reading_times, 0) reading_times,
	post.surface_id,
	post.surface_url,
	post.brief,
	post.is_pop,
	post.is_pop_name
FROM
	post
WHERE 1=1 AND post.is_show = true";
            var orderBy = "post.is_pop DESC, post.created_at desc, post.title, post.id";
            return new List<EntityView>()
            {
                new EntityView()
                {
                    Sql = "SELECT * FROM post",
                    ViewId = "C94EDAAE-0C59-41E6-A373-D4816C2FD882",
                    CustomFilter = new List<string>(){ "title" },
                    Name = "全部博客",
                    OrderBy = orderBy
                },
                new EntityView()
                {
                    Sql = sql + " AND post.is_series is false",
                    ViewId = "463BE7FE-5435-4841-A365-C9C946C0D655",
                    CustomFilter = new List<string>() { "title" },
                    Name = "展示的博客",
                    OrderBy = orderBy
                },
                new EntityView()
                {
                    Sql = sql + " AND post.is_series is true",
                    ViewId = "834F8083-47BC-42F3-A6B2-DE25BE755714",
                    CustomFilter = new List<string>() { "title" },
                    Name = "展示的系列",
                    OrderBy =orderBy
                },
                new EntityView()
                {
                    Sql = $@"SELECT * FROM post WHERE post.is_series is true",
                    ViewId = "ACCE50D6-81A5-4240-BD82-126A50764FAB",
                    CustomFilter = new List<string>() { "title" },
                    Name = "全部系列",
                    OrderBy = orderBy
                }
            };
        }

        /// <summary>
        /// 根据id查询博客
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Post GetData(string id)
        {
            return Manager.ExecuteTransaction(() =>
            {
                var data = base.GetData(id);
                var paramList = new Dictionary<string, object>() { { "@id", id } };
                Manager.Execute("UPDATE post SET reading_times = COALESCE(reading_times, 0) + 1 WHERE id = @id", paramList);
                return data;
            });
        }

        public PostCategories GetCategories()
        {
            var data = new PostCategories();

            var sql = @"
SELECT
	post.id,
	post.title,
	post.post_type,
	post.post_type_name
FROM
	post
WHERE 1=1 AND post.is_show is true";
            var dataList = Manager.Query<Post>(sql).ToList();

            var categories = dataList
                .GroupBy(p => p.PostType)
                .Select(b => new CategoryModel() { category = b.First().PostType, category_name = b.First().PostTypeName, data = new List<CategoryData>() })
                .ToList();

            dataList.Each(item =>
            {
                var category = categories.Where(d => d.category.Equals(item.PostType)).FirstOrDefault();
                category.data.Add(new CategoryData() { id = item.Id, title = item.Title });
            });

            return new PostCategories()
            {
                count = categories.Count(),
                data = categories
            };
        }

        /// <summary>
        /// 获取博客路由
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetPostRouters()
        {
            var sql = @"
SELECT
	router
FROM
	sys_menu 
WHERE
	parent_id = '7EB12A4C-2698-4A8B-956D-B2467BE1D886'
";
            return Manager.DbClient.Query<string>(sql);
        }

        /// <summary>
        /// 导出Markdown
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public (string fileName, string ContentType, byte[] bytes) ExportMarkdown(string id)
        {
            var data = GetData(id);
            var fileName = $"{data.Title}.md";
            var contentType = "application/octet-stream";
            using (MemoryStream ms = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(ms))
                {
                    sw.Write(data.Content);
                    sw.Close();
                }
                return (fileName, contentType, ms.ToArray());
            }
        }

        /// <summary>
        /// 获取首页用户
        /// </summary>
        /// <returns></returns>
        public SysUser? GetIndexUser()
        {
            var config = Manager.QueryFirst<SysConfig>(new { code = "index_user" });
            if (!string.IsNullOrEmpty(config?.Value))
            {
                return new SysUserService(Manager).GetDataByCode(config?.Value);
            }
            return null;
        }
        
        /// <summary>
        /// 生成文章摘要
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GenerateSummary(string content)
        {
            IAIService service = ServiceFactory.Resolve<IAIService>(AIServiceResolver.Resolve);

            string template = "根据以下内容写一个 100 字摘要：{question}";

            PromptTemplate promptTemplate = new(template);

            Dictionary<string, string> variables = new ()
            {
                { "question", content }
            };

            try
            {
                string result = await service.ProcessChatTemplateAsync(promptTemplate, variables);
                return result;
            }
            catch (Exception ex)
            {
                throw new SpException($"发生错误: {ex.Message}");
            }
        }
        
        /// <summary>
        /// 根据提示词生成 Markdown 内容
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<string> GenerateMarkdownContent(string prompt)
        {
            IAIService service = ServiceFactory.Resolve<IAIService>(AIServiceResolver.Resolve);

            string template = "根据提示词写一篇 Markdown 文章，文字里要夹杂图标：{question}";

            PromptTemplate promptTemplate = new(template);

            Dictionary<string, string> variables = new()
            {
                { "question", prompt }
            };

            try
            {
                string result = await service.ProcessChatTemplateAsync(promptTemplate, variables);
                return result;
            }
            catch (Exception ex)
            {
                throw new SpException($"发生错误: {ex.Message}");
            }
        }
    }
}
