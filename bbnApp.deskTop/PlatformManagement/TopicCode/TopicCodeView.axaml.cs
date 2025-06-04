using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.deskTop.PlatformManagement.DeviceCode;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.PlatformManagement.TopicCode;

public partial class TopicCodeView : UserControl
{
    TopicCodeViewModel? vm;
    public TopicCodeView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not TopicCodeViewModel vm) return;
        this.vm = vm;
        vm.TopicCodesDicInit(this);

    }
    /// <summary>
    /// tree 节点选中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TreeView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not TreeView tree) return;
        if (tree.SelectedItem is not TopicCodesTreeNodeDto node) return;
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
        if (item.DataContext is not TopicCodesTreeNodeDto node) return;
        vm.TopicState(item.Tag.ToString(), node);
    }
}