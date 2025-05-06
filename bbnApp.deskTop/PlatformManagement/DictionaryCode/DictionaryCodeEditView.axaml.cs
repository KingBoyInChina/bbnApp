using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace bbnApp.deskTop.PlatformManagement.DictionaryCode;

public partial class DictionaryCodeEditView : UserControl
{
    public DictionaryCodeEditView()
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
        if (DataContext is not DictionaryCodeEditViewModel vm) return;
        vm.ViewModelInit();
    }
}