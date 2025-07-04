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
        services.AddSingleton(Log.Logger); // ע�� ILogger
        // ע�� AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));
        #region exceptionless ����
        var exceptionlessClient = new ExceptionlessClient();
        exceptionlessClient.Startup(configuration.GetSection("Exceptionless:ApiKey").Value.ToString());
        exceptionlessClient.Configuration.ServerUrl = configuration.GetSection("Exceptionless:ServerUrl").Value.ToString();
        #endregion
        #region grpc ע��
        // �ڿͻ��˷���ע�������
        services.AddSingleton<IConsulClient>(sp =>
            new ConsulClient(config =>
            {
                config.Address = new Uri(configuration["Consul:Address"] ?? "http://localhost:5003");
            }));
        //΢���񣬲���Ҫ�̶�������GRPC��ַ
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
        //ע�Ṥ��
        //debug����ģʽ���ӳ�һ�������Ȼ����ûע��Ϳ�ʼ������
        Task.Delay(20000).GetAwaiter().GetResult();
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
        services.AddSingleton<ExceptionService>();//ע��ȫ���쳣�������
        //ע��MQTTClient����
        services.AddSingleton<MqttClientService>();

        return services.BuildServiceProvider();
    }
}