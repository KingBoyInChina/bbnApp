using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using bbnApp.deskTop.ViewModels;
using System;
using System.Threading.Tasks;

namespace bbnApp.deskTop.Views;

public partial class LoginWindow : UserControl
{
    public LoginWindow()
    {
        InitializeComponent();
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        PasswordTextBox.Focus();//设置密码框获取焦点
    }
    private void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            if (DataContext is not LoginWindowViewModel vm) return;
            _ = vm.PassWordKeyPress();
        }
    }
    private void TextBox_GotFocus(object sender, GotFocusEventArgs e)
    {
        // 光标进入时禁用中文输入法
        if (sender is TextBox textBox)
        {
            InputMethod.SetIsInputMethodEnabled(textBox, false);
        }
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        // 光标离开时恢复中文输入法
        if (sender is TextBox textBox)
        {
            InputMethod.SetIsInputMethodEnabled(textBox, true);
        }
    }
}