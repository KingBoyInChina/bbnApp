using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using bbnApp.Core;
using bbnApp.deskTop.AssistiveTools.AES;
using bbnApp.deskTop.AssistiveTools.AESMix;
using bbnApp.deskTop.AssistiveTools.Base64;
using bbnApp.deskTop.AssistiveTools.MD5;
using bbnApp.deskTop.AssistiveTools.RSA;
using bbnApp.deskTop.AssistiveTools.SM2;
using bbnApp.deskTop.AssistiveTools.WaterMark;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Common.CommonViews;
using bbnApp.deskTop.Features.CustomTheme;
using bbnApp.deskTop.Features.Splash;
using bbnApp.deskTop.Features.Theming;
using bbnApp.deskTop.OrganizationStructure.Company;
using bbnApp.deskTop.OrganizationStructure.DepartMent;
using bbnApp.deskTop.OrganizationStructure.Employee;
using bbnApp.deskTop.OrganizationStructure.Operator;
using bbnApp.deskTop.OrganizationStructure.ReigisterKey;
using bbnApp.deskTop.OrganizationStructure.Role;
using bbnApp.deskTop.PlatformManagement.AppSetting;
using bbnApp.deskTop.PlatformManagement.AreaCode;
using bbnApp.deskTop.PlatformManagement.DeviceCode;
using bbnApp.deskTop.PlatformManagement.DictionaryCode;
using bbnApp.deskTop.PlatformManagement.MaterialsCode;
using bbnApp.deskTop.PlatformManagement.OperationCode;
using bbnApp.deskTop.PlatformManagement.TopicCode;
using bbnApp.deskTop.Services;
using bbnApp.deskTop.UserCenter.UserDevice;
using bbnApp.deskTop.UserCenter.UserInformation;
using bbnApp.deskTop.ViewModels;
using bbnApp.deskTop.Views;
using bbnApp.GrpcClients;
using bbnApp.MQTT.Client;
using bbnApp.Service.GlobalService;
using Consul;
using Exceptionless;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace bbnApp.deskTop;

