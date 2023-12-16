using Microsoft.AspNetCore.Mvc.ApplicationModels;
using NLog.Extensions.Logging;
using Sixpence.Portal;
using Sixpence.Web;
using Sixpence.Web.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(m => m.AddNLog());

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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 添加 Web 服务注入
builder.Services.AddSixpencePortal();
builder.Services.AddSixpenceWeb();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSixpenceWeb();

app.MapFallbackToFile("/index.html");

app.Run();
