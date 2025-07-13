#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/aspnet:8.0 AS base
#USER $APP_UID
WORKDIR /app
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:8080

#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/BlogApp.API/BlogApp.API.csproj", "src/BlogApp.API/"]
COPY ["src/BlogApp.Core/BlogApp.Core.csproj", "src/BlogApp.Core/"]
COPY ["src/BlogApp.Domain/BlogApp.Domain.csproj", "src/BlogApp.Domain/"]
COPY ["src/BlogApp.Infrastructure/BlogApp.Infrastructure.csproj", "src/BlogApp.Infrastructure/"]
RUN dotnet restore "src/BlogApp.API/BlogApp.API.csproj"
COPY . .
WORKDIR "/src/src/BlogApp.API"
RUN dotnet build "./BlogApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BlogApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BlogApp.API.dll"]
