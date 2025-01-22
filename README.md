# TinyJourney 🌟  

## 简介  

生活中，每个为人父母者都希望永远珍藏孩子成长的点点滴滴。TinyJourney（小小旅程）正是为此而生，它让我们得以记录下孩子们最珍贵的每一刻。这个名字承载着温暖与期待，就像孩子们带给我们的小小幸福一样美好。  

项目基于我的[博客网站](https://karldu.cn)进行改造，有异曲同工之处。  如果你感兴趣可以访问我宝贝的[TinyJourney在线网站](https://duyuchen.cn)。  

> **注意**：评论功能（💬 Disqus）和视频内容（📹 YouTube）可能需要外网访问，如果你的网络环境有限制，可能无法正常访问。  

---  

## 技术特点 🚀  

后端采用 **.NET 6.0**，前端采用 **Vue 2.6** 开发的 Web 应用程序。它具有响应式设计，可以在 PC 和移动设备上访问。  

---  

## 功能特点 ✨  

- **特点 1**：支持 PC 和移动端显示 📱💻  
- **特点 2**：Markdown 编辑器，程序员的最爱 ✍️  
- **特点 3**：Disqus 留言板，方便用户交流 💬  
- **特点 4**：Docker 部署，方便快捷 🐳  
- **特点 5**：前后端分离，方便二次开发 🔧  
- **特点 6**：支持文心和 DeepSeek 两种 AI 平台，可以动态生成文案 🤖  

---  

## 项目结构 📂  

```
TinyJourney/
├── Sixpence.Core/                          # 核心库  
│   ├── Sixpence.AI/                        # AI模块  
│   ├── Sixpence.Common/                    # 公共模块  
│   ├── Sixpence.EntityFramework/           # EF ORM框架  
│   ├── Sixpence.EntityFramework.Postgres/  # Postgres驱动  
│   ├── Sixpence.EntityFramework.Sqlite/    # Sqlite驱动  
│   └── Sixpence.Web/                       # Web模块  
├── Sixpence.TinyJourney/                   # 项目主体  
│   ├── Config/                             # 配置类  
│   ├── Controllers/                        # 控制器  
│   ├── Entity/                             # 实体类  
│   ├── EntityOptionsProvider/              # 实体选项类  
│   ├── Job/                                # 任务  
│   ├── Model/                              # 模型  
│   ├── Plugin/                             # 插件  
├── Sixpence.UI/                            # 前端代码  
│   ├── vue-admin/                          # 后台管理网站  
│   ├── vue-mobile-public/                  # 移动端网站  
│   └── vue-pc-public                       # PC端网站  
├── docker-compose.yml                      # Docker Compose 配置文件 🐳  
├── Dockerfile                              # 后端 Docker 镜像构建文件 🛠️  
├── Makefile                                # 编译脚本 📜  
└── README.md                               # 项目说明文件 📘  
```  

---  

## 许可证 📜  

本项目基于 [MIT 许可证](./LICENSE)。