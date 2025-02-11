using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using Sixpence.Common;
using Sixpence.EntityFramework;
using Sixpence.EntityFramework.Postgres;
using Sixpence.EntityFramework.Repository;
using Sixpence.Web.Auth.Gitee;
using Sixpence.Web.Auth.Github;
using Sixpence.Web.Auth.Role;
using Sixpence.Web.Config;
using Sixpence.Web.Entity;
using Sixpence.Web.EntityInterceptor;
using Sixpence.Web.EntityPlugin;
using Sixpence.Web.Module.SysAttrs;
using Sixpence.Web.Module.SysMenu;
using Sixpence.Web.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sixpence.Web.ImageResource;
using Sixpence.EntityFramework.Sqlite;
using Sixpence.Web.Service;

namespace Sixpence.Web
{
    public static class ServiceCollectionExtension
    {
        public static void AddSixpenceWeb(this IServiceCollection services)
        {
            services
                .AddEntityFramework()
                .AddRepository()
                .AddStorage()
                .AddRole()
                .AddInitData()
                .AddSSO()
                .AddJwt()
                .AddServices()
                .AddEntityService();
        }

        private static IServiceCollection AddEntityFramework(this IServiceCollection services)
        {
            // 1. 添加实体
            services.AddTransient<IEntity, Gallery>();
            services.AddTransient<IEntity, JobHistory>();
            services.AddTransient<IEntity, MailVertification>();
            services.AddTransient<IEntity, MessageRemind>();
            services.AddTransient<IEntity, SysAttrs>();
            services.AddTransient<IEntity, SysAuthUser>();
            services.AddTransient<IEntity, SysConfig>();
            services.AddTransient<IEntity, SysEntity>();
            services.AddTransient<IEntity, SysFile>();
            services.AddTransient<IEntity, SysMenu>();
            services.AddTransient<IEntity, SysRole>();
            services.AddTransient<IEntity, SysRolePrivilege>();
            services.AddTransient<IEntity, SysUser>();
            services.AddTransient<IEntity, VersionScriptExecutionLog>();

            // 2. 添加实体插件
            services.AddScoped<IEntityManagerPlugin, GalleryPlugin>();
            services.AddScoped<IEntityManagerPlugin, SysEntityPlugin>();
            services.AddScoped<IEntityManagerPlugin, SysMenuPlugin>();
            services.AddScoped<IEntityManagerPlugin, SysEntityPlugin>();
            services.AddScoped<IEntityManagerPlugin, MailVertificationPlugin>();
            services.AddScoped<IEntityManagerPlugin, SysRolePlugin>();
            services.AddScoped<IEntityManagerPlugin, SysRolePrivilegePlugin>();
            services.AddScoped<IEntityManagerPlugin, SysUserPlugin>();

            // 3. 添加实体拦截器
            services.AddScoped<IEntityMigrationInterceptor, EntityMigrationInterceptor>();
            services.AddScoped<IEntityManagerBeforeCreateOrUpdate, Implements.EntityManagerBeforeCreateOrUpdate>();

            // 4. 添加数据库连接
            services.AddEntityFramework(options =>
            {
                var driverType = DBSourceConfig.Config.DriverType.ToEnum<DriverType>();
                var connectionString = DBSourceConfig.Config.ConnectionString;
                var timeout = DBSourceConfig.Config.CommandTimeOut;

                switch (driverType)
                {
                    case DriverType.Postgresql:
                        options.UsePostgres(connectionString, timeout);
                        break;
                    case DriverType.Sqlite:
                        options.UseSqlite(connectionString, timeout);
                        break;
                    case DriverType.MySql:
                    case DriverType.SqlServer:
                    case DriverType.Oracle:
                    default:
                        throw new SpException("unsupported driver type");
                }
            });

            return services;
        }

