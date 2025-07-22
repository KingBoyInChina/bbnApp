using CommunityToolkit.Mvvm.ComponentModel;
using Material.Icons;

namespace bbnApp.deskTop.Common;

/// <summary>
/// 
/// </summary>
/// <param name="Tag"></param>
/// <param name="displayName"></param>
/// <param name="icon"></param>
/// <param name="iconfontname"></param>
/// <param name="index"></param>
public abstract partial class BbnPageBase(string Tag, string displayName, MaterialIconKind icon,string iconfont, int index = 0) : ObservableValidator
{
    [ObservableProperty] private string _tag = Tag;
    [ObservableProperty] private string _displayName = displayName;
    [ObservableProperty] private MaterialIconKind _icon = icon;
    [ObservableProperty] private string _iconFont = iconfont;
    [ObservableProperty] private int _index = index;
}
