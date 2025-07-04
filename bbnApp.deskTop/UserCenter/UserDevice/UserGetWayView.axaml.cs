using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace bbnApp.deskTop.UserCenter.UserDevice;

public partial class UserGetWayView : UserControl
{
    public UserGetWayView()
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
        if (DataContext is not UserGetWayViewModel vm) return;
        vm.ViewModelInit();
    }
}