using AutoMapper;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Styling;
using Avalonia.Threading;
using bbnApp.Application.DTOs.LoginDto;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Features.CustomTheme;
using bbnApp.deskTop.Features.Theming;
using bbnApp.deskTop.Services;
using bbnApp.deskTop.Utilities;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.MQTT.Client;
using bbnApp.Protos;
using bbnApp.Share;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Exceptionless;
using Grpc.Core;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using Serilog;
using SukiUI;
using SukiUI.Dialogs;
using SukiUI.Enums;
using SukiUI.Models;
using SukiUI.Theme.Shadcn;
using SukiUI.Toasts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Tmds.DBus.Protocol;

namespace bbnApp.deskTop.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        /// <summary>
        /// 机构清单
        /// </summary>
        private List<CompanyItemDto> CompanyItems { get; set; }
        /// <summary>
        /// 当前登录的操作员信息
        /// </summary>
        [ObservableProperty] private UserModel _loginUser;
        /// <summary>
        /// 顶部菜单
        /// </summary>
        [ObservableProperty] private AvaloniaList<TopMenuItemDto> _topMenuItems=new AvaloniaList<TopMenuItemDto> ();
        /// <summary>
        /// 页面字典
        /// </summary>
        private Dictionary<string, IAvaloniaReadOnlyList<BbnPageBase>> pagesDic = new Dictionary<string, IAvaloniaReadOnlyList<BbnPageBase>>();
        /// <summary>
        /// title
        /// </summary>
        [ObservableProperty] public string _title =DateTime.Now.Hour>=18?"晚上好！请登录":DateTime.Now.Hour>12?"下午好！请登录":DateTime.Now.Hour>=6?"上午好！请登录":"夜深了!该休息了";
        /// <summary>
        /// 窗口状态
        /// </summary>
        [ObservableProperty] public WindowState _windowState = WindowState.Maximized;
        /// <summary>
        /// 配置的主题
        /// </summary>
        private JObject ObjTheme=null!;
        /// <summary>
        /// 主题设置
        /// </summary>
        void ThemeSetting()
        {
            string theme = "Dark";
            string color = "Blue";
            string style = "Gradient";
            if (ObjTheme != null)
            {
                theme = ObjTheme["basicTheme"] == null ? "Dark" : ObjTheme["basicTheme"].ToString();
                color = ObjTheme["colorTheme"] == null ? "Blue" : ObjTheme["colorTheme"].ToString();
                style = ObjTheme["style"] == null ? "Gradient" : ObjTheme["style"].ToString();
            }
            
            ThemeVariant _theme=theme== "Dark" ? ThemeVariant.Dark : ThemeVariant.Light;
            SukiColor _color= color == "Blue" ? SukiColor.Blue : color == "Red" ? SukiColor.Red: color == "Green" ? SukiColor.Green : color == "Orange" ? SukiColor.Orange: SukiColor.Blue;
            SukiTheme.GetInstance().ChangeBaseTheme(_theme);
            SukiTheme.GetInstance().ChangeColorTheme(SukiColor.Blue);

            if (Enum.TryParse(style, out SukiBackgroundStyle stylevalue))
            {
                _theming.BackgroundStyle = stylevalue;
            }
            else
            {
                _theming.BackgroundStyle = SukiBackgroundStyle.Gradient;
            }
        }

        public IAvaloniaReadOnlyList<BbnPageBase> BbnPages { get; }
        
        public IAvaloniaReadOnlyList<SukiColorTheme> Themes { get; }

        public IAvaloniaReadOnlyList<SukiBackgroundStyle> BackgroundStyles { get; }

        public ISukiToastManager ToastManager { get; }
        public ISukiDialogManager DialogManager { get; }
        private readonly IDialog dialog;
        [ObservableProperty] private string _selectedToolTag;
        [ObservableProperty] private IAvaloniaReadOnlyList<BbnPageBase> _selectedPages;
        [ObservableProperty] private ThemeVariant _baseTheme;
        [ObservableProperty] private BbnPageBase? _activePage;
        [ObservableProperty] private bool _windowLocked;
        [ObservableProperty] private bool _titleBarVisible = true;
        [ObservableProperty] private SukiBackgroundStyle _backgroundStyle = SukiBackgroundStyle.GradientSoft;
        [ObservableProperty] private bool _animationsEnabled;
        [ObservableProperty] private string? _customShaderFile;
        [ObservableProperty] private bool _transitionsEnabled;
        [ObservableProperty] private double _transitionTime;
        [ObservableProperty] private bool _isLightTheme;
        [ObservableProperty] private bool _backgroundAnimations;
        [ObservableProperty] private bool _backgroundTransitions;
        [ObservableProperty] private string _windowLockedInfo= "窗体锁定";
        [ObservableProperty] private string _toggleTitleBarInfo= "隐藏标题栏";
        [ObservableProperty] private string _toggleRightToLeftInfo = "切换到右边";
        [ObservableProperty] private string _toggleTitleBackgroundInfo = "显示工具栏背景";
        [ObservableProperty] private bool _isFullScreen = false;
        [ObservableProperty] private bool _isShadCn=false;
        [ObservableProperty] private bool _isLogined=false;
        public IAvaloniaReadOnlyList<string> CustomShaders { get; } = new AvaloniaList<string> { "Space", "Weird", "Clouds" };
        public Action<string?>? CustomBackgroundStyleChanged { get; set; }
        [ObservableProperty] private bool _showTitleBar = true;
        [ObservableProperty] private bool _showBottomBar = true;
        private readonly SukiTheme _theme;
        private readonly ThemingViewModel _theming;
        private string? _customShader = null;
        /// <summary>
        /// 应用定义命令
        /// </summary>
        public ReactiveCommand<Unit, Unit> AppExitCommand { get; }
        /// <summary>
        /// 自定义命令回调
        /// </summary>
        private void AppExitViewFunction(){}
        /// <summary>
        /// 
        /// </summary>
        private ILogger? _ILogger;
        /// <summary>
        /// 
        /// </summary>
        private ExceptionlessClient? _ExceptionlessClient;
        /// <summary>
        /// 
        /// </summary>
        private Author.AuthorClient _client;
        /// <summary>
        /// 机构服务
        /// </summary>
        private CompanyGrpcService.CompanyGrpcServiceClient _companyclient;
        /// <summary>
        /// 注册密钥服务
        /// </summary>
        private ReigisterKeyGrpcService.ReigisterKeyGrpcServiceClient _authorKeyClient;
        /// <summary>
        /// 地区代码服务
        /// </summary>
        private AreaGrpc.AreaGrpcClient _codeClient;
        /// <summary>
        /// 系统配置服务
        /// </summary>
        private AppSettingGrpc.AppSettingGrpcClient _appSettignClient;
        /// <summary>
        /// 字典服务
        /// </summary>
        private DataDictionaryGrpc.DataDictionaryGrpcClient _dictionaryClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IGrpcClientFactory _grpcClientFactory;
        /// <summary>
        /// 
        /// </summary>
        private readonly IRedisService _redisService;
        /// <summary>
        /// 映射
        /// </summary>
        private readonly IMapper? _mapper;
        /// <summary>
        /// 
        /// </summary>
        private readonly IConfiguration configuration;
        /// <summary>
        /// MQTTClient服务
        /// </summary>
        private readonly MqttClientService mqttClientService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="client"></param>
        /// <param name="exceptionlessClient"></param>
        /// <param name="wstcPages"></param>
        /// <param name="pageNavigationService"></param>
        /// <param name="toastManager"></param>
        /// <param name="dialogManager"></param>
        public MainWindowViewModel(IMapper mapper,ILogger logger, IGrpcClientFactory grpcClientFactory, ExceptionlessClient exceptionlessClient
            ,IEnumerable<BbnPageBase> wstcPages, PageNavigationService pageNavigationService, IDialog dialog,ISukiToastManager toast
            ,  ISukiDialogManager dialogManager,IRedisService redisService, IConfiguration configuration, MqttClientService mqttClientService
            )
        {
            _redisService =redisService; 
            _ILogger = logger;
            _mapper = mapper;
            _ExceptionlessClient = exceptionlessClient;
            _grpcClientFactory = grpcClientFactory;
            this.configuration = configuration;
            this.mqttClientService = mqttClientService;
            _ =ClientInit();
            this.dialog = dialog;
            DialogManager = dialogManager;
            ToastManager = toast;

            BbnPages = new AvaloniaList<BbnPageBase>(wstcPages.OrderBy(x => x.Index).ThenBy(x => x.DisplayName));
            _theming = (ThemingViewModel)BbnPages.First(x => x is ThemingViewModel);
            _theming.BackgroundStyleChanged += style => BackgroundStyle = style;
            _theming.BackgroundAnimationsChanged += enabled => AnimationsEnabled = enabled;
            _theming.CustomBackgroundStyleChanged += shader => CustomShaderFile = shader;
            _theming.BackgroundTransitionsChanged += enabled => TransitionsEnabled = enabled;

            //参数配置
            _ = ParamSettingLoad();

            BackgroundStyles = new AvaloniaList<SukiBackgroundStyle>(Enum.GetValues<SukiBackgroundStyle>());
            _theme = SukiTheme.GetInstance();
            // Subscribe to the navigation service (when a page navigation is requested)
            pageNavigationService.NavigationRequested += pageType =>
            {
                var page = BbnPages.FirstOrDefault(x => x.GetType() == pageType);
                if (page is null || ActivePage?.GetType() == pageType) return;
                ActivePage = page;
            };

            IsLightTheme = _theme.ActiveBaseTheme == ThemeVariant.Light;

            Themes = _theme.ColorThemes;
            BaseTheme = _theme.ActiveBaseTheme;

            // Initialize non-nullable fields
            _selectedToolTag = string.Empty;
            _selectedPages = new AvaloniaList<BbnPageBase>();

            // Subscribe to the base theme changed events
            _theme.OnBaseThemeChanged += variant =>
            {
                IsLightTheme = variant == ThemeVariant.Light;
                BaseTheme = variant;
            };

            // Subscribe to the color theme changed events
            _theme.OnColorThemeChanged += theme => { };

            AppExitCommand = ReactiveCommand.Create(AppExitViewFunction);
        }
        /// <summary>
        /// Client 初始化
        /// </summary>
        private async Task ClientInit()
        {
            _companyclient = await _grpcClientFactory.CreateClient<CompanyGrpcService.CompanyGrpcServiceClient>();
            _client = await _grpcClientFactory.CreateClient<Author.AuthorClient>();
            _codeClient = await _grpcClientFactory.CreateClient<AreaGrpc.AreaGrpcClient>();
            _appSettignClient = await _grpcClientFactory.CreateClient<AppSettingGrpc.AppSettingGrpcClient>();
            _dictionaryClient =await _grpcClientFactory.CreateClient<DataDictionaryGrpc.DataDictionaryGrpcClient>();
            _authorKeyClient = await _grpcClientFactory.CreateClient<ReigisterKeyGrpcService.ReigisterKeyGrpcServiceClient>();
        }
        /// <summary>
        /// 主题变更
        /// </summary>
        /// <param name="value"></param>
        partial void OnIsLightThemeChanged(bool value) {
            _theme.ChangeBaseTheme(value ? ThemeVariant.Light : ThemeVariant.Dark);
        }
        /// <summary>
        /// 动画背景变更
        /// </summary>
        [RelayCommand]
        private void ToggleAnimations()
        {
            AnimationsEnabled = !AnimationsEnabled;
            dialog.Tips("动画效果变更", AnimationsEnabled ? "背景动画可用" : "背景动画禁用");
        }
        /// <summary>
        /// 过渡效果变更
        /// </summary>
        [RelayCommand]
        private void ToggleTransitions()
        {
            TransitionsEnabled = !TransitionsEnabled;
            dialog.Tips("过渡效果变更", TransitionsEnabled ? "背景过渡效果已开启" : "背景过渡效果已禁用");
        }
        /// <summary>
        /// 基本主题变更
        /// </summary>
        [RelayCommand]
        private void ToggleBaseTheme()
        {
            _theme.SwitchBaseTheme();
            IsAuto = false;
        }
        /// <summary>
        /// 主题颜色变更
        /// </summary>
        /// <param name="colorTheme"></param>
        [RelayCommand]
        private void SwitchToColorTheme(SukiColorTheme colorTheme)
        {
            _theme.ChangeColorTheme(colorTheme);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="theme"></param>
        public void ChangeTheme(SukiColorTheme theme)
        {
            _theme.ChangeColorTheme(theme);
            IsAuto = false;
        }
        /// <summary>
        /// ShadCn模式变更
        /// </summary>
        [RelayCommand]
        private void ShadCnMode()
        {
            try
            {
                // Replace the problematic line:  
                // var currentApplication = Application.Current;  

                // With the following code:  
                var currentApplication = Avalonia.Application.Current;
                if (currentApplication != null)
                {
                    IsShadCn = true;
                    Shadcn.Configure(currentApplication, currentApplication.ActualThemeVariant);
                }
                else
                {
                    // Handle the case where Application.Current is null
                    dialog.Error("错误", "无法配置Shadcn模式，因为当前应用程序实例为null。");
                }
            }
            catch(Exception ex)
            {
                dialog.Error("错误", ex.Message.ToString());
                
                ExceptionService.HandleException(_ILogger,_ExceptionlessClient, ex, "ShadCnModeCommand");
            }
        }
        /// <summary>
        /// 自定义主题
        /// </summary>
        [RelayCommand]
        private void CreateCustomTheme()
        {
            DialogManager.CreateDialog()
                .WithViewModel(dialog => new CustomThemeDialogViewModel(_theme, dialog))
                .TryShow();
        }
        /// <summary>
        /// 锁定状态变更
        /// </summary>
        [RelayCommand]
        private void ToggleWindowLock()
        {
            WindowLocked = !WindowLocked;
            WindowLockedInfo=WindowLocked ? "窗体解锁" : "窗体锁定";

            dialog.Tips("窗体状态变更", $"窗体已经 {(WindowLocked ? "锁定" : "解锁")}.");
        }
        /// <summary>
        /// 背景栏状态变更
        /// </summary>
        [RelayCommand]
        private void ToggleTitleBackground()
        {
            ShowTitleBar = !ShowTitleBar;
            ShowBottomBar = !ShowBottomBar;
            ToggleTitleBackgroundInfo = ShowTitleBar ? "隐藏工具栏背景" : "显示工具栏背景";
            dialog.Tips("背景栏状态变更", $"窗体工具栏背景 {(ShowTitleBar ? "可见" : "隐藏")}.");
        }
        /// <summary>
        /// 标题栏状态变更
        /// </summary>
        [RelayCommand]
        private void ToggleTitleBar()
        {
            TitleBarVisible = !TitleBarVisible;
            ToggleTitleBarInfo= TitleBarVisible ? "隐藏标题栏" : "显示标题栏";
            dialog.Tips("标题栏状态变更", $"窗体标题栏 {(TitleBarVisible ? "可见" : "隐藏")}.");
        }
        /// <summary>
        /// 菜单栏左右切换
        /// </summary>
        [RelayCommand]
        private void ToggleRightToLeft()
        {
            _theme.IsRightToLeft = !_theme.IsRightToLeft;
            ToggleRightToLeftInfo=_theme.IsRightToLeft ? "切换到左边" : "切换到右边";
        }
        /// <summary>
        /// 新开页面
        /// </summary>
        /// <param name="url"></param>
        [RelayCommand]
        private static void OpenUrl(string url) => UrlUtilities.OpenUrl(url);
        /// <summary>
        /// 背景色变更事件
        /// </summary>
        /// <param name="value"></param>
        partial void OnBackgroundStyleChanged(SukiBackgroundStyle value)
        {
            _theming.BackgroundStyle = value;
        }
        /// <summary>
        /// 动画背景切换事件
        /// </summary>
        /// <param name="value"></param>
        partial void OnAnimationsEnabledChanged(bool value) =>
            _theming.BackgroundAnimations = value;
        /// <summary>
        /// 过渡背景切换事件
        /// </summary>
        /// <param name="value"></param>
        partial void OnTransitionsEnabledChanged(bool value) =>
            _theming.BackgroundTransitions = value;
        /// <summary>
        /// 自定义颜色
        /// </summary>
        /// <param name="shaderType"></param>
        [RelayCommand]
        private void TryCustomShader(string shaderType)
        {
            _customShader = _customShader == shaderType ? null : shaderType;
            CustomBackgroundStyleChanged?.Invoke(_customShader);
        }
        /// <summary>
        /// 初始化完毕
        /// </summary>
        private bool PageInited = false;
        /// <summary>
        /// 选择
        /// </summary>
        /// <param name="tag"></param>
        [RelayCommand]
        private async Task AppTagSelected(object? sender)
        {
            if (PageInited)
            {
                if (LoginUser == null)
                {
                    dialog.Error("提示", "无效的登录信息,请先登录");
                }
                else
                {
                    if (sender is not MenuItem item) return;
                    string tag = item.Tag.ToString();
                    if (SelectedToolTag != tag)
                    {
                        IAvaloniaReadOnlyList<BbnPageBase> pagelist;
                        if (pagesDic.ContainsKey(tag))
                        {
                            pagesDic.TryGetValue(tag, out pagelist);
                        }
                        else
                        {
                            pagelist = new AvaloniaList<BbnPageBase>(BbnPages.Where(x => x.Tag == item.Tag.ToString()).ToList());
                            pagesDic.Add(tag, pagelist);
                        }
                        dialog.ShowLoading($"页面正在载入中,请稍等...", async (e) =>
                        {
                            SelectedPages = new AvaloniaList<BbnPageBase>(pagelist);
                            SelectedToolTag = tag;
                            await Task.Delay(500);
                            dialog.LoadingClose(e);
                        });

                    }
                }
            }
        }
        /// <summary>
        /// 登录验证
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private void LoginCheck()
        {
            OpenLoginWindow();
        }
        /// <summary>
        /// 登录窗口
        /// </summary>
        public void OpenLoginWindow()
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                bool b = DialogManager.CreateDialog().ShowCardBackground(false)
                .WithViewModel(window => new LoginWindowViewModel(window, _client, _companyclient, dialog,LoginCallBack, CompanyItems, _redisService,_mapper))
                .TryShow();
            }
            );
        }
        /// <summary>
        /// 登录回调
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="logined"></param>
        /// <param name="value"></param>
        /// <param name="response"></param>
        public void LoginCallBack(ISukiDialog window, bool logined, string value,LoginResponse? response)
        {
            if (logined)
            {
                IsLogined = true;
                TopMenuItems = _mapper.Map<AvaloniaList<TopMenuItemDto>>(response.TopMenus);
                UserInfoDto userInfo = _mapper.Map<UserInfoDto>(response.UserInfo);
                //这里因为字段属性不一致，所以需要手动映射
                LoginUser = new UserModel {
                    Yhid = userInfo.Yhid,
                    AreaCode = userInfo.AreaCode,
                    AreaName = userInfo.AreaName,
                    DateOfEmployment = userInfo.DateOfEmployment,
                    EmailNum = userInfo.EmailNum,
                    Expires = CommMethod.GetValueOrDefault(userInfo.Expires, DateTime.Now),
                    IsLock = userInfo.IsLock,
                    OperatorId = userInfo.OperatorId,
                    Token = userInfo.Token,
                    CompanyId = userInfo.CompanyId,
                    CompanyName = userInfo.CompanyName,
                    DepartMentId = userInfo.DepartMentId,
                    DepartMentName = userInfo.DepartMentName,
                    EmployeeId = userInfo.EmployeeId,
                    EmployeeName = userInfo.EmployeeName,
                    EmployeeNum = userInfo.EmployeeNum,
                    PhoneNum = userInfo.PhoneNum,
                    Position = userInfo.Position,
                    PositionLeve = userInfo.PositionLeve,
                    PassWordExpTime =CommMethod.GetValueOrDefault(userInfo.PassWordExpTime,DateTime.Now),
                };
                UserContext.CurrentUser = LoginUser;
                //变更Tilte
                Title = $"{LoginUser.CompanyName}-{LoginUser.DepartMentName}";
                window.Dismiss();

                dialog.Success("登录成功", value);

                //以防万一，再执行一次Dialog关闭
                DialogManager.DismissDialog();
                //校验密码是否到期
                PassWordExpCheck(Convert.ToDateTime(LoginUser.PassWordExpTime));
                //主题切换定时器开启
                if (_autoThemeTimer == null && IsAuto)
                {
                    #region 开启主题切换定时器
                    _autoThemeTimer = new Timer(600000); // 600秒
                    _autoThemeTimer.Elapsed += OnThemeCheckTimerElapsed;
                    _autoThemeTimer.Start();
                    #endregion
                }
                //初始化字典信息
                DicInit();
            }
            else
            {
                //退出程序
                AppExitCommand.Execute().Subscribe();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        private  void PassWordExpCheck(DateTime dt)
        {
            if (DateTime.Now.CompareTo(dt)>0)
            {
                ShowPassWordWindow("密码到期,请立即修改！");
            }
        }
        /// <summary>
        /// 注销登录或退出确定
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public void ExitComfirm(string title,Action<bool> ac)
        {
            DialogManager.CreateDialog()
                .WithTitle("操作提示")
                .WithContent(title)
                .OfType(NotificationType.Information)
            .WithActionButton("确认", async _ => {

                if (mqttClientService != null)
                {
                    mqttClientService.ClearHandlers();
                    await mqttClientService.Disconnect();
                }

                await ThemeInfoSave();
                TimerDispose();//销毁定时器
                ac(true); 
            },true, "Outlined")
            .WithActionButton("取消", _ => {
                ac(false); 
            }, true,"Flat") 
            .TryShow();
        }
        /// <summary>
        /// 密码修改
        /// </summary>
        [RelayCommand]
        public void PassWordWindow()
        {
            ShowPassWordWindow();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        private void ShowPassWordWindow(string info="")
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                bool b = DialogManager.CreateDialog().ShowCardBackground(false)
                .WithViewModel(window => new ChangePassWordViewModel(window, dialog, _client, _ILogger, _ExceptionlessClient, PassWordWindowCallBack, info))
                .TryShow();
            }
            );
        }
        /// <summary>
        /// 登录回调
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="logined"></param>
        /// <param name="value"></param>
        /// <param name="response"></param>
        private void PassWordWindowCallBack(ISukiDialog window, PassWordResponse? response)
        {
            LoginUser.PassWordExpTime =Convert.ToDateTime(response?.PassWordExpTime);
            UserContext.CurrentUser = LoginUser;
            window.Dismiss();
            dialog.Success("修改成功");
            //以防万一，再执行一次Dialog关闭
            DialogManager.DismissDialog();
        }
        /// <summary>
        /// 系统参数配置
        /// </summary>
        [RelayCommand]
        private void ParameSetting() {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                bool b = DialogManager.CreateDialog().ShowCardBackground(false)
                .WithViewModel(window => new SettingViewModel(window, dialog, ParameSettingCallBack))
                .TryShow();
            }
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dialog"></param>
        /// <param name="b"></param>
        private void ParameSettingCallBack(ISukiDialog window,bool b)
        {
            if (b)
            {
                window.Dismiss();
                dialog.Success("参数提交", "参数保存完成!");
                //以防万一，再执行一次Dialog关闭
                DialogManager.DismissDialog();
            }
        }
        #region 系统配置参数读取和事务
        /// <summary>
        /// 系统配置
        /// </summary>
        private JObject ObjSetting;
        private Timer _autoThemeTimer;
        /// <summary>
        /// 读取系统配置参数
        /// </summary>
        private async Task ParamSettingLoad()
        {
            try
            {
                if(File.Exists(Path.Combine("settings", "parameSetting.json")))
                {
                    string settinginfo =await File.ReadAllTextAsync(Path.Combine("settings", "parameSetting.json"));
                    if (!string.IsNullOrEmpty(settinginfo))
                    {
                        string parame = EncodeAndDecode.MixDecrypt(settinginfo);
                        ObjSetting = JObject.Parse(parame);
                        if (ObjSetting["theme"] != null)
                        {
                            ObjTheme = JObject.Parse(ObjSetting["theme"].ToString());
                        }
                        else
                        {
                            ObjTheme = new JObject {
                                { "basicTheme","Blue"},
                                { "colorTheme","Dark"},
                                { "style","Gradient"}
                            };
                        }
                        ThemeSetting();
                        autoTheme();
                    }
                }
            }
            catch(Exception ex)
            {
                _ExceptionlessClient.SubmitException(new Exception($"系统配置参数读取异常：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// 主题切换时间
        /// </summary>
        private int lightHour;
        private int darkHour;
        /// <summary>
        /// 是否是自动主题切换状态，如果是手动切换过则设置为fale
        /// </summary>
        private bool IsAuto=false;
        /// <summary>
        /// 主题自动切换
        /// </summary>
        private void autoTheme()
        {
            if (ObjSetting != null)
            {
                JToken? autoThemeToken = ObjSetting["autotheme"];
                JObject? ObjAutoTheme = autoThemeToken == null ? null : JObject.Parse(autoThemeToken.ToString());
                if (ObjAutoTheme != null)
                {
                    bool use = Convert.ToBoolean(ObjAutoTheme["use"]);
                    lightHour = ObjAutoTheme["lightHour"]==null?-1: Convert.ToInt32(ObjAutoTheme["lightHour"]);
                    darkHour = ObjAutoTheme["darkHour"] == null ? -1 : Convert.ToInt32(ObjAutoTheme["darkHour"]);
                    if (use && lightHour > -1 && darkHour > -1)
                    {
                        IsAuto = true;
                    }
                }
            }
        }
        /// <summary>
        /// 如果当前主题与需要自动切换的主题不一致则自动切换
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnThemeCheckTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.UIThread.InvokeAsync(() =>
            {
                try
                {
                    SukiTheme theme = SukiTheme.GetInstance();
                    ThemeVariant _theme;
                    if (DateTime.Now.Hour >= lightHour && DateTime.Now.Hour < darkHour)
                    {
                        _theme = ThemeVariant.Light;
                    }
                    else
                    {
                        _theme = ThemeVariant.Dark;
                    }
                    if (theme.ActiveBaseTheme != _theme && IsAuto)
                    {
                        SukiTheme.GetInstance().ChangeBaseTheme(_theme);
                        dialog.Tips("主题切换提示", "已按照您设置的主题自动变更时间自动切换主题",3, NotificationType.Information);
                    }
                }
                catch (Exception ex)
                {
                    dialog.Error("异常提示", $"自动主题切换产生异常:{ex.Message.ToString()}");
                }
            }
            );
        }
        /// <summary>
        /// 定时器销毁
        /// </summary>
        private void TimerDispose()
        {
            if (_autoThemeTimer!=null)
            {
                _autoThemeTimer.Stop();
                _autoThemeTimer.Dispose();
            }
        }
        #endregion
        #region 应用退出时，主题信息存储
        /// <summary>
        /// 主题信息存储
        /// </summary>
        private async Task ThemeInfoSave()
        {
            try
            {
                ThemeVariant BaseTheme = _theme.ActiveBaseTheme;
                SukiColorTheme? colorTheme= _theme.ActiveColorTheme;
                SukiBackgroundStyle style = _theming.BackgroundStyle;
                string basicTheme = BaseTheme == ThemeVariant.Dark ? "Dark" : "Light";
                string color = colorTheme == null ? "Blue" : colorTheme.DisplayName;
                string backgroundstyle =style==null? "Gradient":  style.ToString();

                JObject ObjData = new JObject {

                        { "theme",new JObject{
                                { "basicTheme",basicTheme},
                                { "colorTheme",color},
                                { "style",backgroundstyle}
                            }
                        }
                };
                if (ObjSetting == null)
                {
                    ObjSetting = new JObject {
                        ObjData
                    };
                }
                else
                {
                    ObjSetting.Merge(ObjData);
                }
                //配置信息存储
                if (!File.Exists(Path.Combine("settings", "parameSetting.json")))
                {
                    File.Create(Path.Combine("settings", "parameSetting.json")).Dispose();
                }
                string settinginfo = EncodeAndDecode.MixEncrypt(ObjSetting.ToString());
                await File.WriteAllBytesAsync(Path.Combine("settings", "parameSetting.json"),Encoding.UTF8.GetBytes(settinginfo));
                
            }
            catch(Exception ex)
            {

            }
        }
        #endregion
       
        /// <summary>
        /// 初始化机构信息
        /// </summary>
        /// <returns></returns>
        public void CompanyInit(Action<bool> ac)
        {
            try
            {
                dialog.ShowLoading("数据初始化中...", async (e) =>
                {
                    var (b,msg,data) = await BasicRequest.CompanyItemsLoad(_companyclient, _mapper);
                    if (!b)
                    {
                        dialog.Error("提示",msg);
                    }
                    CompanyItems = data;
                    ac(b);
                    dialog.LoadingClose(e);
                });
            }
            catch (Exception ex)
            {
                dialog.Error("机构初始化失败", ex.Message.ToString());
            }
        }
        #region 初始化字典信息
        /// <summary>
        /// 手动刷新字典
        /// </summary>
        [RelayCommand]
        private void DicReload()
        {
            DicInit();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private void DicInit()
        {
            try
            {
                dialog.ShowLoading("系统核心参数装载中...", async (e) =>
                {
                    var header = CommAction.GetHeader();
                    await AreaCodeInit(header);//行政区划下载
                    await AppSettingDownload(header);//系统配置下载
                    await DicDataDownload(header);//数据字典下载
                    dialog.LoadingClose(e);
                    PageInited = true;
                    //MQTT初始化
                    await MqttInit();
                },500);

            }
            catch(Exception ex)
            {
                dialog.Error("字典初始化失败", ex.Message.ToString());
            }
        }

        /// <summary>
        /// 初始化行政区划字典
        /// </summary>
        /// <returns></returns>
        private async Task AreaCodeInit(Metadata header)
        {
            AreaTreeNodeRequestDto request = new AreaTreeNodeRequestDto { AreaCode = string.Empty, AreaLeve = 0 };
            AreaTreeNodeResponse response =await _codeClient.AreaTreeLoadAsync(_mapper.Map<AreaTreeNodeRequest>(request),header);
            if(response.Code)
            {
                List<AreaTreeNodeDto> list = _mapper.Map<List<AreaTreeNodeDto>>(response.Items);
                DicContext.AreaTree = list;
            }
            else
            {
                dialog.Error("错误提示", $"行政区划字典初始化失败:{response.Message}");
            }
        }
        /// <summary>
        /// 下载系统配置信息
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private async Task AppSettingDownload(Metadata header)
        {
            AppSettingDownloadRequestDto request = new AppSettingDownloadRequestDto { };
            var response = await _appSettignClient.AppSettingDownloadAsync(_mapper.Map<AppSettingDownloadRequest>(request), header);
            if (response.Code)
            {
                DicContext.AppSettingList = _mapper.Map<List<AppSettingDto>>(response.Items);
            }
            else
            {
                dialog.Error("错误提示", $"系统配置参数初始化失败:{response.Message}");
            }
        }
        /// <summary>
        /// 数据字典下载
        /// </summary>
        /// <param name="header"></param>
        /// <returns></returns>
        private async Task DicDataDownload(Metadata header)
        {
            DataDictionaryDownloadRequestDto request = new DataDictionaryDownloadRequestDto { };
            var response = await _dictionaryClient.DicDownloadAsync(_mapper.Map<DataDictionaryDownloadRequest>(request), header);
            if (response.Code)
            {
                DicContext.DicItems = _mapper.Map<List<DicTreeItemDto>>(response.Item);
            }
            else
            {
                dialog.Error("错误提示", $"数据字典初始化失败:{response.Message}");
            }
        }
        #endregion
        #region MQTT初始化
        /// <summary>
        /// 
        /// </summary>
        private AuthorReginsterKeyClientDto AuthorKey;
        /// <summary>
        /// MQTT连接状态
        /// </summary>
        [ObservableProperty] private bool _mqttLink = false;
        /// <summary>
        /// 默认的订阅主题
        /// </summary>
        private string[] topics = [];
        /// <summary>
        /// MQTT连接状态
        /// </summary>
        [RelayCommand]
        private void MqttConnect()
        {
            if (!MqttLink)
            {
                dialog.ShowLoading("通信连接中...",async(e) =>{
                    try
                    {
                        if (AuthorKey == null)
                        {
                            await MqttInit();
                        }
                        else
                        {
                            string ip = configuration.GetSection("MQTT:IP").Value?.ToString() ?? string.Empty;
                            int port = string.IsNullOrEmpty(configuration.GetSection("MQTT:Port").Value?.ToString()) ? int.MinValue : Convert.ToInt32(configuration.GetSection("MQTT:Port").Value);
                            if (!string.IsNullOrEmpty(ip))
                            {
                                await MqttConnect(ip, port);
                            }
                        }
                    }
                    catch(Exception ex)
                    {
                        dialog.Tips("提示", ex.Message.ToString());
                    }
                    finally
                    {
                        dialog.LoadingClose(e);
                    }
                });
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private async Task MqttInit()
        {
            try
            {
                string ip = configuration.GetSection("MQTT:IP").Value?.ToString() ?? string.Empty;
                int port =string.IsNullOrEmpty(configuration.GetSection("MQTT:Port").Value?.ToString())?int.MinValue:Convert.ToInt32(configuration.GetSection("MQTT:Port").Value);
                if (!string.IsNullOrEmpty(ip))
                {
                    var request = new AuthorReginsterKeyInfoRequestDto { 
                        CompanyId=UserContext.CurrentUser.CompanyId ,
                        Yhid=UserContext.CurrentUser.Yhid,
                        OperatorId=UserContext.CurrentUser.OperatorId 
                    };
                    var response = await _authorKeyClient.AuthorRegisterInfoAsync(_mapper.Map<AuthorReginsterKeyInfoRequest>(request),CommAction.GetHeader());
                    if (response.Code)
                    {
                        AuthorKey =_mapper.Map< AuthorReginsterKeyClientDto >(response.Item);
                        topics= [
                            $"/Private/Operator/{UserContext.CurrentUser.OperatorId}/Notice",
                            $"/Private/Operator/{UserContext.CurrentUser.OperatorId}/Message",
                        ];
                        await MqttConnect(ip,port);
                    }
                    else
                    {
                        dialog.Tips("提示", response.Message);
                    }
                }
                else
                {
                    dialog.Tips("提示","MQTT参数尚未配置");
                }
            }
            catch(Exception ex)
            {
                dialog.Error("错误提示", $"MQTT初始化异常:{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 连接和初始化订阅主题
        /// </summary>
        private async Task MqttConnect(string ip,int port) {
            
            await mqttClientService.Connect(ip,port,$"bbnAdmin_{UserContext.CurrentUser.OperatorId}",AuthorKey.AppId,AuthorKey.SecriteKey);
            //订阅默认主题(主应用一般只订阅和当前登录人员有关的通知消息类主题)     
            foreach(string topic in topics)
            {
                await mqttClientService.Subscribe(topic);
                mqttClientService.RegisterHandler(topic, OnDataReceived);
            }
            MqttLink = true;
        }
        /// <summary>
        /// 接收到的消息
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="payload"></param>
        private void OnDataReceived(string topic, string payload)
        {
            if (topic.EndsWith("Notice")|| topic.EndsWith("Message"))
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    dialog.Tips("新通知", payload,5, topic.EndsWith("Message")?NotificationType.Success:NotificationType.Information);
                });
            }
        }
        #endregion
    }
}
