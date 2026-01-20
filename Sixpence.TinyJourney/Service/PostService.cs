using Sixpence.AI;
using Sixpence.Common;
using Sixpence.EntityFramework;
using Sixpence.TinyJourney.Entity;
using Sixpence.TinyJourney.Model;
using Sixpence.Web;
using Sixpence.Web.Entity;
using Sixpence.Web.Model;
using Sixpence.Web.Service;

namespace Sixpence.TinyJourney.Service
{
    public class PostService : EntityService<Post>
    {
        private readonly SysUserService _userService;
        private readonly Lazy<IEnumerable<IAIService>> aIService;
        public PostService(IEntityManager manager, ILogger<EntityService<Post>> logger, IRepository<Post> repository, SysUserService userService, IServiceProvider provider) : base(manager, logger, repository)
        {
            _userService = userService;
            aIService = new Lazy<IEnumerable<IAIService>>(provider.GetServices<IAIService>);
        }

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
	post.tags,
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
                    Sql = sql,
                    ViewId = "463BE7FE-5435-4841-A365-C9C946C0D655",
                    CustomFilter = new List<string>() { "title" },
                    Name = "展示的博客",
                    OrderBy = orderBy
                }
            };
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
            var dataList = _manager.Query<Post>(sql).ToList();

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
            return _manager.DbClient.Query<string>(sql);
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
        /// 生成文章摘要
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GenerateSummary(string content)
        {
            IAIService service = aIService.Value?.FirstOrDefault(AIServiceResolver.Resolve);

            string template = "根据以下内容写一个 100 字摘要：{question}";

            PromptTemplate promptTemplate = new(template);

            Dictionary<string, string> variables = new()
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
            IAIService service = aIService.Value?.FirstOrDefault(AIServiceResolver.Resolve);

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
