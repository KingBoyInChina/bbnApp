using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using bbnApp.DTOs.CodeDto;
using BbnApp.Protos;
using System;

namespace bbnApp.deskTop.PlatformManagement.AreaCode;

public partial class AreaCodeEditPage : UserControl
{

    public AreaCodeEditPage()
    {
        InitializeComponent();
    }
    /// <summary>
    /// ≥ı ºªØ
    /// </summary>
    /// <param name="e"></param>
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DataContext is not AreaCodeEditPageViewModel vm) return;
        vm.ViewModelInit();
    }
}