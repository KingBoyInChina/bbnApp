using Avalonia.Data.Converters;
using bbnApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace bbnApp.deskTop.Converters
{
    public class IdToComboboxItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 从 Id 找到对应的 ComboboxItem
            if (value is string id && parameter is IEnumerable<ComboboxItem> items)
            {
                return items.FirstOrDefault(x => x.Id == id);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // 从 ComboboxItem 提取 Id
            if (value is ComboboxItem item)
            {
                return item.Id;
            }
            return null;
        }
    }
}
