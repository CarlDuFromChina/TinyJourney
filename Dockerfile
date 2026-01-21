# 请参阅 https://aka.ms/customizecontainer 以了解如何自定义调试容器，以及 Visual Studio 如何使用此 Dockerfile 生成映像以更快地进行调试。

# 此阶段用于在快速模式(默认为调试配置)下从 VS 运行时
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5050
EXPOSE 8010
EXPOSE 8012
EXPOSE 8014


# 此阶段用于生成服务项目
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sixpence.TinyJourney/Sixpence.TinyJourney.csproj", "Sixpence.TinyJourney/"]
COPY ["Sixpence.Core/Sixpence.Common/Sixpence.Common.csproj", "Sixpence.Core/Sixpence.Common/"]
COPY ["Sixpence.Core/Sixpence.Web/Sixpence.Web.csproj", "Sixpence.Core/Sixpence.Web/"]
COPY ["Sixpence.Core/Sixpence.EntityFramework.Postgres/Sixpence.EntityFramework.Postgres.csproj", "Sixpence.Core/Sixpence.EntityFramework.Postgres/"]
COPY ["Sixpence.Core/Sixpence.EntityFramework/Sixpence.EntityFramework.csproj", "Sixpence.Core/Sixpence.EntityFramework/"]
COPY ["Sixpence.Core/Sixpence.EntityFramework.Sqlite/Sixpence.EntityFramework.Sqlite.csproj", "Sixpence.Core/Sixpence.EntityFramework.Sqlite/"]
RUN dotnet restore "./Sixpence.TinyJourney/Sixpence.TinyJourney.csproj"
COPY . .
WORKDIR "/src/Sixpence.TinyJourney"
RUN dotnet build "./Sixpence.TinyJourney.csproj" -c $BUILD_CONFIGURATION -o /app/build

# 此阶段用于发布要复制到最终阶段的服务项目
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sixpence.TinyJourney.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# 此阶段在生产中使用，或在常规模式下从 VS 运行时使用(在不使用调试配置时为默认值)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sixpence.TinyJourney.dll"]