using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.PlatformManagement.MaterialsCode;

public partial class MaterialsCodeView : UserControl
{
    MaterialsCodeViewModel vm;

    public MaterialsCodeView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not MaterialsCodeViewModel vm) return;
        this.vm = vm;
        vm.MaterialDicInit(this);
    }
    /// <summary>
    /// tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not MaterialTreeItemDto node) return;
        if (vm != null)
        {
            _ = vm.TreeSelecte(node);
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
        if (item.DataContext is not MaterialTreeItemDto node) return;
        _=vm.MaterialState(item.Tag.ToString(), node);
    }
}