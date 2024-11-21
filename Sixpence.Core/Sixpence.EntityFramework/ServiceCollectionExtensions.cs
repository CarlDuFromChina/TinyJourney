using Microsoft.Extensions.DependencyInjection;
using Sixpence.EntityFramework.Mappers;
using Sixpence.EntityFramework.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sixpence.EntityFramework
{
    public static class ServiceCollectionExtensions
    {
        public static ServiceCollectionOptions Options;

        public static IServiceCollection AddSorm(this IServiceCollection services, Action<ServiceCollectionOptions> action)
        {
            services.AddTransient<IEntityManager, EntityManager>();
            services.AddTransient<IEntityManagerBeforeCreateOrUpdate, EntityManagerBeforeCreateOrUpdate>();

            Options = new ServiceCollectionOptions();
            action?.Invoke(Options);

            if (Options.DbSetting == null)
            {
                throw new ArgumentNullException("数据库设置不能为空");
            }

            services.AddMapper(Options);

            return services;
        }
    }
}
