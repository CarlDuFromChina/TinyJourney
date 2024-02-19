#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000

# vi editor
RUN apt-get update && apt-get install -y vim

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Sixpence.Portal/Sixpence.Portal.csproj", "Sixpence.Portal/"]
COPY ["Sixpence.Core/Sixpence.Web/Sixpence.Web.csproj", "Sixpence.Core/Sixpence.Web/"]
COPY ["Sixpence.Core/Sixpence.Common/Sixpence.Common.csproj", "Sixpence.Core/Sixpence.Common/"]
COPY ["Sixpence.Core/Sixpence.ORM.Postgres/Sixpence.ORM.Postgres.csproj", "Sixpence.Core/Sixpence.ORM.Postgres/"]
COPY ["Sixpence.Core/Sixpence.ORM.Sqlite/Sixpence.ORM.Sqlite.csproj", "Sixpence.Core/Sixpence.ORM.Sqlite/"]
COPY ["Sixpence.Core/Sixpence.ORM/Sixpence.ORM.csproj", "Sixpence.Core/Sixpence.ORM/"]
RUN dotnet restore "./Sixpence.Portal/./Sixpence.Portal.csproj"
COPY . .
WORKDIR "/src/Sixpence.Portal"
RUN dotnet build "./Sixpence.Portal.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Sixpence.Portal.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Sixpence.Portal.dll"]