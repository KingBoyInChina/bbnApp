using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.PlatformManagement.DeviceCode;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.PlatformManagement.DeviceCommand;

public partial class DeviceCommandView : UserControl
{
    DeviceCommandViewModel vm;
    public DeviceCommandView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not DeviceCommandViewModel vm) return;
        this.vm = vm;
        vm.MaterialDicInit(this);

        DeviceCommandSplit.PaneClosing += async (s, e) =>
        {
            e.Cancel = vm.CommandPanelOpen;
        };

    }
    /// <summary>
    /// 设备tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not DeviceCodeTreeNodeDto node) return;
        if (vm != null)
        {
            _=vm.TreeSelected(node);
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
        if (item.DataContext is not DeviceCommandDto node) return;
        //vm.DeviceState(item.Tag.ToString(), node);
    }
}