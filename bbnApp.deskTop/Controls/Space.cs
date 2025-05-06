
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using System;
namespace bbnApp.deskTop.Controls;
public class Space : Panel
{
    // 定义间距属性（可绑定）
    public static readonly StyledProperty<double> SpacingProperty =
        AvaloniaProperty.Register<Space, double>(nameof(Spacing), defaultValue: 10);

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    // 定义方向属性（水平或垂直）
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<Space, Orientation>(nameof(Orientation), defaultValue: Orientation.Vertical);

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    // 测量子控件的大小
    protected override Size MeasureOverride(Size availableSize)
    {
        double totalWidth = 0;
        double totalHeight = 0;

        foreach (var child in Children)
        {
            child.Measure(availableSize);
            if (Orientation == Orientation.Vertical)
            {
                totalHeight += child.DesiredSize.Height;
                totalWidth = Math.Max(totalWidth, child.DesiredSize.Width);
            }
            else
            {
                totalWidth += child.DesiredSize.Width;
                totalHeight = Math.Max(totalHeight, child.DesiredSize.Height);
            }
        }

        // 计算总间距
        if (Orientation == Orientation.Vertical)
        {
            totalHeight += Spacing * (Children.Count - 1);
        }
        else
        {
            totalWidth += Spacing * (Children.Count - 1);
        }

        return new Size(totalWidth, totalHeight);
    }

    // 排列子控件
    protected override Size ArrangeOverride(Size finalSize)
    {
        double currentPosition = 0;

        foreach (var child in Children)
        {
            if (Orientation == Orientation.Vertical)
            {
                child.Arrange(new Rect(0, currentPosition, finalSize.Width, child.DesiredSize.Height));
                currentPosition += child.DesiredSize.Height + Spacing;
            }
            else
            {
                child.Arrange(new Rect(currentPosition, 0, child.DesiredSize.Width, finalSize.Height));
                currentPosition += child.DesiredSize.Width + Spacing;
            }
        }

        return finalSize;
    }
}

