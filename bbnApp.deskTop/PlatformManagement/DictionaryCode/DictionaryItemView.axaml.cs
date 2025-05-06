using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace bbnApp.deskTop.PlatformManagement.DictionaryCode;

public partial class DictionaryItemView : UserControl
{
    public DictionaryItemView()
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
        if (DataContext is not DictionaryItemViewModel vm) return;
        vm.ViewModelInit();
    }
}