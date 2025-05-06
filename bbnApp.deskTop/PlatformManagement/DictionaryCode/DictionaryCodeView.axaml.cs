using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.Controls;
using bbnApp.deskTop.PlatformManagement.AppSetting;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.PlatformManagement.DictionaryCode;

public partial class DictionaryCodeView : UserControl
{
    DictionaryCodeViewModel vm;

    public DictionaryCodeView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        DictionaryCodeSplite.PaneClosed -= (s, e) => { };
        if (DataContext is not DictionaryCodeViewModel vm) return;
        this.vm = vm;
        DictionaryCodeSplite.PaneOpened += (s, e) =>
        {

        };
        DictionaryCodeSplite.PaneClosing += async (s, e) =>
        {
            e.Cancel = vm.IsEdit;

        };
        vm.TreeInit();
    }
    /// <summary>
    /// tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeView_SelectionChanged(object sender,SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not DicTreeItemDto node) return;
        if (vm != null)
        {
            _=vm.TreeSelecte(node);
        }
    }
    /// <summary>
    /// MenuItem
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_Click(object sender,RoutedEventArgs e)
    {
        if (sender is not MenuItem item) return;
        if (item.DataContext is not DicTreeItemDto node) return;
        vm.NodeState(item.Tag.ToString(),node);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender,RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.DataContext is not DataDictionaryItemDto item) return;
        if (btn != null) {
            string tag = btn.Tag.ToString();
            if (tag == "Edit")
            {
                vm.ItemWindow(item);
            }
            else
            {
                _=vm.ItemState(tag, item);
            }
        }
    }
    private void ToggleSwitch_Click(object sender,RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch btn) return;
        if (btn.DataContext is not DataDictionaryItemDto item) return;
        if (btn != null)
        {
            string tag = btn.Tag.ToString();
            _ = vm.ItemState(tag, item);
        }
    }
}