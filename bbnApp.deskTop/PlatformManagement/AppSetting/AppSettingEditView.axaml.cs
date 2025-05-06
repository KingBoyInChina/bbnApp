using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.PlatformManagement.AreaCode;

namespace bbnApp.deskTop.PlatformManagement.AppSetting;

public partial class AppSettingEditView : UserControl
{
    public AppSettingEditView()
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
        if (DataContext is not AppSettingEditViewModel vm) return;
        vm.ViewModelInit();
    }
}