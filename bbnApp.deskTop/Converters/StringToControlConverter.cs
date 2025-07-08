using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace bbnApp.deskTop.Converters;


public static class DateFormatConverters
{
    public static IValueConverter SmallDate { get; } = new DateToTextConverter();
    public static IValueConverter LongDate { get; } = new DateToTextConverter();
}

public class DateToTextConverter: IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string result = string.Empty;
        if (value is DateTime v)
        {
            result = v.ToString("yyyy-MM-dd");
        }
        else 
        {
            try
            {
                var vd=System.Convert.ToDateTime(value);
                result = vd.ToString("yyyy-MM-dd");
            }
            catch(Exception ex)
            {

            }
        }
        return result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime text) {
            return text.ToString("yyyy-MM-dd");
        }
        else
        {
            try
            {
                var vd = System.Convert.ToDateTime(value);
                return vd.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}

public class DateToLongTextConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        string result = string.Empty;
        if (value is DateTime v)
        {
            result = v.ToString("yyyy-MM-dd HH:mm:ss");
        }
        else
        {
            try
            {
                var vd = System.Convert.ToDateTime(value);
                result = vd.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {

            }
        }
        return result;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime text)
        {
            return text.ToString("yyyy-MM-dd HH:mm:ss");
        }
        else
        {
            try
            {
                var vd = System.Convert.ToDateTime(value);
                return vd.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}