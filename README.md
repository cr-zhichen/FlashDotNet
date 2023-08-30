# FlashDotNet

🚀 轻松上手的 DotNet Core API 快速开发框架。

## 📌 特性

- 🔧 基于 `.NET 6.0`
- ✅ 启用了 Nullable 参考类型
- 🔄 启用了 Implicit Usings
- 📄 Debug/Release 配置下自动生成文档

## 📂 目录指南

- 🗄️ **App.db**: 主数据库文件（生成时自动创建）
- ⚙️ **appsettings.json**: 项目配置
- 🎫 **Attribute**: 属性相关，如 `AuthAttribute`
- 🎛️ **Controllers**: 控制中心，如 `CommandController`
- 📁 **Database**: 数据库核心，包括 `AppDbContext`
- 📊 **Entity**: 实体及其子类
- 🚧 **Filter**: 过滤机制，如 `CustomerExceptionFilter`
- 🔑 **Jwt**: JWT机制和配置
- 🔄 **Migrations**: 数据库迁移记录
- 🚀 **Program.cs**: 启动入口
- ⚙️ **Properties**: 如 `launchSettings`
- 📄 **Resources**: 资源汇总
- 📚 **Static**: 包括静态资料如 `TokenList`
- 🌐 **WS**: WebSocket 功能及配置

## 🧰 配置及中间件介绍

### 🖋️ Serilog 日志系统
使用了 Serilog 作为日志提供程序。记录包括输出到控制台和每日滚动文件 `Logs/Log.txt`。

### 🌍 跨域设置
已预配置为允许任何来源、任何方法和任何头的跨域请求。

### 🎬 MVC 和过滤器配置
- 采用了 MVC 控制器并启用了端点 API 探索。
- 默认添加了 `CustomerExceptionFilter` 和 `ModelValidateActionFilterAttribute` 过滤器。
- `ApiBehaviorOptions` 已配置以禁止 ModelState 无效的过滤。

### 🔐 JWT 认证
支持 JWT 认证，并从 `appsettings.json` 中获取其配置。

### 📖 Swagger API 文档
- API 文档已集成，并在开发环境中自动启用。
- 文档支持 JWT Bearer 授权。
- 已自定义了 Schema IDs 以适应项目。

### 💬 WebSocket 配置
支持 WebSocket 连接，所有 WebSocket 请求被定向到 `/ws`。

### 💽 数据库配置
使用 SQLite 数据库并存储于 `App.db`。启动时，确保数据库已创建。

### 🚀 控制器与端点
所有 API 控制器已映射，且默认页面为 Swagger UI。

### 📁 静态文件服务
静态文件服务已配置，`Resources` 目录下文件可通过 `/Resources` 访问。此服务已设置跨域权限。

## 🚀 快速开始

1. ⬇️ 克隆本仓库
2. 📦 运行 `dotnet restore`
3. ▶️ 启动项目：`dotnet run`