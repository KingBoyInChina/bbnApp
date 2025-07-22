using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.DTOs.CodeDto;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using WebViewControl;

namespace bbnApp.deskTop.OperationsCenter.InfluxDb;

public partial class InfluxDbView : UserControl
{
    InfluxDbViewModel vm;
    public InfluxDbView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var webview = this.FindControl<WebView>("influxdbWebview");

        if (DataContext is not InfluxDbViewModel vm) return;
        this.vm = vm;
        vm.Init(webview, this);
    }
}