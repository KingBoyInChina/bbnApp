using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.PlatformManagement.OperationCode;

public partial class OperationCodeView : UserControl
{
    OperationCodeViewModel vm;
    public OperationCodeView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not OperationCodeViewModel vm) return;
        this.vm = vm;
        vm.TreeInitCommand.Execute(null);
    }

    /// <summary>
    /// tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not OperationObjectNodeDto node) return;
        if (vm != null)
        {
            vm.NodeSelected(node);
        }
    }
    /// <summary>
    /// MenuItem
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem item) return;
        if (item.DataContext is not OperationObjectNodeDto node) return;
        _=vm.ObjState(item.Tag.ToString(), node);
    }
    private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch btn) return;
        if (btn.DataContext is not ObjectOperationTypeDto item) return;
        if (btn != null)
        {
            vm.ItemSave(item);
        }
    }
}