using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Material.Icons;

namespace bbnApp.deskTop.Converters;

public static class BoolToIconConverters
{
    public static readonly BoolToIconConverter Animation = new(MaterialIconKind.Pause, MaterialIconKind.Play);
    public static readonly BoolToIconConverter WindowLock = new(MaterialIconKind.Unlocked, MaterialIconKind.Lock);
    public static readonly BoolToIconConverter View = new(MaterialIconKind.Show, MaterialIconKind.Close);
    public static readonly BoolToIconConverter ThemeState = new(MaterialIconKind.Lightbulb, MaterialIconKind.ThemeLightDark);
    public static readonly BoolToIconConverter Visibility = new(MaterialIconKind.EyeClosed, MaterialIconKind.Eye);
    public static readonly BoolToIconConverter Simple = new(MaterialIconKind.Close, MaterialIconKind.Ticket);
    public static readonly BoolToIconConverter FullScreen = new(MaterialIconKind.Fullscreen, MaterialIconKind.FullscreenExit);
    public static readonly BoolToIconConverter FileLock = new(MaterialIconKind.File, MaterialIconKind.FileLock);
    public static readonly BoolToIconConverter FileLeaf = new(MaterialIconKind.File, MaterialIconKind.Folder);
    public static readonly BoolToIconConverter Link = new(MaterialIconKind.LanConnect, MaterialIconKind.LanDisconnect);
}

public class BoolToIconConverter(MaterialIconKind trueIcon, MaterialIconKind falseIcon) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b) return null;
        return b ? trueIcon : falseIcon;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public static class IntToIconConverters
{
    public static readonly IntToIconConverter Animation = new(MaterialIconKind.Pause, MaterialIconKind.Play);
    public static readonly IntToIconConverter WindowLock = new(MaterialIconKind.Unlocked, MaterialIconKind.Lock);
    public static readonly IntToIconConverter View = new(MaterialIconKind.Show, MaterialIconKind.Close);
    public static readonly IntToIconConverter ThemeState = new(MaterialIconKind.Lightbulb, MaterialIconKind.ThemeLightDark);
    public static readonly IntToIconConverter Visibility = new(MaterialIconKind.EyeClosed, MaterialIconKind.Eye);
    public static readonly IntToIconConverter Simple = new(MaterialIconKind.Close, MaterialIconKind.Ticket);
    public static readonly IntToIconConverter FullScreen = new(MaterialIconKind.Fullscreen, MaterialIconKind.FullscreenExit);
    public static readonly IntToIconConverter FileLock = new(MaterialIconKind.FileLock,MaterialIconKind.File);
    public static readonly IntToIconConverter FileLeaf = new(MaterialIconKind.File, MaterialIconKind.Folder);
}

public class IntToIconConverter(MaterialIconKind trueIcon, MaterialIconKind falseIcon) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool b = false;
        if (value is int intValue)
        {
            b= intValue == 1 ? true : false;
        }
        else if (value is byte byteValue)
        {
            b= byteValue == 1 ? true : false;
        }
        return b ? trueIcon : falseIcon;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}