using CommunityToolkit.Mvvm.ComponentModel;
using Exceptionless;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace bbnApp.deskTop.ViewModels;

public class ViewModelBase : ObservableObject
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
