using Microsoft.Extensions.DependencyInjection;
using Sixpence.ORM;
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
            services.AddSingleton<IEntityManagerPlugin, PostPlugin>();
            services.AddSingleton<IEntityManagerPlugin, CategoryPlugin>();
            return services;
        }

        private static IServiceCollection AddEntityOptionProvider(this IServiceCollection services)
        {
            services.AddSingleton<IEntityOptionProvider, CategoryEntityOptionProvider>();
            return services;
        }
    }
}
