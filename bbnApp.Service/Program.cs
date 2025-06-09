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

// ��ӷ���ע��
builder.Services
    .AddMemoryCacheServices()
    .AddDatabaseServices(builder.Configuration)
    .AddRedisCacheServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddLoggingServices(builder.Configuration)
    .AddConsulServiceDiscovery(builder.Configuration)
    .AddGrpcServices(builder.Configuration)
    .AddApplicationServices();

// ���� HTTP ����ܵ�
var app = builder.Build();

// ������֤����Ȩ
app.UseAuthentication();
app.UseAuthorization();

// ���� Exceptionless
app.UseExceptionless();

// ��ʼ������
using (var scope = app.Services.CreateScope())
{
    // ȷ�� DbContext �ѳ�ʼ��
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var contextcode = scope.ServiceProvider.GetRequiredService<ApplicationDbCodeContext>();
    bool isLink = false;
    // ����Ƿ������ӵ����ݿ�
    bool canConnect = await context.Database.CanConnectAsync();
    bool codecanConnect = await contextcode.Database.CanConnectAsync();
    if (!canConnect||!codecanConnect)
    {
        bool isDbCreated = await context.Database.EnsureCreatedAsync();
        bool iscodeDbCreated = await contextcode.Database.EnsureCreatedAsync();
        if (isDbCreated&& iscodeDbCreated)
        {
            Console.WriteLine("���ݿ��ѳɹ�������");
            isLink = true;
        }
    }
    else
    {
        isLink = true;
    }
    if (isLink)
    {
        //�����ֵ��ʼ��
        var dictionaryService = scope.ServiceProvider.GetRequiredService<IDictionaryInitialization>();
        await dictionaryService.DictionaryInit();
        //����Ա��Ϣ��ʼ��
        var operatorService = scope.ServiceProvider.GetRequiredService<IOperatorService>();
        await operatorService.OperatorInitlize();
        //������Ϣ��ʼ��
        var companyService = scope.ServiceProvider.GetRequiredService<ICompanyService>();
        await companyService.CompanyInit();
        //����������ʼ��
        var areaService = scope.ServiceProvider.GetRequiredService<IAreaService>();
        await areaService.AreaInit();
        //ϵͳ���ó�ʼ��
        var appSettingService = scope.ServiceProvider.GetRequiredService<IAppSettingService>();
        await appSettingService.AppSettingInit();
        //�����ֵ��ʼ��
        var DataDictionaryService = scope.ServiceProvider.GetRequiredService<IDataDictionaryService>();
        await DataDictionaryService.DicInit();
    }
    else
    {
        Log.Warning("���ݿ�����ʧ��,���³�ʼ������ʧ��");
        throw new RpcException(new Status(StatusCode.PermissionDenied, "���ݿ�����ʧ��,���³�ʼ������ʧ��"));
    }
}

// �Զ�ִ�����ݿ�Ǩ�ƣ�������������Ҫ���Σ�
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
//        Log.Warning("�Զ�ִ�����ݿ�Ǩ��һ��: {0}", ex.Message.ToString());
//        throw new RpcException(new Status(StatusCode.PermissionDenied, $"��User�⡿�Զ�ִ�����ݿ�Ǩ��һ��: {ex.Message.ToString()}"));
//    }
//}

// ���� gRPC ����
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
//app.MapGet("/health", () => Results.Ok("Healthy")); // ��򵥵ķ�ʽ
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();