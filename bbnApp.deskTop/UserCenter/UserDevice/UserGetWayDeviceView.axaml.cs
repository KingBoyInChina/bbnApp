using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.UserCenter.UserDevice;

namespace bbnApp.deskTop.UserCenter.UserDevice;

public partial class UserGetWayDeviceView : UserControl
{
    public UserGetWayDeviceView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// ≥ı ºªØ
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not UserGetWayDeviceViewModel vm) return;
        vm.ViewModelInit();
    }
}