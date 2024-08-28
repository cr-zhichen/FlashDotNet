# 🛠️ FlashDotNet

轻松上手的 DotNet Core API 快速开发框架。

## 📌 特性

- 基于 `.NET 8.0`
- 启用了 Nullable 参考类型
- 启用了 Implicit Usings
- Debug/Release 配置下自动生成文档

## 🧰 配置及中间件介绍

### 🖋️ Serilog 日志系统

使用了 Serilog 作为日志提供程序。日志记录方式如下：

- 输出到控制台。
- 记录到每日滚动文件夹：
    - 所有日志：`Logs/AllLogs/Log.txt`
    - 信息级别日志：`Logs/Information/Log-Information-.txt`
    - 警告级别日志：`Logs/Warning/Log-Warning-.txt`
    - 错误级别日志：`Logs/Error/Log-Error-.txt`

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

### 💽 数据库配置更新

数据库配置现在通过 `appsettings.json` 文件进行。在启动应用程序时，确保配置文件中包含正确的数据库连接字符串。

修改后的配置信息如下所示：

  ```json5
{
    // 可选值：mysql, sqlite, sqlserver, postgresql
    // 默认值：sqlite
    // 为空默认在程序根目录下创建App.db
    "DefaultConnection": "sqlite",
    "ConnectionStrings": {
        "MySqlConnection": "server=127.0.0.1;database=FlashDotNet;user=root;password=qwertyuiop",
        // 测试环境使用 Encrypt=False;TrustServerCertificate=True;用于解决SQL Server 的 SSL 连接问题
        // 正式环境请使用 Encrypt=True;TrustServerCertificate=False;或者删除这两个参数
        "SqlServerConnection": "Server=127.0.0.1;Database=FlashDotNet;User Id=sa;Password=Qwertyuiop1234;Encrypt=False;TrustServerCertificate=True;",
        "SqliteConnection": "Data Source=App.db",
        "PostgreSqlConnection": "Host=127.0.0.1;Database=FlashDotNet;Username=postgres;Password=qwertyuiop"
    },
}
  ```

例如，如果希望使用 MySQL 数据库，则需要在 `DefaultConnection` 中设置为 `"mysql"` 并使用 `MySqlConnection`
字符串。同理，对于其他数据库类型也是如此。

### 🚀 控制器与端点

所有 API 控制器已映射，且默认页面为 /index.html  
如需访问Swagger，请访问 /swagger/index.html

### 📁 静态文件服务

静态文件服务已配置，`Resources` 目录下文件可通过 `{url}/resources` 访问。此服务已设置跨域权限。

## 📡 接口通用规则

1. **响应格式** 🎯: 所有 API 的响应格式遵循 `IRe<T>` 接口定义。其中：

    - 📊 `Code`：响应的状态码，基于 `Code` 枚举定义。
        - 🟢 `Success`: 操作成功
        - 🛑 `Error`: 未知错误
        - 🔑 `TokenError`: Token 错误
    - 💬 `Message`：响应消息，例如错误描述。
    - 📦 `Data`：响应的具体数据。

2. **WebSocket 响应格式** 🌐: 所有 WebSocket 的响应格式遵循 `IWsRe<T>` 接口定义。其中：

    - 📍 `Route`：WebSocket 返回的路由，基于 `Route` 枚举定义。
    - 💬 `Message`：响应消息。
    - 📦 `Data`：响应的具体数据。

3. **WebSocket 请求格式** 📟: 所有 WebSocket 的请求格式遵循 `WsReq` 类定义。其中：

    - 📍 `Route`：WebSocket 请求的路由。
    - 📦 `Data`：请求的具体数据。

4. **异常处理** ⚠️:

    - 使用 `CustomerExceptionFilter` 过滤器捕捉所有异常。异常响应会返回 `Code.Error` 和具体的异常消息。
    - 使用 `ModelValidateActionFilterAttribute` 过滤器处理模型验证失败。当数据验证失败时，会返回 `Code.Error` 和描述消息。

5. **数据验证** ❌: 当输入数据不符合 API 预期格式或内容时，响应将返回描述性错误消息。

6. **认证和授权** 🔐: 使用 JWT 进行身份验证。Token 错误会返回 `Code.TokenError` 和描述消息。

请确保在实际使用中遵循以上规则以保持一致性和可预测性。

## 🚀 快速开始

1. 克隆本仓库
2. 运行 `dotnet restore`
3. 启动项目：`dotnet run`

## Dockerfile 部署

### 构建 Docker 镜像

``` bash
docker build --network="host" \
  --build-arg HTTP_PROXY=http://127.0.0.1:7890 \
  --build-arg HTTPS_PROXY=http://127.0.0.1:7890 \
  -t flash_dotnet:latest .
```

若无代理需求，可省略 `--network="host"` 和 `--build-arg` 参数。

```bash
docker build --network="host" \
  -t flash_dotnet:latest .
```

### 运行 Docker 镜像

``` bash
docker run -d \
  -p 9000:9000 \
  --name flash_dotnet \
  -e TokenOptions__SecretKey=Your_New_Secret_Key \
  -e TokenOptions__Issuer=Your_New_Issuer \
  -e TokenOptions__Audience=Your_New_Audience \
  flash_dotnet:latest
```
