using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace bbnApp.deskTop.Converters
{
    public class IntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int intValue)
            {
                return intValue == 1 ? true : false;
            }
            else if (value is byte byteValue)
            {
                return byteValue == 1 ? true : false;
            }
            return false; // 默认值
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? 1 : 0;
            }
            return 0; // 默认值
        }
    }
}
