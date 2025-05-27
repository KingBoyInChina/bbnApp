using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using bbnApp.DTOs.CodeDto;
using System;

namespace bbnApp.deskTop.PlatformManagement.DeviceCode;

public partial class DeviceCodeView : UserControl
{
    DeviceCodeViewModel vm;

    public DeviceCodeView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not DeviceCodeViewModel vm) return;
        this.vm = vm;
        vm.MaterialDicInit(this);

        DeviceCodeSplit.PaneClosing += async (s, e) =>
        {
            e.Cancel = vm.StructPanelOpen;
        };
    }
    /// <summary>
    /// tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not DeviceCodeTreeNodeDto node) return;
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
        if (item.DataContext is not DeviceCodeTreeNodeDto node) return;
        vm.DeviceState(item.Tag.ToString(), node);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender,RoutedEventArgs e)
    {
        if (sender is not Button item) return;
        if (item.DataContext is not DeviceStructItemDto node) return;
        vm.DeviceStuctState(item.Tag.ToString(), node);
    }
}