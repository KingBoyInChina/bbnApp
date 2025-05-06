using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Controls;
using bbnApp.DTOs.CodeDto;
using System.Collections;

namespace bbnApp.deskTop.PlatformManagement.AreaCode;

public partial class AreaCodePage : UserControl
{
    public AreaCodePage()
    {
        InitializeComponent();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        AreaSplite.PaneClosed -= (s, e) => { };
        if (DataContext is not AreaCodePageViewModel vm) return;
        // 订阅 PageChanged 事件
        AreaPagination.PageChanged += (Pagination p, int currentPage, int itemsPerPage, IEnumerable pagedData) =>
        {
            vm?.OnPageChanged(p, currentPage, itemsPerPage, pagedData);
        };


        vm.InitValue = new AreaTreeNodeDto
        {
            AreaId = UserContext.CurrentUser.AreaCode,
            AreaFullName = UserContext.CurrentUser.AreaName,
            AreaName = UserContext.CurrentUser.AreaName
        };
        AreaSplite.PaneOpened += (s, e) =>
        {
            
        };
        AreaSplite.PaneClosing += async (s, e) =>
        {
            e.Cancel = vm.IsEdit;
        };
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