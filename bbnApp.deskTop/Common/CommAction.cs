using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static bbnApp.deskTop.Common.CommModels;
using Grpc.Core;
using bbnApp.Common.Models;
using bbnApp.Core;
using System.Collections.ObjectModel;
using System.IO;

namespace bbnApp.deskTop.Common
{
    public class CommAction
    {
        /// <summary>
        /// 文件选择
        /// </summary>
        /// <param name="UserControl">UserControl</param>
        /// <param name="FileTypeFilter">指定文件类型</param>
        /// <param name="AllowMultiple">是否支持多选</param>
        /// <returns></returns>
        public static async Task<FileModle?> OpenFileAndGetDetails(UserControl UserControl, List<FilePickerFileType> FileTypeFilter, bool AllowMultiple = false)
        {
            // 使用 StorageProvider 打开文件选择对话框

            var options = new FilePickerOpenOptions
            {
                Title = "选择文件",
                AllowMultiple = AllowMultiple,
                FileTypeFilter = FileTypeFilter == null ? new List<FilePickerFileType>
                {
                    new FilePickerFileType("图片文件") { Patterns = new[] { "*.png", "*.jpg", "*.jpeg" } },
                    new FilePickerFileType("文档文件") { Patterns = new[] { "*.txt", "*.docx", "*.pdf" } }
                } : FileTypeFilter
            };

            var mainWindow = TopLevel.GetTopLevel(UserControl) as Window;
            var files = await mainWindow.StorageProvider.OpenFilePickerAsync(options);

            if (files != null && files.Any())
            {
                // 获取文件路径
                var filePath = files.First().Path.LocalPath;
                // 获取文件名
                var fileName = files.First().Name;
                return new FileModle(fileName, filePath);
            }
            return null;
        }
        /// <summary>
        /// 文件夹选择
        /// </summary>
        /// <param name="UserControl"></param>
        /// <returns></returns>
        public static async Task<FileModle?> OpenFolderAndGetDetails(UserControl UserControl, bool AllowMultiple = false)
        {
            // 使用 StorageProvider 打开文件选择对话框

            var options = new FolderPickerOpenOptions
            {
                Title = "选择文件夹",
                AllowMultiple = AllowMultiple,
            };

            var mainWindow = TopLevel.GetTopLevel(UserControl) as Window;
            var folders = await mainWindow.StorageProvider.OpenFolderPickerAsync(options);

            if (folders != null && folders.Any())
            {
                // 获取文件路径
                var filePath = folders.First().Path.LocalPath;
                // 获取文件名
                var fileName = folders.First().Name;
                return new FileModle(fileName, filePath);
            }
            return null;
        }
        /// <summary>
        /// 复制
        /// </summary>
        public static async Task<string> OnCopyButtonClick(UserControl u, string copyinfo)
        {

            // 获取当前窗口的 Clipboard
            var clipboard = TopLevel.GetTopLevel(u)?.Clipboard;

            if (clipboard != null)
            {
                try
                {
                    // 将内容复制到剪贴板
                    await clipboard.SetTextAsync(copyinfo);

                    return string.Empty;
                }
                catch (Exception ex)
                {
                    // 处理异常
                    return $"复制失败：{ex.Message}";
                }
            }
            else
            {
                return $"无法访问剪贴板";
            }
        }

