using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using bbnApp.deskTop.Services;

namespace bbnApp.deskTop.Features.Splash;

public partial class SplashViewModel(PageNavigationService nav) : BbnPageBase("UserCenter", "事务中心", MaterialIconKind.Hand, "bbn-chart-pie-alt", int.MinValue)
{
    [ObservableProperty] private bool _dashBoardVisited;

    [RelayCommand]
    private void OpenDashboard()
    {
        
    }
}