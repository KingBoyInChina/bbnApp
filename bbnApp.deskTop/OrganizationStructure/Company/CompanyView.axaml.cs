using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using bbnApp.deskTop.PlatformManagement.AreaCode;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.OrganizationStructure.Company;

public partial class CompanyView : UserControl
{
    CompanyViewModel vm;
    public CompanyView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not CompanyViewModel vm) return;
        this.vm = vm;
        vm.CompanyDicInit(this);
    }
    /// <summary>
    /// tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not CompanyTreeItemDto node) return;
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
        if (item.DataContext is not CompanyTreeItemDto node) return;
        vm.CompanyState(item.Tag.ToString(), node);
    }
    /// <summary>
    /// 行政地区改变
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    protected void AreaNodeSelector_SelectionChanged(object? obj, AreaTreeNodeDto selected)
    {
        if (DataContext is not AreaCodePageViewModel vm) return;
        vm.AreaCodeSelected = selected;
    }
}