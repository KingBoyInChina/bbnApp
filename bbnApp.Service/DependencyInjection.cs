using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.IServices.IINIT;
using bbnApp.Application.IServices.IJWT;
using bbnApp.Application.Services.CODE;
using bbnApp.Application.Services.INIT;
using bbnApp.Application.Services.JWT;
using bbnApp.Common;
using bbnApp.Core;
using bbnApp.GrpcClients;
using bbnApp.Infrastructure.Dapr;
using bbnApp.Infrastructure.Data;
using bbnApp.Service.GlobalService;
using bbnApp.Service.Services;
using Exceptionless;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using bbnApp.Protos;

public static class DependencyInjection
{
    /// <summary>
    /// 内存缓存
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMemoryCacheServices(this IServiceCollection services)
    {
        // 配置内存缓存
        services.AddMemoryCache();
        return services;
    }
    /// <summary>
    /// 数据库服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 读取数据库连接字符串
        var basicConnectionString = bbnApp.Share.EncodeAndDecode.MixDecrypt(configuration.GetConnectionString("DataConnection")); // 基础数据数据库
        var codeConnectionString = bbnApp.Share.EncodeAndDecode.MixDecrypt(configuration.GetConnectionString("CodeConnection"));     // 代码数据数据库

        // 配置 ApplicationDbContext 并注入到服务容器中
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseMySql(basicConnectionString, ServerVersion.AutoDetect(basicConnectionString));
        });

        // 配置 ApplicationDbCodeContext 并注入到服务容器中
        services.AddDbContext<ApplicationDbCodeContext>(options =>
        {
            options.UseMySql(codeConnectionString, ServerVersion.AutoDetect(codeConnectionString));
        });

        // 注册 IApplicationDbContext 使用不同的实现
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IApplicationDbCodeContext>(provider => provider.GetRequiredService<ApplicationDbCodeContext>());

        // 注册连接字符串
        services.AddSingleton(new ConnectionStringOptions
        {
            BasicConnectionString = basicConnectionString,
            CodeConnectionString = codeConnectionString
        });

        // 注册 DbConnectionFactory
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

        return services;
    }
    /// <summary>
    /// redis服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddRedisCacheServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 配置 Redis 缓存
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetSection("Redis:ConnectionString").Value; // Redis 连接字符串
        });

        // 注册 Redis 服务
        services.AddScoped<IRedisService, RedisService>();

        return services;
    }
    /// <summary>
    /// jwt服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("Jwt");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

        // 配置 JwtBearer 认证
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    //OnMessageReceived = context =>
                    //{
                    //    if (context.Request.Headers.ContainsKey("Authorization"))
                    //    {
                    //        Console.WriteLine("AUTH HEADER: " + context.Request.Headers["Authorization"]);
                    //    }
                    //    else
                    //    {
                    //        Console.WriteLine("AUTH HEADER NOT PROVIDED");
                    //    }
                    //    return Task.CompletedTask;
                    //},
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine("Token authentication failed: " + context.Exception.Message);
                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],              // 签发者
                    ValidAudience = jwtSettings["Audience"],          // 受众
                    IssuerSigningKey = new SymmetricSecurityKey(key) // 密钥
                };
            });

        services.AddAuthorization();

        // 注册 JWT 服务
        services.AddSingleton<IJwtService, JwtService>();

        return services;
    }
    /// <summary>
    /// 日志服务
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddLoggingServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 设置 Serilog 输出到文本文件
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration.GetSection("Serilog"))
            .CreateLogger();
        services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

        // 启用 Exceptionless
        services.AddExceptionless(config =>
        {
            config.ApiKey = configuration["Exceptionless:ApiKey"];
            config.ServerUrl = configuration["Exceptionless:ServerUrl"];
        });

        return services;
    }
    /// <summary>
    /// GRPC服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddGrpcServices(this IServiceCollection services)
    {
        // 配置 gRPC 服务
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<GlobalInterceptor>(); // 添加全局拦截器
        });

        // 注册 gRPC 服务
        services.AddScoped<Author.AuthorBase, AuthorService>();
        services.AddScoped<AreaGrpc.AreaGrpcBase, AreaGrpcService>();
        services.AddScoped<AppSettingGrpc.AppSettingGrpcBase, AppSettingGrpcService>();
        services.AddScoped<DataDictionaryGrpc.DataDictionaryGrpcBase, DataDictionaryGrpcService>();
        //注册gRPC工厂
        services.AddSingleton<IGrpcClientFactory, BbnGrpcClientFactory>();

        return services;
    }
    /// <summary>
    /// 应用层服务
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // 注册 IHttpContextAccessor
        services.AddHttpContextAccessor();

        // 注册 AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // 注册 DapperRepository
        services.AddScoped<IDapperRepository, DapperRepository>();
        // 注册行政区划代码服务
        services.AddScoped<IAreaService, AreaService>();
        // 注册通用字典服务
        services.AddScoped<IDictionaryInitialization, DictionaryInitialization>();
        // 注册业务服务
        services.AddScoped<IOperatorService, OperatorService>();
        // 注册机构服务
        services.AddScoped<ICompanyService, CompanyService>();
        // 注册系统配置服务
        services.AddScoped<IAppSettingService, AppSettingService>();
        //注册字典服务
        services.AddScoped<IDataDictionaryService, DataDictionaryService>();
        //注册标准权限代码服务
        services.AddScoped<IOperationObjectsService, OperationObjectsService>();
        //物资代码服务
        services.AddScoped<IMaterialsCodeService, MaterialsCodeService>();
         

        return services;
    }
}
