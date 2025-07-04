using Avalonia.Controls;
using Avalonia.Data.Converters;
using Material.Icons;
using System;
using System.Globalization;

namespace bbnApp.deskTop.Converters;

public static class BoolToTextConverters
{
    public static readonly BoolToTextConverter Lock = new("锁定", "正常");
    public static readonly BoolToTextConverter LockLg = new("解锁", "锁定");
    public static readonly BoolToTextConverter State = new("正常", "异常");
}

public class BoolToTextConverter(string trueText, string falseText) : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool b) return null;
        return b ? trueText : falseText;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not string text) return null;

        // 根据文本返回对应的布尔值
        return text == trueText;
    }
}


