using Avalonia.Controls;
using Avalonia.Interactivity;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.OrganizationStructure.DepartMent;

public partial class DepartMentView : UserControl
{
    DepartMentViewModel vm;

    public DepartMentView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not DepartMentViewModel vm) return;
        this.vm = vm;
        vm.DepartMentDicInit(this);
    }
    /// <summary>
    /// tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not DepartMentTreeItemDto node) return;
        if (vm != null)
        {
            vm.TreeSelecte(node);
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
        if (item.DataContext is not DepartMentTreeItemDto node) return;
        vm.DepartMentState(item.Tag.ToString(), node);
    }
}