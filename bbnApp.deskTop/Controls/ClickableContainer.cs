using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System;
namespace bbnApp.deskTop.Controls;

public class ClickableContainer : StackPanel
{
    /// <summary>
    /// 定义 Click 事件
    /// </summary>
    public event EventHandler<RoutedEventArgs> Click;

    /// <summary>
    /// 构造函数
    /// </summary>
    public ClickableContainer()
    {
        // 监听鼠标按下事件
        this.PointerPressed += OnPointerPressed;
    }

    /// <summary>
    /// 鼠标按下事件处理
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnPointerPressed(object sender, PointerPressedEventArgs e)
    {
        // 触发 Click 事件
        Click?.Invoke(this, new RoutedEventArgs());
    }
}
