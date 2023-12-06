# 使用基础镜像
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 9000
ARG HTTP_PROXY
ARG HTTPS_PROXY
ENV http_proxy=$HTTP_PROXY \
    https_proxy=$HTTPS_PROXY

# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG HTTP_PROXY
ARG HTTPS_PROXY
ENV http_proxy=$HTTP_PROXY \
    https_proxy=$HTTPS_PROXY
    
# 安装Node.js
RUN curl -fsSL https://deb.nodesource.com/setup_16.x | bash - \
    && apt-get install -y nodejs

WORKDIR /src

# 复制.csproj文件和还原依赖关系
COPY ["FlashDotNet/FlashDotNet.csproj", "FlashDotNet/"]
RUN dotnet restore "FlashDotNet/FlashDotNet.csproj"
COPY . .
WORKDIR "/src/FlashDotNet"

# 运行Vue项目构建命令
WORKDIR "/src/FlashDotNet/ClientApp"
RUN npm install
RUN npm run build

# 构建.NET项目
WORKDIR "/src/FlashDotNet"
RUN dotnet build "FlashDotNet.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FlashDotNet.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FlashDotNet.dll"]
