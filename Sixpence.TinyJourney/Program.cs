using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.FileProviders;
using NLog.Extensions.Logging;
using Sixpence.TinyJourney;
using Sixpence.Web;
using Sixpence.Web.WebApi;
using Sixpence.Web.Config;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls(SystemConfig.Config.LocalUrls);

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.SetIsOriginAllowed(_ => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddLogging(m => m.AddNLog());

builder.Services.AddHttpContextAccessor();

builder.Services
    .AddControllers(options =>
    {
        options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
        options.Filters.Add<WebApiContextFilter>();
        options.Filters.Add<WebApiExceptionFilter>();
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = new SnakeCaseNamingPolicy();
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加 Web 服务注入
builder.Services.AddSixpencePortal();
builder.Services.AddSixpenceWeb();

var app = builder.Build();

var adminWebRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "admin");
var pcWebRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "pc");
var mobileWebRoot = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "mobile");

if (Directory.Exists(adminWebRoot))
{
    app.MapWhen(context => context.Connection.LocalPort == 8010, branch =>
    {
        var fileProvider = new PhysicalFileProvider(adminWebRoot);
        branch.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider });
        branch.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider });
        branch.UseRouting();
        branch.UseEndpoints(endpoints =>
        {
            endpoints.MapFallbackToFile("{*path}", "index.html", new StaticFileOptions { FileProvider = fileProvider });
        });
    });
}

if (Directory.Exists(pcWebRoot))
{
    app.MapWhen(context => context.Connection.LocalPort == 8012, branch =>
    {
        var fileProvider = new PhysicalFileProvider(pcWebRoot);
        branch.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider });
        branch.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider });
        branch.UseRouting();
        branch.UseEndpoints(endpoints =>
        {
            endpoints.MapFallbackToFile("{*path}", "index.html", new StaticFileOptions { FileProvider = fileProvider });
        });
    });
}

if (Directory.Exists(mobileWebRoot))
{
    app.MapWhen(context => context.Connection.LocalPort == 8014, branch =>
    {
        var fileProvider = new PhysicalFileProvider(mobileWebRoot);
        branch.UseDefaultFiles(new DefaultFilesOptions { FileProvider = fileProvider });
        branch.UseStaticFiles(new StaticFileOptions { FileProvider = fileProvider });
        branch.UseRouting();
        branch.UseEndpoints(endpoints =>
        {
            endpoints.MapFallbackToFile("{*path}", "index.html", new StaticFileOptions { FileProvider = fileProvider });
        });
    });
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDefaultFiles();
    app.UseStaticFiles();

    app.MapFallbackToFile("{*path:regex(^(?!api).*$)}", "index.html");
}

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers().RequireCors("CorsPolicy");

app.UseSixpenceWeb();

app.Run();
