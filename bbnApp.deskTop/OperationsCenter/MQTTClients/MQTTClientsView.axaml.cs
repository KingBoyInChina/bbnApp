using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.OrganizationStructure.Company;

namespace bbnApp.deskTop.OperationsCenter.MQTTClients;

public partial class MQTTClientsView : UserControl
{
    MQTTClientsViewModel vm;
    public MQTTClientsView()
    {
        InitializeComponent();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not MQTTClientsViewModel vm) return;
        this.vm = vm;
        _=vm.GetRegisterKeys(this);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        base.OnUnloaded(e);
        if (vm != null)
        {
            //移除动态添加的订阅主题
            vm.OnDeactivated();
        }
    }
}