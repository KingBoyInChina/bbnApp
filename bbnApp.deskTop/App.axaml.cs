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
        //�������ļ���ȡ������Ϣ
        // ���� appsettings.json
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        #region serilog����
        // ���� Serilog
        Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(configuration.GetSection("Serilog"))
           .CreateLogger();

        #endregion

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var services = new ServiceCollection();
            // ע�� IConfiguration
            services.AddSingleton<IConfiguration>(configuration);

            services.AddSingleton(desktop);

            var views = ConfigureViews(services);


            DataTemplates.Add(new ViewLocator(views));
            DisableAvaloniaDataAnnotationValidation();


            var provider = ConfigureServices(configuration, services);

            // ��ȡ ExceptionService ʵ��
            var exceptionService = provider.GetRequiredService<ExceptionService>();

            // ���� UI �߳��е�δ�����쳣
            Dispatcher.UIThread.UnhandledException += exceptionService.OnDispatcherUnhandledException;

            // ���ķ� UI �߳��е�δ�����쳣
            AppDomain.CurrentDomain.UnhandledException += exceptionService.OnUnhandledException;

            // �����첽�����е�δ�����쳣
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
        // ע�� AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));
        #region exceptionless ����
        var exceptionlessClient = new ExceptionlessClient();
        exceptionlessClient.Startup(configuration.GetSection("Exceptionless:ApiKey").Value.ToString());
        exceptionlessClient.Configuration.ServerUrl = configuration.GetSection("Exceptionless:ServerUrl").Value.ToString();
        #endregion
        #region grpc ע��
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
        //ע�Ṥ��
        services.AddSingleton<IGrpcClientFactory, BbnGrpcClientFactory>();
        #endregion
        #region redisע��
        // ���� Redis ����c
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetSection("Redis:ConnectionString").Value; // Redis �����ַ���
        });
        // ע�� Redis ����
        services.AddScoped<IRedisService, RedisService>();
        #endregion
        services.AddSingleton<ClipboardService>();
        services.AddSingleton<PageNavigationService>();
        services.AddSingleton<ISukiToastManager, SukiToastManager>();
        services.AddSingleton<ISukiDialogManager, SukiDialogManager>();
        services.AddSingleton<IDialog, Dialog>();
        services.AddSingleton(exceptionlessClient);//ע��exceptionless
        services.AddSingleton(Log.Logger); // ע�� ILogger
        services.AddSingleton<ExceptionService>();//ע��ȫ���쳣�������
        return services.BuildServiceProvider();
    }
}