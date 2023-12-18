# 定义项目路径变量
ARG PROJECT_PATH=FlashDotNet/
# 定义项目名称变量
ARG PROJECT_NAME=FlashDotNet

# 使用 Node.js 镜像来构建前端
FROM node:16 AS node_builder
WORKDIR /app
# 定义项目路径
ARG PROJECT_PATH
ARG PROJECT_NAME
# 定义代理变量
ARG HTTP_PROXY
ARG HTTPS_PROXY
ENV http_proxy=$HTTP_PROXY \
    https_proxy=$HTTPS_PROXY
# 安装依赖
COPY ["${PROJECT_PATH}ClientApp/package.json", "${PROJECT_PATH}ClientApp/package-lock.json*", "./"]
RUN npm install
COPY ["${PROJECT_PATH}ClientApp", "."]
RUN npm run build

# 使用基础 ASP.NET 镜像
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 9000
# 定义项目路径
ARG PROJECT_PATH
ARG PROJECT_NAME
# 定义代理变量
ARG HTTP_PROXY
ARG HTTPS_PROXY
ENV http_proxy=$HTTP_PROXY \
    https_proxy=$HTTPS_PROXY

# 使用 .NET SDK 镜像构建后端
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# 定义项目路径
ARG PROJECT_PATH
ARG PROJECT_NAME
# 定义代理变量
ARG HTTP_PROXY
ARG HTTPS_PROXY
ENV http_proxy=$HTTP_PROXY \
    https_proxy=$HTTPS_PROXY
# 复制项目文件
COPY ["${PROJECT_PATH}${PROJECT_NAME}.csproj", "."]
RUN dotnet restore "${PROJECT_NAME}.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "${PROJECT_PATH}${PROJECT_NAME}.csproj" -c Release -o /app/build
# 发布 .NET 应用
FROM build AS publish
RUN dotnet publish "${PROJECT_PATH}${PROJECT_NAME}.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 将构建的文件复制到最终镜像中
FROM base AS final
WORKDIR /app
# 定义项目路径
ARG PROJECT_PATH
ARG PROJECT_NAME
COPY --from=publish /app/publish .
COPY --from=node_builder /wwwroot ./wwwroot
ENTRYPOINT ["dotnet", "${PROJECT_NAME}.dll"]
