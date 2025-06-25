using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using bbnApp.deskTop.Controls;
using bbnApp.DTOs.CodeDto;
using System.Collections;
using System.Threading.Tasks;

namespace bbnApp.deskTop.PlatformManagement.AppSetting;

public partial class AppSettingView : UserControl
{
    AppSettingViewModel vm;
    public AppSettingView()
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
        AppSettingSplite.PaneClosed -= (s, e) => { };
        if (DataContext is not AppSettingViewModel vm) return;
        this.vm = vm;
        // ¶©ÔÄ PageChanged ÊÂ¼þ
        AppSettingPagination.PageChanged += (Pagination p, int currentPage, int itemsPerPage, IEnumerable pagedData) =>
        {
            vm?.OnPageChanged(p, currentPage, itemsPerPage, pagedData);
        };
        AppSettingSplite.PaneOpened += (s, e) =>
        {

        };
        AppSettingSplite.PaneClosing += async (s, e) =>
        {
            e.Cancel = vm.IsEdit;

        };
    }

    private void ToggleSwitch_Click(object? obj,RoutedEventArgs e)
    {
        if (obj is not ToggleSwitch ts) return;
        if (ts.DataContext is not AppSettingDto item) return;
        if (ts.Tag != null)
        {
            _ = this.vm.SettingState(ts.Tag.ToString(), item);
        }
    }
}