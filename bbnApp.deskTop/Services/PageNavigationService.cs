using System;
using bbnApp.deskTop.Features;


namespace bbnApp.deskTop.Services;

public class PageNavigationService
{
    public Action<Type>? NavigationRequested { get; set; }

    public void RequestNavigation<T>() where T : BbnPageBase
    {
        NavigationRequested?.Invoke(typeof(T));
    }
}
