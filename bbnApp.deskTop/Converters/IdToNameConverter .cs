using Avalonia.Data.Converters;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
namespace bbnApp.deskTop.Converters;



public static class DicIdToNameConverters
{
    public static IValueConverter Instance { get; } = new DicIdToNameConverter();
}

//字典ID获取Name
public class DicIdToNameConverter : IValueConverter
{

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string id && !string.IsNullOrEmpty(value.ToString()))
        {
            var model = DicContext.DicList.FirstOrDefault(x => x.Id == value.ToString());
            return model==null?"":model.Name;
        }
        return "未知"; // 默认值
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException(); // 单向绑定不需要实现
    }
}
//自定义数据集，ID获取Name
public class IdToNameConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            // 1. 获取输入的 Id（支持 string/int/Guid 等类型）
            var id = value?.ToString();

            // 2. 获取数据源（通过 Binding 或 ConverterParameter 传入）
            var items = parameter as IEnumerable ??
                       throw new ArgumentException("ConverterParameter 必须提供数据源");

            // 3. 动态查找每个项的 Id 和 Name 属性（支持灵活属性名）
            foreach (var item in items)
            {
                var idProp = item.GetType().GetProperty("Id") ??
                            item.GetType().GetProperty("ID"); // 兼容命名
                var nameProp = item.GetType().GetProperty("Name");

                if (idProp != null && nameProp != null)
                {
                    var itemId = idProp.GetValue(item)?.ToString();
                    if (itemId == id)
                    {
                        return nameProp.GetValue(item)?.ToString() ?? "[无名]";
                    }
                }
            }

            return "[未找到]";
        }
        catch
        {
            return "[转换错误]";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException("IdToNameConverter 仅支持单向绑定");
    }
}