#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5000

# vi editor
RUN apt-get update && apt-get install -y vim

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

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sixpence.TinyJourney.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sixpence.TinyJourney.dll"]