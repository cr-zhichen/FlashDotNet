using System.Reflection;
using System.Text;
using FlashDotNet.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using FlashDotNet.Filter;
using FlashDotNet.Jwt;
using FlashDotNet.Static;
using FlashDotNet.WS;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Serilog;

#region 应用构建器与配置

var baseDirectory = Path.GetDirectoryName(AppContext.BaseDirectory);

// 配置Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(Path.Combine(baseDirectory, "Logs/AllLogs/Log.txt"), rollingInterval: RollingInterval.Day)
    .WriteTo.File(Path.Combine(baseDirectory, "Logs/Information/Log-Information-.txt"),
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information, rollingInterval: RollingInterval.Day)
    .WriteTo.File(Path.Combine(baseDirectory, "Logs/Warning/Log-Warning-.txt"),
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning, rollingInterval: RollingInterval.Day)
    .WriteTo.File(Path.Combine(baseDirectory, "Logs/Error/Log-Error-.txt"),
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error, rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = baseDirectory,
});

// 使用Serilog作为日志提供程序
builder.Host.UseSerilog();

#endregion

#region 跨域设置配置

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        corsPolicyBuilder => corsPolicyBuilder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

#endregion

#region MVC配置及过滤器

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<CustomerExceptionFilter>();
    options.Filters.Add(typeof(ModelValidateActionFilterAttribute));
});

builder.Services.Configure<ApiBehaviorOptions>(opt => opt.SuppressModelStateInvalidFilter = true);

#endregion

#region JWT配置

var section = builder.Configuration.GetSection("TokenOptions");
var tokenOptions = section.Get<TokenOptions>();
builder.Services.AddTransient<IJwtService, JwtService>();
builder.Services.Configure<TokenOptions>(section);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = tokenOptions.Audience,
            ValidIssuer = tokenOptions.Issuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenOptions.SecretKey))
        };
    });

#endregion

#region Swagger配置

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "FlashDotNet", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "使用 Bearer 方案的 JWT 授权标头。",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new List<string>()
        }
    });
});

builder.Services.AddSwaggerGen(c => { c.CustomSchemaIds(x => x.FullName); });

#endregion

#region 依赖注入

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<WebSocketController>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(baseDirectory, "App.db")}")
);
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    // 如果在开发环境中使用了代理服务器，需要添加下面的代码以防止环回网络地址的错误
    // options.KnownNetworks.Clear();
    // options.KnownProxies.Clear();
});

#endregion

var app = builder.Build();

#region Swagger中间件配置

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

#endregion

#region 基础中间件配置

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

#endregion

#region WebSocket配置

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};

app.UseWebSockets(webSocketOptions);
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var webSocketController = context.RequestServices.GetService<WebSocketController>();
            await webSocketController?.HandleWebSocketAsync(context, webSocket)!;
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});

#endregion

#region 数据库初始化

using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
context.Database.EnsureCreated();

#endregion

#region 控制器与端点配置

app.MapControllers();

app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", httpContext =>
    {
        httpContext.Response.Redirect("/index.html");
        return Task.CompletedTask;
    });
});

#endregion

#region 静态文件配置

app.UseStaticFiles();

var resourcesPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources");

if (!Directory.Exists(resourcesPath))
{
    Directory.CreateDirectory(resourcesPath);
}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(resourcesPath),
    RequestPath = "/resources",
    OnPrepareResponse = ctx => { ctx.Context.Response.Headers.Add("Access-Control-Allow-Origin", "*"); }
});

#endregion

app.Run();

// 关闭和刷新日志
Log.CloseAndFlush();