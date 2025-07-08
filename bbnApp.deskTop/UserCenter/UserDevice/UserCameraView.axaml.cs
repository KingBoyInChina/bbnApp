using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace bbnApp.deskTop.UserCenter.UserDevice;

public partial class UserCameraView : UserControl
{
    public UserCameraView()
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
        if (DataContext is not UserCameraViewModel vm) return;
        vm.ViewModelInit();
    }
}