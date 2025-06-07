using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.OrganizationStructure.Employee;

public partial class EmployeeView : UserControl
{
    EmployeeViewModel vm;

    public EmployeeView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not EmployeeViewModel vm) return;
        this.vm = vm;
        vm.EmployeeDicInit(this);
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
    /// 人员tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void EmployeeTreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not EmployeeTreeItemDto node) return;
        if (vm != null)
        {
            vm.NodeInfoLoad(node);
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
        if (item.DataContext is not EmployeeTreeItemDto node) return;
        vm.DepartMentState(item.Tag.ToString(), node);
    }
}