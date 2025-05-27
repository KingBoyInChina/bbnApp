using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using System.Linq;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.ViewModels;
using bbnApp.deskTop.Views;
using Avalonia.Controls;
using bbnApp.deskTop.Features.CustomTheme;
using bbnApp.deskTop.Features.Splash;
using bbnApp.deskTop.Features.Theming;
using bbnApp.deskTop.Services;
using Microsoft.Extensions.DependencyInjection;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using bbnApp.deskTop.Common;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;
using Exceptionless;
using Serilog;
using Microsoft.Extensions.Configuration;
using Grpc.Net.Client;
using System.Net.Http;
using bbnApp.Core;
using bbnApp.deskTop.AssistiveTools.AESMix;
using bbnApp.deskTop.AssistiveTools.SM2;
using bbnApp.deskTop.AssistiveTools.RSA;
using bbnApp.deskTop.AssistiveTools.AES;
using bbnApp.deskTop.AssistiveTools.Base64;
using bbnApp.deskTop.AssistiveTools.MD5;
using bbnApp.deskTop.AssistiveTools.WaterMark;
using bbnApp.deskTop.PlatformManagement.AreaCode;
using bbnApp.GrpcClients;
using bbnApp.Service.GlobalService;
using bbnApp.deskTop.Common.CommonViews;
using bbnApp.deskTop.PlatformManagement.AppSetting;
using bbnApp.deskTop.PlatformManagement.DictionaryCode;
using bbnApp.deskTop.PlatformManagement.OperationCode;
using bbnApp.deskTop.PlatformManagement.MaterialsCode;
using bbnApp.Domain.Entities.Code;
using bbnApp.deskTop.PlatformManagement.DeviceCode;

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
            
            .AddView<InputPrompt, InputPromptViewModel>(services);
    }

    private static ServiceProvider ConfigureServices(IConfiguration configuration, ServiceCollection services)
    {
        // 注册 AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));
        #region exceptionless 配置
        var exceptionlessClient = new ExceptionlessClient();
        exceptionlessClient.Startup(configuration.GetSection("Exceptionless:ApiKey").Value.ToString());
        exceptionlessClient.Configuration.ServerUrl = configuration.GetSection("Exceptionless:ServerUrl").Value.ToString();
        #endregion
        #region grpc 注入
        services.AddSingleton(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var grpcUrl = configuration.GetSection("Grpc:Url").Value;

            if (string.IsNullOrEmpty(grpcUrl))
            {
                throw new InvalidOperationException("gRPC URL is not configured.");
            }

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };


            return GrpcChannel.ForAddress(
                grpcUrl,
                new GrpcChannelOptions { HttpHandler = httpClientHandler }
            );

            //return new Author.AuthorClient(channel);
        });
        //注册工厂
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
        services.AddSingleton(Log.Logger); // 注册 ILogger
        services.AddSingleton<ExceptionService>();//注册全局异常处理服务
        return services.BuildServiceProvider();
    }
}