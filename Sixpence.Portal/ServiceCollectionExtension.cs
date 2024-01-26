using Microsoft.Extensions.DependencyInjection;
using Sixpence.ORM;
using Sixpence.Portal.Implements;
using Sixpence.PortalEntity;
using Sixpence.PortalEntityOptionProvider;
using Sixpence.PortalPlugin;
using Sixpence.Web;

namespace Sixpence.Portal
{
    public static class ServiceCollectionExtension
    {
        public static void AddSixpencePortal(this IServiceCollection services)
        {
            services
                .AddEntity()
                .AddInitData()
                .AddEntityPlugin()
                .AddEntityOptionProvider();
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
    }
}
