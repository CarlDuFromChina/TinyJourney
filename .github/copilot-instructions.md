# TinyJourney Copilot Instructions

## 项目概览

TinyJourney 是一个用于记录孩子成长历程的博客系统，使用 .NET 8.0 后端 + Vue 2.6 前端，支持 PC/移动端响应式设计。

## 架构分层

```
Sixpence.Core/           # 核心框架层（可复用）
├── Sixpence.Common      # 工具类、缓存、配置、异常处理
├── Sixpence.EntityFramework  # 自研 ORM 框架
├── Sixpence.EntityFramework.Postgres  # PostgreSQL 驱动
├── Sixpence.Web         # Web 基础设施（认证、控制器基类、服务基类）
└── Sixpence.AI          # AI 集成（文心、DeepSeek）

Sixpence.TinyJourney/    # 业务应用层
├── Entity/              # 实体定义
├── Controllers/         # API 控制器
├── Service/             # 业务服务
├── Plugin/              # 实体生命周期插件
└── Job/                 # 定时任务
```

## 核心开发模式

### 实体定义（Entity）

实体类必须继承 `TrackedEntity`，使用特性声明表和字段：

```csharp
[Table, Description("博客分类")]
public class Category : TrackedEntity
{
    [PrimaryColumn, Description("id")]
    public string Id { get; set; }
    
    [Column, Description("名称")]
    public string Name { get; set; }
}
```

- 属性名使用 PascalCase，框架自动转换为 snake_case 数据库字段
- `TrackedEntity` 自动包含 `created_by`, `updated_by`, `created_at`, `updated_at` 字段

### 服务层（Service）

服务类继承 `EntityService<TEntity>`，自动获得 CRUD 能力：

```csharp
public class PostService : EntityService<Post>
{
    public PostService(IEntityManager manager, ILogger<EntityService<Post>> logger, 
        IRepository<Post> repository) : base(manager, logger, repository) { }

    // 重写 GetViewList() 自定义查询视图
    public override IList<EntityView> GetViewList() => new List<EntityView>() { ... };
}
```

### 控制器（Controller）

继承 `EntityBaseController<TEntity, TService>` 自动暴露标准 CRUD API：

```csharp
public class PostController : EntityBaseController<Post, PostService>
{
    public PostController(PostService service) : base(service) { }
    
    // 添加自定义端点，使用 [AllowAnonymous] 允许公开访问
    [HttpGet("categories"), AllowAnonymous]
    public PostCategories GetCategories() => _service.GetCategories();
}
```

### 实体插件（Plugin）

实现 `IEntityManagerPlugin` 处理实体生命周期事件：

```csharp
public class PostPlugin : IEntityManagerPlugin
{
    public void Execute(EntityManagerPluginContext context)
    {
        switch (context.Action)
        {
            case EntityAction.PreCreate:
            case EntityAction.PreUpdate:
                // 保存前处理
                break;
            case EntityAction.PostCreate:
                // 创建后清理缓存
                break;
        }
    }
}
```

### 定时任务（Job）

继承 `JobBase` 并配置 Cron 表达式：

```csharp
public class CleanJob : JobBase
{
    public override string Name => "清理作业";
    public override IScheduleBuilder ScheduleBuilder => 
        CronScheduleBuilder.CronSchedule("0 0 0 * * ?");
    
    public override void Executing(IJobExecutionContext context) { ... }
}
```

## 依赖注入注册

在 [ServiceCollectionExtension.cs](Sixpence.TinyJourney/ServiceCollectionExtension.cs) 中注册：

```csharp
// 1. 注册实体
services.AddTransient<IEntity, Post>();
// 2. 注册插件
services.AddScoped<IEntityManagerPlugin, PostPlugin>();
// 3. 注册仓储
services.AddScoped<IRepository<Post>, Repository<Post>>();
// 4. 注册服务
services.AddScoped<PostService>();
```

## 构建与运行

```bash
# 构建
dotnet build TinyJourney.sln

# 本地运行
dotnet run --project Sixpence.TinyJourney

# Docker 部署
docker-compose up -d
```

**端口分配：**
- `5050`: API 服务
- `8010`: 后台管理 (admin)
- `8012`: PC 公开页面
- `8014`: 移动端公开页面

## 配置文件

- [appsettings.json](Sixpence.TinyJourney/appsettings.json): 数据库连接、JWT、AI 平台配置
- 继承 `BaseAppConfig<T>` 创建强类型配置类（参考 `WebSiteInfoConfig`）

## 命名约定

| 类型 | 约定 | 示例 |
|------|------|------|
| 实体类 | PascalCase 单数 | `Post`, `Category` |
| 控制器 | `{Entity}Controller` | `PostController` |
| 服务 | `{Entity}Service` | `PostService` |
| 插件 | `{Entity}Plugin` | `PostPlugin` |
| 数据库表/字段 | snake_case | `post`, `created_at` |

## 新增功能检查清单

1. 在 `Entity/` 创建实体类
2. 在 `Service/` 创建服务类
3. 在 `Controllers/` 创建控制器
4. 在 `ServiceCollectionExtension.cs` 注册依赖
5. 如需生命周期钩子，在 `Plugin/` 创建插件
