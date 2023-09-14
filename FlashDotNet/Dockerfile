# 使用基础镜像
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 9000

# 构建阶段
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
# 安装Node.js
RUN apt-get update && apt-get install -y nodejs npm


WORKDIR /src

# 复制.csproj文件和还原依赖关系
COPY ["CampusSecurityRankings/CampusSecurityRankings.csproj", "CampusSecurityRankings/"]
RUN dotnet restore "CampusSecurityRankings/CampusSecurityRankings.csproj"
COPY . .
WORKDIR "/src/CampusSecurityRankings"

# 运行Vue项目构建命令
WORKDIR "/src/CampusSecurityRankings/ClientApp"
RUN npm install
RUN npm run build

# 构建.NET项目
WORKDIR "/src/CampusSecurityRankings"
RUN dotnet build "CampusSecurityRankings.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CampusSecurityRankings.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CampusSecurityRankings.dll"]
