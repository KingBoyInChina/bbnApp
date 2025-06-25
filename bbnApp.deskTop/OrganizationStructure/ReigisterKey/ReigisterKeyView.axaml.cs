using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.DTOs.CodeDto;
using System.ComponentModel;

namespace bbnApp.deskTop.OrganizationStructure.ReigisterKey;

public partial class ReigisterKeyView : UserControl
{
    ReigisterKeyViewModel vm;

    public ReigisterKeyView()
    {
        InitializeComponent();
    }
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not ReigisterKeyViewModel vm) return;
        this.vm = vm;
        RegisterKeySplite.PaneOpened += (s, e) =>
        {

        };
        RegisterKeySplite.PaneClosing += async (s, e) =>
        {
            e.Cancel = vm.IsEdit;
        };
        vm.CompanRegisterKeysLoad();
    }
    /// <summary>
    /// Ëø¶¨×´Ì¬±ä¸ü
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not ToggleSwitch btn) return;
        if (btn.DataContext is not AuthorRegisterKeyItemDto node) return;
        if (vm != null)
        {
            vm.ItemSate("IsLock",node);
        }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button btn) return;
        if (btn.DataContext is not AuthorRegisterKeyItemDto node) return;
        if (vm != null)
        {
            string Tag = btn.Tag.ToString();
            vm.ItemSate(Tag, node);
        }
    }
}