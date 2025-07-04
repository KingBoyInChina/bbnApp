using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.Converters
{

    public static class IntToTextConverters
    {
        public static readonly IntToTextConverter Lock = new("锁定", "正常");
        public static readonly IntToTextConverter LockLg = new("解锁", "锁定");
        public static readonly IntToTextConverter State = new("正常", "异常");
    }

    public class IntToTextConverter(string trueText, string falseText) : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool b = false;
            if (value is int intValue)
            {
                b = intValue == 1 ? true : false;
            }
            else if (value is byte byteValue)
            {
                b = byteValue == 1 ? true : false;
            }
            return b ? trueText : falseText;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is not string text) return null;

            // 根据文本返回对应的布尔值
            return text == trueText;
        }
    }
}
