using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using bbnApp.deskTop.Models.User;
using bbnApp.deskTop.PlatformManagement.MaterialsCode;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.deskTop.UserCenter.UserInformation;

public partial class UserInformationView : UserControl
{
    UserInformationViewModel vm;
    public UserInformationView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not UserInformationViewModel vm)return;
        this.vm = vm;
        vm.DataInit();
    }
    /// <summary>
    /// 行政地区改变
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="e"></param>
    protected void AreaNodeFilterSelector_SelectionChanged(object? obj, AreaTreeNodeDto selected)
    {
        if (DataContext is not UserInformationViewModel vm) return;
        vm.AreaFilterSelected = selected;
    }
    protected void AreaNodeSelector_SelectionChanged(object? obj, AreaTreeNodeDto selected)
    {
        if (DataContext is not UserInformationViewModel vm) return;
        vm.AreaSelected = selected;
    }
    /// <summary>
    /// MenuItem
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not MenuItem item) return;
        if (item.DataContext is not UserTreeItemDto node) return;
        _ = vm.UserState(item.Tag.ToString(), node.Id,"","", (bool)node.IsLock);
    }

    /// <summary>
    /// 状态变更
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContactToggleSwitch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch btn) return;
        if (btn.DataContext is not UserContactDto item) return;
        _ = vm.UserState(btn.Tag.ToString(), item.UserId,item.ContactId,"",item.IsLock==0?false:true);
    }
    private void AabToggleSwitch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch btn) return;
        if (btn.DataContext is not UserAabItem item) return;
        _ = vm.UserState(btn.Tag.ToString(), item.UserId, "", item.AabId, item.IsLock == 0 ? false : true);
    }
    /// <summary>
    /// 删除操作
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ContactButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.DataContext is not UserContactDto item) return;
        _ = vm.UserState(btn.Tag.ToString(), item.UserId,item.ContactId,"", item.IsLock == 0 ? false : true);
    }
    private void AabButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.DataContext is not UserAabItem item) return;
        _ = vm.UserState(btn.Tag.ToString(), item.UserId, "", item.AabId, item.IsLock == 0 ? false : true);
    }
}