        /// <summary>
        /// 获取位置
        /// </summary>
        /// <param name="w"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static PixelPoint GetPoint(Window w, Window n, string type)
        {
            double x = 0;
            double y = 0;
            switch (type)
            {
                case "top":
                    // 计算顶部中央位置
                    x = (w.Bounds.Width - n.Width) / 2;
                    y = 60;
                    break;
                case "topRight":
                    x = w.Bounds.Width - n.Width;
                    y = 60;
                    break;
                case "topLeft":
                    x = 5;
                    y = 60;
                    break;
                case "bottom":
                    x = (w.Bounds.Width - n.Width) / 2;
                    y = w.Bounds.Height - 10;
                    break;
                case "bottomLeft":
                    x = 5;
                    y = w.Bounds.Width - 10;
                    break;
                case "bottomRight":
                    x = 5;
                    y = w.Bounds.Height - 10;
                    break;
                default:
                    // 计算顶部中央位置
                    x = (w.Bounds.Width - n.Width) / 2;
                    y = (w.Bounds.Height - n.Height) / 2;
                    break;
            }
            return new PixelPoint((int)x, (int)y);
        }
        /// <summary>
        /// 设置选中项
        /// </summary>
        /// <param name="list"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static ComboboxItem SetSelectedItem(ObservableCollection<ComboboxItem> list,string value)
        {
            if (list != null && list.Count > 0)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return list.FirstOrDefault();
                }
                else
                {
                    var item = list.FirstOrDefault(x => x.Id == value);
                    return item;
                }
            }
            return null;
        }
        /// <summary>
        /// 图片添加添加水印
        /// </summary>
        /// <param name="inputImagePath">图片路径</param>
        /// <param name="Angle">旋转角度</param>
        /// <param name="position">位置</param>
        /// <param name="formattedText">水印文本</param>
        /// <returns></returns>
        public static RenderTargetBitmap AddWatermark(string inputImagePath, double Angle, List<Point> positions, FormattedText formattedText)
        {
            // 加载原始图片
            using (var originalImage = new Bitmap(inputImagePath))
            {
                // 创建 RenderTargetBitmap
                var renderTargetBitmap = new RenderTargetBitmap(
                    new PixelSize(originalImage.PixelSize.Width, originalImage.PixelSize.Height),
                    new Vector(96, 96));

                using (var drawingContext = renderTargetBitmap.CreateDrawingContext())
                {
                    // 绘制原始图片
                    drawingContext.DrawImage(originalImage, new Rect(0, 0, originalImage.PixelSize.Width, originalImage.PixelSize.Height));
                    foreach (Point position in positions)
                    {
                        // 应用旋转变换
                        using (drawingContext.PushTransform(RotateTransform(Angle, position)))
                        {
                            // 绘制水印文本
                            drawingContext.DrawText(formattedText, position);
                        }
                    }
                }

                // 保存为 PNG 文件
                //renderTargetBitmap.Save(outputImagePath);
                return renderTargetBitmap;
            }
        }
        /// <summary>
        /// 角度变换
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        private static Matrix RotateTransform(double angle, Point center)
        {
            // 创建旋转矩阵
            var matrix = Matrix.CreateTranslation(-center.X, -center.Y) * // 将中心点移动到原点
                         Matrix.CreateRotation(angle * Math.PI / 180) *   // 旋转
                         Matrix.CreateTranslation(center.X, center.Y);    // 将中心点移回原位
            return matrix;
        }
        /// <summary>
        /// datatable绑定到DataGrid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="dt"></param>
        public static void DataGridBindDataTable(DataGrid grid, DataTable dt)
        {
            try
            {
                while (grid.Columns.Count > 0) { grid.Columns.RemoveAt(grid.Columns.Count - 1); }
                grid.ItemsSource = dt.DefaultView;
                foreach (System.Data.DataColumn x in dt.Columns)
                {
                    if (x.DataType == typeof(bool))
                    {
                        grid.Columns.Add(new DataGridCheckBoxColumn { Header = x.ColumnName, Binding = new Avalonia.Data.Binding($"Row.ItemArray[{x.Ordinal}]") });
                    }
                    else
                    {
                        grid.Columns.Add(new DataGridTextColumn { Header = x.ColumnName, Binding = new Avalonia.Data.Binding($"Row.ItemArray[{x.Ordinal}]") });
                    }
                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 获取请求头
        /// </summary>
        /// <returns></returns>
        public static Metadata GetHeader()
        {
            UserModel User = UserContext.CurrentUser;
            var headers = new Metadata
                        {
                            { "Yhid", User.Yhid},
                            { "OperatorId", User.OperatorId},
                            { "authorization","Bearer "+User.Token}
                        };
            return headers;
        }
        /// <summary>
        /// 获取数据字典
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="TagIsKey">针对行政级别这种特殊字典</param>
        /// <returns></returns>
        public static List<ComboboxItem> GetDicItems(string ID,bool TagIsKey=false)
        {
            if (DicContext.DicItems == null) return new List<ComboboxItem>();
            var item = DicContext.DicItems.FirstOrDefault(x => x.Id == ID);
            if (item != null)
            {
                if (item.SubItems != null)
                {
                    List<ComboboxItem> items = new List<ComboboxItem>();
                    foreach (var x in item.SubItems)
                    {
                        if (TagIsKey)
                        {
                            items.Add(new ComboboxItem(x.Tag, x.Name, x.Id));
                        }
                        else
                        {
                            items.Add(new ComboboxItem(x.Id, x.Name, x.Tag));
                        }
                    }
                    return items;
                }
                return new List<ComboboxItem>();
            }
            return new List<ComboboxItem>();
        }
        /// <summary>
        /// 获取系统配置参数
        /// </summary>
        /// <param name="SettingCode"></param>
        /// <returns>(当前值,默认值)</returns>
        public static (object, object) GetAppSettingValue(string SettingCode)
        {
            if (DicContext.AppSettingList == null) return ("","");
            var item = DicContext.AppSettingList.FirstOrDefault(x=>x.SettingCode== SettingCode&&x.Yhid== UserContext.CurrentUser.Yhid);
            if (item != null)
            {
                if (item.SettingType == "int")
                {
                    return (Convert.ToInt32(item.NowValue), item.SettingDesc);
                }
                else if (item.SettingType == "bool")
                {
                    return (Convert.ToBoolean(item.NowValue), item.SettingDesc);
                }
                else
                {
                    return (item.NowValue, item.SettingDesc);
                }
            }
            return ("", "");
        }
    }
}
