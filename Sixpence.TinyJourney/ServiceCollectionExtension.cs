using Microsoft.Extensions.DependencyInjection;
using Sixpence.EntityFramework;
using Sixpence.TinyJourney.Entity;
using Sixpence.TinyJourney.EntityOptionProvider;
using Sixpence.TinyJourney.Plugin;
using Sixpence.Web;
using Quartz;
using Sixpence.TinyJourney.Job;
using Sixpence.TinyJourney.Config;

namespace Sixpence.TinyJourney
{
    public static class ServiceCollectionExtension
    {
        public static void AddSixpencePortal(this IServiceCollection services)
        {
            services
                .AddEntity()
                .AddInitData()
                .AddEntityPlugin()
                .AddEntityOptionProvider()
                .AddSysConfig()
                .AddJob();
        }

        private static IServiceCollection AddEntity(this IServiceCollection services)
        {
            services.AddSingleton<IEntity, Category>();
            services.AddSingleton<IEntity, Draft>();
            services.AddSingleton<IEntity, Link>();
            services.AddSingleton<IEntity, Post>();
            services.AddSingleton<IEntity, Idea>();
            return services;
        }

        private static IServiceCollection AddEntityPlugin(this IServiceCollection services)
        {
            services.AddTransient<IEntityManagerPlugin, PostPlugin>();
            services.AddTransient<IEntityManagerPlugin, CategoryPlugin>();
            return services;
        }

        private static IServiceCollection AddEntityOptionProvider(this IServiceCollection services)
        {
            services.AddTransient<IEntityOptionProvider, CategoryEntityOptionProvider>();
            return services;
        }

        private static IServiceCollection AddInitData(this IServiceCollection services)
        {
            services.AddTransient<IInitDbData, InitDbBusinessData>();
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
            services.AddSingleton<IJob, CleanJob>();
            return services;
        }
    }
}
