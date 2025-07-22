using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.OperationsCenter.InfluxDb;
using WebViewControl;

namespace bbnApp.deskTop.OperationsCenter.RabbitMQ;

public partial class RabbitMQView : UserControl
{
    RabbitMQViewModel vm;
    public RabbitMQView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var webview = this.FindControl<WebView>("influxdbWebview");

        if (DataContext is not RabbitMQViewModel vm) return;
        this.vm = vm;
        vm.Init(webview, this);
    }
}