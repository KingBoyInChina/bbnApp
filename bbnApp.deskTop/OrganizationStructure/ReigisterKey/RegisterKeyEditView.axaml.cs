using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace bbnApp.deskTop.OrganizationStructure.ReigisterKey;

public partial class RegisterKeyEditView : UserControl
{
    public RegisterKeyEditView()
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
        if (DataContext is not RegisterKeyEditViewModel vm) return;
        vm.ViewModelInit();
    }
}