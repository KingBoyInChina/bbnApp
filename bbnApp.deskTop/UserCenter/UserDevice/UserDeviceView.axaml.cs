using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.UserCenter.UserDevice;

public partial class UserDeviceView : UserControl
{
    UserDeviceViewModel vm;
    public UserDeviceView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not UserDeviceViewModel vm) return;
        DeviceSplite.PaneOpened += (s, e) =>
        {

        };
        DeviceSplite.PaneClosing += async (s, e) =>
        {
            e.Cancel = vm.IsEdit;
        };
        this.vm = vm;
        _=vm.DataInit();
    }
    /// <summary>
    /// 行政地区改变
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    protected void AreaNodeFilterSelector_SelectionChanged(object? obj, AreaTreeNodeDto selected)
    {
        if (DataContext is not UserDeviceViewModel vm) return;
        vm.AreaFilterSelected = selected;
    }
    /// <summary>
    /// MenuItem
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void GetWayMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem item) return;
        if (item.DataContext is not UserGetWayDto node) return;
        _=vm.GetWayState(item.Tag.ToString(), node);
    }
    /// <summary>
    /// 设备按钮点击操作
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void GetWayDeviceButton_Click(object sender,RoutedEventArgs e)
    {
        if (sender is not Button item) return;
        if (item.DataContext is not UserGetWayDeviceDto node) return;
        _ = vm.GetWayDeviceState(item.Tag.ToString(), node);
    }

    public void GetWayDeviceCheckbox_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not CheckBox item) return;
        if (item.DataContext is not UserGetWayDeviceDto node) return;
        _ = vm.GetWayDeviceState(item.Tag.ToString(), node);
    }
}