using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.Models.Role;
using bbnApp.deskTop.OrganizationStructure.Employee;
using bbnApp.DTOs.CodeDto;
using System;

namespace bbnApp.deskTop.OrganizationStructure.Role;

public partial class RoleView : UserControl
{
    RoleViewModel vm;

    public RoleView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not RoleViewModel vm) return;
        this.vm = vm;
        _=this.vm.RoleDicInit();
    }
    
    private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

        if (sender is not ListBox item) return;
        if (item.SelectedItem is not RoleItemDto ) return;
        if (vm != null)
        {
            vm.RoleSelected=item.SelectedItem as RoleItemDto;
        }
    }
    /// <summary>
    /// 应用选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox btn) return;
        if (btn.DataContext is not RoleAppsModel item) return;
        if (btn != null)
        {
            vm.AppChekced(item);
        }
    }
    /// <summary>
    /// 权限全选
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch btn) return;
        if (btn.DataContext is not RolePermissionItemModel item) return;
        if (btn != null)
        {
            vm.PermissionChange(item);
        }
    }
    /// <summary>
    /// 权限勾选点击
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void CheckBox_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox btn) return;
        if (btn.DataContext is not PermissionCodeItemModel item) return;
        if (btn != null)
        {
            vm.CodeChekced(item);
        }
    }
    /// <summary>
    /// 右键-角色状态变更
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem item) return;
        if (item.DataContext is not RoleItemDto node) return;
        vm.RoleState(item.Tag.ToString(), node);
    }
}