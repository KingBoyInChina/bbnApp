using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia;
using Material.Icons;
using Avalonia.Controls;
using Avalonia.Media;
using Material.Icons.Avalonia;

namespace bbnApp.deskTop.Controls
{
    public class IconFont : TemplatedControl
    {
        // 定义 Kind 依赖属性
        public static readonly StyledProperty<string> KindProperty =
            AvaloniaProperty.Register<IconFont, string>(nameof(Kind));

        public string Kind
        {
            get => GetValue(KindProperty);
            set => SetValue(KindProperty, value);
        }

        // 定义 MKind 依赖属性
        public static readonly StyledProperty<MaterialIconKind> MKindProperty =
            AvaloniaProperty.Register<IconFont, MaterialIconKind>(nameof(MKind));

        public MaterialIconKind MKind
        {
            get => GetValue(MKindProperty);
            set => SetValue(MKindProperty, value);
        }

        // 定义 IconWidth 和 IconHeight 依赖属性
        public static readonly StyledProperty<double> IconWidthProperty =
            AvaloniaProperty.Register<IconFont, double>(nameof(IconWidth));

        public double IconWidth
        {
            get => GetValue(IconWidthProperty);
            set => SetValue(IconWidthProperty, value);
        }

        public static readonly StyledProperty<double> IconHeightProperty =
            AvaloniaProperty.Register<IconFont, double>(nameof(IconHeight));

        public double IconHeight
        {
            get => GetValue(IconHeightProperty);
            set => SetValue(IconHeightProperty, value);
        }

        public static readonly StyledProperty<double> IconFontSizeProperty =
            AvaloniaProperty.Register<IconFont, double>(nameof(FontSize), defaultValue: 14);

        public double FontSize
        {
            get => GetValue(IconFontSizeProperty);
            set => SetValue(IconFontSizeProperty, value);
        }

        // 构造函数
        public IconFont()
        {
            this.Template = new FuncControlTemplate<IconFont>((control, scope) =>
            {
                if (Kind != null && !string.IsNullOrEmpty(Kind))
                {
                    var textBlock = new TextBlock
                    {
                        FontFamily = (FontFamily)control.FindResource("BbnIconFont"),
                        FontSize = control.FontSize,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                    };

                    // 处理 Kind 属性
                    if (control.Kind != null && control.TryFindResource(control.Kind, out var resource))
                    {
                        textBlock.Text = resource.ToString(); // 使用资源中的值
                    }

                    return textBlock;
                }
                else
                {
                    var _materialIcon = new MaterialIcon
                    {
                        Kind = control.MKind,
                    };

                    return _materialIcon;
                }
            });
        }
    }
}
