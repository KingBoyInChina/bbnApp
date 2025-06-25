using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.OrganizationStructure.Employee;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.OrganizationStructure.Operator;

public partial class OperatorView : UserControl
{
    OperatorViewModel vm;

    public OperatorView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not OperatorViewModel vm) return;
        this.vm = vm;
        vm.OperatorDicInit(this);
    }
    /// <summary>
    /// 部门tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void DepartMentTreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not DepartMentTreeItemDto node) return;
        if (vm != null)
        {
            vm.DepartMentTreeSelecte(node);
        }
    }
    /// <summary>
    /// 人员List 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not ListBox tree) return;
        if (tree.SelectedItem is not OperatorItemDto node) return;
        if (vm != null)
        {
            vm.OperatorSelectedAction(node);
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
        if (item.DataContext is not OperatorItemDto node) return;
        vm.OperatorState(item.Tag.ToString(), node);
    }
}