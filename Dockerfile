# 使用 Node.js 镜像来构建前端
FROM node:16 AS node_builder
WORKDIR /app
COPY ["FlashDotNet/ClientApp/package.json", "FlashDotNet/ClientApp/package-lock.json*", "./"]
# 使用代理安装依赖（如果有的话）
ARG HTTP_PROXY
ARG HTTPS_PROXY
RUN if [ -n "$HTTP_PROXY" ]; then npm config set proxy $HTTP_PROXY; fi; \
    if [ -n "$HTTPS_PROXY" ]; then npm config set https-proxy $HTTPS_PROXY; fi; \
    npm install
COPY ["FlashDotNet/ClientApp", "."]
RUN npm run build

# 使用 .NET SDK 镜像构建后端
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["FlashDotNet/FlashDotNet.csproj", "./"]
# 使用代理恢复依赖（如果有的话）
ARG HTTP_PROXY
ARG HTTPS_PROXY
RUN if [ -n "$HTTP_PROXY" ]; then dotnet nuget add source $HTTP_PROXY --name proxy; fi; \
    if [ -n "$HTTPS_PROXY" ]; then dotnet nuget add source $HTTPS_PROXY --name proxy; fi; \
    dotnet restore "FlashDotNet.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "FlashDotNet/FlashDotNet.csproj" -c Release -o /app/build

# 发布 .NET 应用
FROM build AS publish
RUN dotnet publish "FlashDotNet/FlashDotNet.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 使用基础 ASP.NET 镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 9000

# 将构建的文件复制到最终镜像中
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=node_builder /wwwroot ./wwwroot
ENTRYPOINT ["dotnet", "FlashDotNet.dll"]
