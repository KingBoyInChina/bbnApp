using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Services;
using Material.Icons;
using SukiUI.Dialogs;
using SukiUI.Toasts;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System.Collections.Generic;

namespace bbnApp.deskTop.AssistiveTools.WaterMark
{
    public partial class WaterMarkPageViewModel: BbnPageBase
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly ISukiToastManager sukiToastManager;
        /// <summary>
        /// 
        /// </summary>
        private readonly ISukiDialogManager dialogManager;
        /// <summary>
        /// 
        /// </summary>
        private readonly PageNavigationService nav;
        /// <summary>
        /// <summary>
        /// 图片添加水印
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public WaterMarkPageViewModel(ISukiToastManager ToastManager, ISukiDialogManager DialogManager, PageNavigationService nav) : base("AssistiveToolsFiles", "图片添加水印", MaterialIconKind.Decrement, "bbn-watermark", 2)
        {
            this.sukiToastManager = ToastManager;
            this.dialogManager = DialogManager;
            this.nav = nav;

        }
        
        /// <summary>
        /// 水印文字
        /// </summary>
        [ObservableProperty] private string _waterText = DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒");
        /// <summary>
        /// 字体大小
        /// </summary>
        [ObservableProperty] private int _waterSize = 14;
        /// <summary>
        /// 水印颜色
        /// </summary>
        [ObservableProperty] private Avalonia.Media.Color _waterColor = new Avalonia.Media.Color(1, 255, 255, 255);
        /// <summary>
        /// 透明度
        /// </summary>
        [ObservableProperty] private byte _waterOpacity = 255;
        /// <summary>
        /// 旋转角度
        /// </summary>
        [ObservableProperty] private double _waterAngle = -30;
        /// <summary>
        /// 选中的文件
        /// </summary>
        [ObservableProperty] private Common.CommModels.FileModle _selectedFile;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private SolidColorBrush _waterBrush;
        /// <summary>
        /// R
        /// </summary>
        [ObservableProperty] private byte _r=255;
        /// <summary>
        /// G
        /// </summary>
        [ObservableProperty] private byte _g = 255;
        /// <summary>
        /// B
        /// </summary>
        [ObservableProperty] private byte _b = 255;
        /// <summary>
        /// 选中的图片
        /// </summary>
        [ObservableProperty] private Bitmap _selectImage;
        /// <summary>
        /// 重写 OnPropertyChanged 方法
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPropertyChanged(System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == nameof(WaterText)|| e.PropertyName == nameof(WaterSize)
                || e.PropertyName == nameof(WaterAngle) || e.PropertyName == nameof(WaterBrush) || e.PropertyName == nameof(SelectedFile) )
            {
                UpdateWaterColor();
            }
            else if(e.PropertyName == nameof(R)|| e.PropertyName == nameof(G) || e.PropertyName == nameof(B)|| e.PropertyName == nameof(WaterOpacity))
            {

                // 计算透明度（将 0-100 映射到 0-255）
                byte alpha = (byte)(WaterOpacity / 100.0 * 255);
                WaterColor = Avalonia.Media.Color.FromArgb(alpha, R, G, B);

                WaterBrush = new SolidColorBrush(WaterColor);
            }
        }
        /// <summary>
        /// 消息提示
        /// </summary>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        public void ToastInfo(string title,string msg, Avalonia.Controls.Notifications.NotificationType infotype)
        {
            sukiToastManager.CreateToast()
                .WithTitle(title)
                .WithContent(msg)
                .OfType(infotype)
                .Dismiss().ByClicking()
                .Dismiss().After(TimeSpan.FromSeconds(3))
                .Queue();
        }

        /// <summary>
        /// 更新 WaterColor 的值
        /// </summary>
        
        public void UpdateWaterColor()
        {

            // 更新 WaterColor
            if (SelectedFile != null && SelectImage != null)
            {
                //添加水印
                var typeface = new Typeface("Arial", FontStyle.Normal, FontWeight.Normal);

                var formattedText = new FormattedText(
                                        WaterText,
                                        System.Globalization.CultureInfo.CurrentCulture,
                                        FlowDirection.LeftToRight,
                                        typeface,
                                        WaterSize, // 字体大小
                                        WaterBrush); // 水印颜色

                List<Avalonia.Point> points = new List<Avalonia.Point>();

                var pixeSzie = SelectImage.PixelSize;
                int _imageWidth = pixeSzie.Width;
                int _imageHeight = pixeSzie.Height;

                int _textwidth = WaterText.Length * WaterSize;
                //计算需要多少列和多少行
                int _columns = (_imageWidth / _textwidth) + 1;
                int _rows = (_imageWidth / 240) + 1;

                for (int i = 0; i <= _columns; i++)
                {
                    for (int j = 0; j <= _rows; j++)
                    {
                        //加一个随机数
                        int _randxvalue = Share.CommMethod.GetIntRandom(20);
                        int _randyvalue = Share.CommMethod.GetIntRandom(20);

                        Avalonia.Point position = new Avalonia.Point((_textwidth * i) + _randxvalue, (200 * j + _randyvalue)); // 水印位置
                        points.Add(position);
                    }

                }
                SelectImage = CommAction.AddWatermark(SelectedFile.FilePath, WaterAngle, points, formattedText);
            }

        }
    }
}
