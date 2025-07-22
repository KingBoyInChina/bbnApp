using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.OperationsCenter.ExceptionLess;
using bbnApp.deskTop.OperationsCenter.RabbitMQ;
using WebViewControl;

namespace bbnApp.deskTop.OperationsCenter.ExceptionLess;

public partial class ExceptionLessView : UserControl
{
    ExceptionLessViewModel vm;
    public ExceptionLessView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var webview = this.FindControl<WebView>("influxdbWebview");

        if (DataContext is not ExceptionLessViewModel vm) return;
        this.vm = vm;
        vm.Init(webview, this);
    }
}