using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.IServices.IINIT;
using bbnApp.Infrastructure.Data;
using bbnApp.Protos;
using bbnApp.Service.GlobalService;
using bbnApp.Service.Services;
using Exceptionless;
using Grpc.Core;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// 添加服务注册
builder.Services
    .AddMemoryCacheServices()
    .AddDatabaseServices(builder.Configuration)
    .AddRedisCacheServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddLoggingServices(builder.Configuration)
    .AddConsulServiceDiscovery(builder.Configuration)
    .AddGrpcServices(builder.Configuration)
    .AddApplicationServices();

// 配置 HTTP 请求管道
var app = builder.Build();

// 启用认证和授权
app.UseAuthentication();
app.UseAuthorization();

// 启用 Exceptionless
app.UseExceptionless();

// 初始化数据
using (var scope = app.Services.CreateScope())
{
    // 确保 DbContext 已初始化
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var contextcode = scope.ServiceProvider.GetRequiredService<ApplicationDbCodeContext>();
    bool isLink = false;
    // 检查是否能连接到数据库
    bool canConnect = await context.Database.CanConnectAsync();
    bool codecanConnect = await contextcode.Database.CanConnectAsync();
    if (!canConnect||!codecanConnect)
    {
        bool isDbCreated = await context.Database.EnsureCreatedAsync();
        bool iscodeDbCreated = await contextcode.Database.EnsureCreatedAsync();
        if (isDbCreated&& iscodeDbCreated)
        {
            Console.WriteLine("数据库已成功创建！");
            isLink = true;
        }
    }
    else
    {
        isLink = true;
    }
    if (isLink)
    {
        //公共字典初始化
        var dictionaryService = scope.ServiceProvider.GetRequiredService<IDictionaryInitialization>();
        await dictionaryService.DictionaryInit();
        //操作员信息初始化
        var operatorService = scope.ServiceProvider.GetRequiredService<IOperatorService>();
        await operatorService.OperatorInitlize();
        //机构信息初始化
        var companyService = scope.ServiceProvider.GetRequiredService<ICompanyService>();
        await companyService.CompanyInit();
        //行政区划初始化
        var areaService = scope.ServiceProvider.GetRequiredService<IAreaService>();
        await areaService.AreaInit();
        //系统配置初始化
        var appSettingService = scope.ServiceProvider.GetRequiredService<IAppSettingService>();
        await appSettingService.AppSettingInit();
        //数据字典初始化
        var DataDictionaryService = scope.ServiceProvider.GetRequiredService<IDataDictionaryService>();
        await DataDictionaryService.DicInit();
    }
    else
    {
        Log.Warning("数据库连接失败,导致初始化数据失败");
        throw new RpcException(new Status(StatusCode.PermissionDenied, "数据库连接失败,导致初始化数据失败"));
    }
}

// 自动执行数据库迁移（生产环境下需要屏蔽）
//using (var scope = app.Services.CreateScope())
//{
//    try
//    {
//        var authordbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
//        authordbContext.Database.Migrate();

//        var codedbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbCodeContext>();
//        codedbContext.Database.Migrate();
//    }
//    catch (Exception ex)
//    {
//        Log.Warning("自动执行数据库迁移一层: {0}", ex.Message.ToString());
//        throw new RpcException(new Status(StatusCode.PermissionDenied, $"【User库】自动执行数据库迁移一层: {ex.Message.ToString()}"));
//    }
//}

// 配置 gRPC 服务
app.MapGrpcService<HealthCheckService>();
app.MapGrpcService<AuthorService>();
app.MapGrpcService<CompanyInfoGrpcService>();
app.MapGrpcService<AreaGrpcService>();
app.MapGrpcService<AppSettingGrpcService>();
app.MapGrpcService<DataDictionaryGrpcService>();
app.MapGrpcService<OperationObjectGrpcService>();
app.MapGrpcService<MaterialGrpcCodeService>();
app.MapGrpcService<DeviceCodeGrpcService>();
app.MapGrpcService<TopicCodesGrpcService>();
app.MapGrpcService<FileUploadGrpcService>();
app.MapGrpcService<GuideGrpcService>();
app.MapGrpcService<DepartMentGrpcService>();
app.MapGrpcService<EmployeeGrpcServcie>();
//app.MapGet("/health", () => Results.Ok("Healthy")); // 最简单的方式
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();