public partial class App :Avalonia.Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }


    public override void OnFrameworkInitializationCompleted()
    {
        //从配置文件读取配置信息
        // 加载 appsettings.json
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        #region serilog配置
        // 配置 Serilog
        Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(configuration.GetSection("Serilog"))
           .CreateLogger();

        #endregion

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();
            // 注册 IConfiguration
            services.AddSingleton<IConfiguration>(configuration);

            services.AddSingleton(desktop);

            var views = ConfigureViews(services);


            DataTemplates.Add(new ViewLocator(views));
            DisableAvaloniaDataAnnotationValidation();


            var provider = ConfigureServices(configuration, services);

            // 获取 ExceptionService 实例
            var exceptionService = provider.GetRequiredService<ExceptionService>();

            // 订阅 UI 线程中的未处理异常
            Dispatcher.UIThread.UnhandledException += exceptionService.OnDispatcherUnhandledException;

            // 订阅非 UI 线程中的未处理异常
            AppDomain.CurrentDomain.UnhandledException += exceptionService.OnUnhandledException;

            // 订阅异步任务中的未处理异常
            TaskScheduler.UnobservedTaskException += exceptionService.OnUnobservedTaskException;

            desktop.MainWindow = views.CreateView<MainWindowViewModel>(provider) as Window;
        }

        base.OnFrameworkInitializationCompleted();

        //    Shadcn.Configure(Application.Current, ThemeVariant.Dark);
    }


    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    private static BbnViews ConfigureViews(ServiceCollection services)
    {
        return new BbnViews()

            // Add main view
            .AddView<MainWindow, MainWindowViewModel>(services)

            // Add pages
            .AddView<LoginWindow, LoginWindowViewModel>(services)
            .AddView<ChangePassWordView, ChangePassWordViewModel>(services)
            .AddView<SettingView, SettingViewModel>(services)
            .AddView<SplashView, SplashViewModel>(services)
            .AddView<ThemingView, ThemingViewModel>(services)
            .AddView<AESMixPage, AESMixPageViewModel>(services)
            .AddView<SM2Page, SM2PageViewModel>(services)
            .AddView<AESPage, AESPageViewModel>(services)
            .AddView<RSAPage, RSAPageViewModel>(services)
            .AddView<Base64Page, Base64PageViewModel>(services)
            .AddView<MD5Page, MD5PageViewModel>(services)
            .AddView<WaterMarkPage, WaterMarkPageViewModel>(services)
            .AddView<AreaCodePage, AreaCodePageViewModel>(services)
            .AddView<AppSettingView, AppSettingViewModel>(services)
            .AddView<DictionaryCodeView,DictionaryCodeViewModel>(services)
            .AddView<DictionaryItemView, DictionaryItemViewModel>(services)
            .AddView<CustomThemeDialogView, CustomThemeDialogViewModel>(services)
            .AddView<MaterialsCodeView, MaterialsCodeViewModel>(services)
            .AddView<DeviceCodeView, DeviceCodeViewModel>(services)
            .AddView<OperationCodeView, OperationCodeViewModel>(services)
            .AddView<TopicCodeView, TopicCodeViewModel>(services)
            .AddView<CompanyView, CompanyViewModel>(services)
            .AddView<DepartMentView, DepartMentViewModel>(services)
            .AddView<EmployeeView, EmployeeViewModel>(services)
            .AddView<RoleView, RoleViewModel>(services)
            .AddView<OperatorView, OperatorViewModel>(services)
            .AddView<ReigisterKeyView, ReigisterKeyViewModel>(services)
            .AddView<UserInformationView, UserInformationViewModel>(services)
            .AddView<UserDeviceView, UserDeviceViewModel>(services)
            .AddView<InputPrompt, InputPromptViewModel>(services);
    }

    private static ServiceProvider ConfigureServices(IConfiguration configuration, ServiceCollection services)
    {
        services.AddSingleton(Log.Logger); // 注册 ILogger
        // 注册 AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));
        #region exceptionless 配置
        var exceptionlessClient = new ExceptionlessClient();
        exceptionlessClient.Startup(configuration.GetSection("Exceptionless:ApiKey").Value.ToString());
        exceptionlessClient.Configuration.ServerUrl = configuration.GetSection("Exceptionless:ServerUrl").Value.ToString();
        #endregion
        #region grpc 注入
        // 在客户端服务注册中添加
        services.AddSingleton<IConsulClient>(sp =>
            new ConsulClient(config =>
            {
                config.Address = new Uri(configuration["Consul:Address"] ?? "http://localhost:5003");
            }));
        //微服务，不需要固定的配置GRPC地址
        //services.AddSingleton(provider =>
        //{
        //    var configuration = provider.GetRequiredService<IConfiguration>();
        //    var grpcUrl = configuration.GetSection("Grpc:Url").Value;

        //    if (string.IsNullOrEmpty(grpcUrl))
        //    {
        //        throw new InvalidOperationException("gRPC URL is not configured.");
        //    }

        //    var httpClientHandler = new HttpClientHandler
        //    {
        //        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        //    };


        //    return GrpcChannel.ForAddress(
        //        grpcUrl,
        //        new GrpcChannelOptions { HttpHandler = httpClientHandler }
        //    );

        //    //return new Author.AuthorClient(channel);
        //});
        //注册工厂
        //debug调试模式下延迟一会儿，不然服务还没注册就开始调用了
        Task.Delay(20000).GetAwaiter().GetResult();
        services.AddSingleton<IGrpcClientFactory, BbnGrpcClientFactory>();
        #endregion
        #region redis注入
        // 配置 Redis 缓存c
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetSection("Redis:ConnectionString").Value; // Redis 连接字符串
        });
        // 注册 Redis 服务
        services.AddScoped<IRedisService, RedisService>();
        #endregion
        services.AddSingleton<ClipboardService>();
        services.AddSingleton<PageNavigationService>();
        services.AddSingleton<ISukiToastManager, SukiToastManager>();
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        services.AddSingleton<IDialog, Dialog>();
        services.AddSingleton(exceptionlessClient);//注册exceptionless
        services.AddSingleton<ExceptionService>();//注册全局异常处理服务
        //注册MQTTClient服务
        services.AddSingleton<MqttClientService>();

        return services.BuildServiceProvider();
    }
}