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
    /// �ر�״̬
    /// </summary>
    private bool _isClosing = false;
    /// <summary>
    /// ����رհ�ť�ر���Ҫ������ʾ
    /// </summary>
    private bool _showClosingConfirm = true;
    public MainWindow()
    {
        InitializeComponent();
        if (RuntimeFeature.IsDynamicCodeCompiled == false)
        {
            Title += " (native)";
        }
        // ���� Loaded �¼�
        this.Loaded +=OnMainWindowLoaded;
    }

    private async void OnMainWindowLoaded(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        //��ʼ��������Ϣ
        vm.CompanyInit(b => {
            //��¼����
            vm.OpenLoginWindow();
            // ��������
            vm.AppExitCommand.Subscribe(_ => AppExit());
        });
        
    }
    /// <summary>
    /// �˳�
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
    /// ע��������µ�¼
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MakeReLoginPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainWindowViewModel vm) return;
        vm.ExitComfirm("ȷ��Ҫע����ǰ��¼�������", (b) => {
            if (b)
            {
                //��������Ϣ
                vm.Title = DateTime.Now.Hour >= 18 ? "���Ϻã����¼" : DateTime.Now.Hour > 12 ? "����ã����¼" : DateTime.Now.Hour >= 6 ? "����ã����¼" : "ҹ����!����Ϣ��";
                vm.LoginUser = null;
                vm.OpenLoginWindow();
            }
        });
    }
    /// <summary>
    /// �˳�Ӧ��
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
                //��������Ϣ
                vm.Title = DateTime.Now.Hour >= 18 ? "���Ϻã����¼" : DateTime.Now.Hour > 12 ? "����ã����¼" : DateTime.Now.Hour >= 6 ? "����ã����¼" : "ҹ����!����Ϣ��";
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
        vm.ExitComfirm("ȷ��Ҫ�˳���ǰӦ����", (b) =>
        {
            ac(b);
        });
    }
    /// <summary>
    /// �û���Ϣ�鿴
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
    /// �ر��¼�
    /// </summary>
    /// <param name="e"></param>
    protected override void OnClosing(WindowClosingEventArgs e)
    {
        // ��ֹĬ�Ϲر���Ϊ
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
