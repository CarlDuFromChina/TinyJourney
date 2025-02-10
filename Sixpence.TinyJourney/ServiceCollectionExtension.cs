using Microsoft.Extensions.DependencyInjection;
using Sixpence.EntityFramework;
using Sixpence.TinyJourney.Entity;
using Sixpence.TinyJourney.EntityOptionProvider;
using Sixpence.TinyJourney.Plugin;
using Sixpence.Web;
using Quartz;
using Sixpence.TinyJourney.Job;
using Sixpence.TinyJourney.Config;
using Sixpence.AI;
using Sixpence.AI.Wenxin;
using Sixpence.AI.DeepSeek;
using Sixpence.TinyJourney.Service;
using Sixpence.EntityFramework.Repository;

namespace Sixpence.TinyJourney
{
    public static class ServiceCollectionExtension
    {
        public static void AddSixpencePortal(this IServiceCollection services)
        {
            services
                .AddEntityFramework()
                .AddInitData()
                .AddEntityOptionProvider()
                .AddSysConfig()
                .AddJob()
                .AddAIService()
                .AddEntityService();
        }

        private static IServiceCollection AddEntityFramework(this IServiceCollection services)
        {
            // 1. 添加实体
            services.AddTransient<IEntity, Category>();
            services.AddTransient<IEntity, Draft>();
            services.AddTransient<IEntity, Link>();
            services.AddTransient<IEntity, Post>();
            services.AddTransient<IEntity, Idea>();

            // 2. 添加实体插件
            services.AddScoped<IEntityManagerPlugin, PostPlugin>();
            services.AddScoped<IEntityManagerPlugin, CategoryPlugin>();

            // 3. 添加仓储
            services.AddScoped<IRepository<Category>, Repository<Category>>();
            services.AddScoped<IRepository<Draft>, Repository<Draft>>();
            services.AddScoped<IRepository<Link>, Repository<Link>>();
            services.AddScoped<IRepository<Post>, Repository<Post>>();
            services.AddScoped<IRepository<Idea>, Repository<Idea>>();

            return services;
        }

        private static IServiceCollection AddEntityOptionProvider(this IServiceCollection services)
        {
            services.AddScoped<IEntityOptionProvider, CategoryEntityOptionProvider>();
            return services;
        }

        private static IServiceCollection AddInitData(this IServiceCollection services)
        {
            services.AddScoped<IInitDbData, InitDbBusinessData>();
            return services;
        }

        private static IServiceCollection AddSysConfig(this IServiceCollection services)
        {
            services.AddTransient<ISysConfig, IndexUserConfig>();
            services.AddTransient<ISysConfig, WebSiteInfoConfig>();
            return services;
        }

        private static IServiceCollection AddJob(this IServiceCollection services)
        {
            services.AddScoped<IJob, CleanJob>();
            return services;
        }

        public static IServiceCollection AddAIService(this IServiceCollection services)
        {
            services.AddSingleton<IAIService, WenxinAIService>();
            services.AddSingleton<IAIService, DeepSeekAIService>();
            return services;
        }

        public static IServiceCollection AddEntityService(this IServiceCollection services)
        {
            services.AddScoped<AnalysisService>();
            services.AddScoped<CategoryService>();
            services.AddScoped<DraftService>();
            services.AddScoped<IdeaSerivice>();
            services.AddScoped<LinkService>();
            services.AddScoped<PostService>();
            return services;
        }
    }
}
