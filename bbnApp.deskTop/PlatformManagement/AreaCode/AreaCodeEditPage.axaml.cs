using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace bbnApp.deskTop.PlatformManagement.AreaCode;

public partial class AreaCodeEditPage : UserControl
{

    public AreaCodeEditPage()
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
        if (DataContext is not AreaCodeEditPageViewModel vm) return;
        vm.ViewModelInit();
    }
}