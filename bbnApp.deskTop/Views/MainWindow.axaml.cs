using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Avalonia.Threading;
using SukiUI.Controls;
using SukiUI.Dialogs;
using SukiUI.Models;
using System;
using System.Runtime.CompilerServices;
using bbnApp.deskTop.ViewModels;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Notifications;

namespace bbnApp.deskTop.Views;

public partial class MainWindow : SukiWindow
{
    /// <summary>
    /// 关闭状态
    /// </summary>
    private bool _isClosing = false;
    /// <summary>
    /// 点击关闭按钮关闭需要弹出提示
    /// </summary>
    private bool _showClosingConfirm = true;
    public MainWindow()
    {
        InitializeComponent();
        if (RuntimeFeature.IsDynamicCodeCompiled == false)
        {
            Title += " (native)";
        }
        // 订阅 Loaded 事件
        this.Loaded +=OnMainWindowLoaded;
    }

    private async void OnMainWindowLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        //初始化机构信息
        vm.CompanyInit(b => {
            //登录窗口
            vm.OpenLoginWindow();
            // 订阅命令
            vm.AppExitCommand.Subscribe(_ => AppExit());
        });
        
    }
    /// <summary>
    /// 退出
    /// </summary>
    public void AppExit()
    {
        _isClosing = true;
        Dispatcher.UIThread.Post(() => this.Close());
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        IsMenuVisible = !IsMenuVisible;
    }

    private void MakeFullScreenPressed(object? sender, PointerPressedEventArgs e)
    {
        FullScreenState();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        string? tag = btn.Tag?.ToString();
        if (tag == "ExitFullScreen")
        {
            FullScreenState();
        }
    }

    private void FullScreenState()
    {
        if (DataContext is not MainWindowViewModel vm) return;
        WindowState = WindowState == WindowState.FullScreen ? WindowState.Maximized : WindowState.FullScreen;
        IsTitleBarVisible = WindowState != WindowState.FullScreen;
        vm.IsFullScreen = WindowState == WindowState.FullScreen ? true : false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void RadioButton_Checked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        if (sender is not RadioButton rb) return;
        if (rb.DataContext is not SukiColorTheme colorTheme) return;
        vm.ChangeTheme(colorTheme);

    }
    /// <summary>
    /// 注销身份重新登录
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MakeReLoginPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        vm.ExitComfirm("确定要注销当前登录的身份吗？", (b) => {
            if (b)
            {
                //清除身份信息
                vm.Title = DateTime.Now.Hour >= 18 ? "晚上好！请登录" : DateTime.Now.Hour > 12 ? "下午好！请登录" : DateTime.Now.Hour >= 6 ? "上午好！请登录" : "夜深了!该休息了";
                vm.LoginUser = null;
                vm.OpenLoginWindow();
            }
        });
    }
    /// <summary>
    /// 退出应用
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MakeExitePressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        _showClosingConfirm = false;
        AppExiteConfirm(b => {
            if (b)
            {
                //清除身份信息
                vm.Title = DateTime.Now.Hour >= 18 ? "晚上好！请登录" : DateTime.Now.Hour > 12 ? "下午好！请登录" : DateTime.Now.Hour >= 6 ? "上午好！请登录" : "夜深了!该休息了";
                vm.LoginUser = null;
                this.Close();
            }
            else
            {
                _showClosingConfirm = true;
            }
        });
    }

    private void AppExiteConfirm(Action<bool>? ac)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        vm.ExitComfirm("确定要退出当前应用吗？", (b) =>
        {
            ac(b);
        });
    }
    /// <summary>
    /// 用户信息查看
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    public void UsrePointerPressed(object sender, PointerPressedEventArgs args)
    {
        var ctl = sender as Control;
        if (ctl != null)
        {
            FlyoutBase.ShowAttachedFlyout(ctl);
        }
    }
    /// <summary>
    /// 关闭事件
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        // 阻止默认关闭行为
        if (!_isClosing)
        {
            _isClosing = true;
            e.Cancel = true;
            if (_showClosingConfirm)
            {
                AppExiteConfirm(b => {
                    if (b)
                    {
                        _showClosingConfirm = false;
                        Dispatcher.UIThread.Post(() => this.Close());
                    }
                    else
                    {
                        _isClosing = false;
                    }  
                });
            }
            else
            {
                Dispatcher.UIThread.Post(()=> this.Close());
            }
        }
    }
}