        private static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddScoped<IRepository<Gallery>, Repository<Gallery>>();
            services.AddScoped<IRepository<JobHistory>, Repository<JobHistory>>();
            services.AddScoped<IRepository<MailVertification>, Repository<MailVertification>>();
            services.AddScoped<IRepository<MessageRemind>, Repository<MessageRemind>>();
            services.AddScoped<IRepository<SysAttrs>, Repository<SysAttrs>>();
            services.AddScoped<IRepository<SysAuthUser>, Repository<SysAuthUser>>();
            services.AddScoped<IRepository<SysConfig>, Repository<SysConfig>>();
            services.AddScoped<IRepository<SysEntity>, Repository<SysEntity>>();
            services.AddScoped<IRepository<SysFile>, Repository<SysFile>>();
            services.AddScoped<IRepository<SysMenu>, Repository<SysMenu>>();
            services.AddScoped<IRepository<SysRole>, Repository<SysRole>>();
            services.AddScoped<IRepository<SysRolePrivilege>, Repository<SysRolePrivilege>>();
            services.AddScoped<IRepository<SysUser>, Repository<SysUser>>();
            services.AddScoped<IRepository<VersionScriptExecutionLog>, Repository<VersionScriptExecutionLog>>();
            return services;
        }

        private static IServiceCollection AddStorage(this IServiceCollection services)
        {
            services.AddScoped<IStorage, SystemLocalStore>();
            return services;
        }

        private static IServiceCollection AddRole(this IServiceCollection services)
        {
            services.AddScoped<IRole, AdminRole>();
            services.AddScoped<IRole, GuestRole>();
            services.AddScoped<IRole, UserRole>();
            services.AddScoped<IRole, SystemRole>();
            return services;
        }

        private static IServiceCollection AddInitData(this IServiceCollection services)
        {
            services.AddTransient<IInitDbData, InitDbData>();
            return services;
        }

        public static IServiceCollection AddSSO(this IServiceCollection services)
        {
            services.AddScoped<IThirdPartyBindStrategy, GithubUserBind>();
            services.AddScoped<IThirdPartyBindStrategy, GiteeUserBind>();
            services.AddScoped<IThirdPartyLoginStrategy, GiteeLogin>();
            services.AddScoped<IThirdPartyLoginStrategy, GithubLogin>();
            return services;
        }

        public static IServiceCollection AddJwt(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Api", policy => policy.RequireClaim("Type", "AccessToken").Build());
                options.AddPolicy("Refresh", policy => policy.RequireClaim("Type", "RefreshToken").Build());
            });

            // 读取配置文件
            var symmetricKeyAsBase64 = JwtConfig.Config.SecretKey;
            var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
            var signingKey = new SymmetricSecurityKey(keyByteArray);
            var Issuer = JwtConfig.Config.Issuer;
            var Audience = JwtConfig.Config.Audience;

            //2.1【认证】、core自带官方JWT认证
            // 开启Bearer认证
            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             // 添加JwtBearer服务
             .AddJwtBearer(o =>
             {
                 o.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     IssuerSigningKey = signingKey,
                     ValidateIssuer = true,
                     ValidIssuer = Issuer, // 发行人
                     ValidateAudience = true,
                     ValidAudience = Audience,//订阅人
                     ValidateLifetime = true,
                     ClockSkew = TimeSpan.FromSeconds(30),
                     RequireExpirationTime = true,
                 };
                 o.Events = new JwtBearerEvents
                 {
                     OnAuthenticationFailed = context =>
                     {
                         // 如果过期，则把<是否过期>添加到，返回头信息中
                         if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                         {
                             context.Response.StatusCode = 403;
                             context.Response.ContentType = "application/json";
                             var error = "Token Expired";
                             return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(error));
                         }
                         return Task.CompletedTask;
                     }
                 };
             });
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddSingleton<IThirdPartyImageResourceDriver, LandscapeImageResourceDriver>();
            return services;
        }

        public static IServiceCollection AddEntityService(this IServiceCollection services)
        {
            services.AddScoped<GalleryService>();
            services.AddScoped<GiteeAuthService>();
            services.AddScoped<GithubAuthService>();
            services.AddScoped<JobHistoryService>();
            services.AddScoped<JobService>();
            services.AddScoped<MailVertificationService>();
            services.AddScoped<MessageRemindService>();
            services.AddScoped<PixabayService>();
            services.AddScoped<SysAttrsService>();
            services.AddScoped<SysAuthUserService>();
            services.AddScoped<SysEntityService>();
            services.AddScoped<SysFileService>();
            services.AddScoped<SysMenuService>();
            services.AddScoped<SysRolePrivilegeService>();
            services.AddScoped<SysRoleService>();
            services.AddScoped<SystemService>();
            services.AddScoped<SysUserService>();
            services.AddScoped<VersionScriptExecutionLogService>();
            return services;
        }
    }
}
