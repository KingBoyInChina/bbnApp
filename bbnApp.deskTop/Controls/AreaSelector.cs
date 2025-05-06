// AreaNodeSelector.cs
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using Avalonia.Controls.Templates;
using bbnApp.deskTop.Common;
using Avalonia.Data;
using Avalonia;
using Avalonia.Controls.Primitives;
using System.Linq;
using bbnApp.DTOs.CodeDto;
namespace bbnApp.deskTop.Controls;

public class AreaNodeSelector : ContentControl
{
    private ComboBox _provinceComboBox;
    private ComboBox _cityComboBox;
    private ComboBox _countyComboBox;
    private ComboBox _townComboBox;
    private Button _resetButton;
    private List<AreaTreeNodeDto> areas;

    // 定义可绑定的 AccessLeve 属性(可选级别)
    public static readonly StyledProperty<int> AccessLeveProperty =
        AvaloniaProperty.Register<AreaNodeSelector, int>(nameof(AccessLeve), 0, defaultBindingMode: BindingMode.TwoWay);

    public int AccessLeve
    {
        get => GetValue(AccessLeveProperty);
        set => SetValue(AccessLeveProperty, value);
    }

    // 默认选中项
    public static readonly StyledProperty<AreaTreeNodeDto> InitValueProperty =
        AvaloniaProperty.Register<AreaNodeSelector, AreaTreeNodeDto>(
            nameof(InitValue),
            null,
            defaultBindingMode: BindingMode.TwoWay,
            coerce: InitValueChanged); // 添加属性变更回调

    public AreaTreeNodeDto InitValue
    {
        get => GetValue(InitValueProperty);
        set => SetValue(InitValueProperty, value);
    }

    // 属性变更回调
    private static AreaTreeNodeDto InitValueChanged(AvaloniaObject obj, AreaTreeNodeDto value)
    {
        var control = obj as AreaNodeSelector;
        if (control != null)
        {
            control.SetInitialValue(value); // 调用 SetInitialValue 方法
        }
        return value;
    }
    /// <summary>
    /// 是否启用
    /// </summary>
    public static readonly StyledProperty<bool> IsAllEnabledProperty =
        AvaloniaProperty.Register<AreaNodeSelector, bool>(nameof(IsAllEnabled), true, defaultBindingMode: BindingMode.TwoWay);

    public bool IsAllEnabled
    {
        get => GetValue(IsAllEnabledProperty);
        set => SetValue(IsAllEnabledProperty, value);
    }
    /// <summary>
    /// 
    /// </summary>
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<AreaNodeSelector, Orientation>(nameof(Orientation), Orientation.Horizontal, defaultBindingMode: BindingMode.TwoWay);

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
    /// <summary>
    /// 是否显示重置按钮
    /// </summary>
    public static readonly StyledProperty<bool> ShowResetButtonProperty =
        AvaloniaProperty.Register<AreaNodeSelector, bool>(nameof(ShowResetButton), false, defaultBindingMode: BindingMode.TwoWay);

    public bool ShowResetButton
    {
        get => GetValue(ShowResetButtonProperty);
        set => SetValue(ShowResetButtonProperty, value);
    }

    // 定义回调事件
    public event EventHandler<AreaTreeNodeDto> SelectionChanged;

    /// <summary>
    /// 重写 OnApplyTemplate 方法
    /// </summary>
    /// <param name="e"></param>
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e); // 传递 TemplateAppliedEventArgs 参数
        try
        {
            _provinceComboBox.SelectionChanged += OnProvinceSelected;
            _cityComboBox.SelectionChanged += OnCitySelected;
            _countyComboBox.SelectionChanged += OnCountySelected;
            _townComboBox.SelectionChanged += OnTownSelected;
            _resetButton.Click += OnResetClicked;

            // 加载数据
            LoadData();
            //初始值设置
            //SetInitialValue();
            //禁用状态
            _provinceComboBox.IsEnabled = IsAllEnabled;
            _cityComboBox.IsEnabled = IsAllEnabled;
            _countyComboBox.IsEnabled = IsAllEnabled;
            _townComboBox.IsEnabled = IsAllEnabled;
            _resetButton.IsEnabled = IsAllEnabled;
            _resetButton.IsVisible = ShowResetButton;
            //是否控制可选范围且未禁用情况下
            if (AccessLeve > 0 && IsAllEnabled)
            {
                _provinceComboBox.IsEnabled = AccessLeve <= 2 ? true : false;
                _cityComboBox.IsEnabled = AccessLeve <= 3 ? true : false;
                _countyComboBox.IsEnabled = AccessLeve <= 4 ? true : false;
                _townComboBox.IsEnabled = AccessLeve <= 5 ? true : false;
            }
        }
        catch(Exception ex)
        {
            throw;
        }
    }

    public AreaNodeSelector()
    {
        InitializeComponent();
    }
    /// <summary>
    /// 控件初始化
    /// </summary>
    private void InitializeComponent()
    {
        // 创建布局
        var stackPanel = new StackPanel
        {
            Orientation = Orientation
        };

        // 创建省选择框
        _provinceComboBox = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxWidth= 100,
        };
        stackPanel.Children.Add(_provinceComboBox);

        // 创建市选择框
        _cityComboBox = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxWidth = 120,
        };
        stackPanel.Children.Add(_cityComboBox);

        // 创建县选择框
        _countyComboBox = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxWidth = 120,
        };
        stackPanel.Children.Add(_countyComboBox);

        // 创建乡选择框
        _townComboBox = new ComboBox
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            MaxWidth = 120,
        };
        stackPanel.Children.Add(_townComboBox);

        // 创建重置按钮
        _resetButton = new Button
        {
            Content = "重置",
            Height=32,
            FontSize=12,
            Classes = { "Basic" },
            HorizontalAlignment = HorizontalAlignment.Stretch
        };
        stackPanel.Children.Add(_resetButton);

        // 设置控件内容
        Content = stackPanel;

    }
    /// <summary>
    /// 加载数据
    /// </summary>
    private void LoadData()
    {
        // 加载行政区划数据
        areas = DicContext.AreaTree;
        if (areas != null)
        {
            // 绑定省数据
            _provinceComboBox.ItemsSource =new List<AreaTreeNodeDto> { null }.Concat(areas).ToList();
            _provinceComboBox.ItemTemplate = CreateItemTemplate();
        }
    }

    private IDataTemplate CreateItemTemplate()
    {
        return new FuncDataTemplate<AreaTreeNodeDto>((area, _) =>
        {
            return new TextBlock { Text = area==null?"--请选择--": area.AreaName };
        });
    }


    private void OnProvinceSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedProvince = _provinceComboBox.SelectedItem as AreaTreeNodeDto;
        _cityComboBox.ItemsSource = selectedProvince?.Children;
        _cityComboBox.ItemTemplate = CreateItemTemplate();
        _cityComboBox.SelectedItem = null;
        _cityComboBox.IsVisible = _cityComboBox.ItemsSource == null ? false : true;
        _countyComboBox.ItemsSource = null;
        _countyComboBox.IsVisible = false;
        _townComboBox.ItemsSource = null;
        _townComboBox.IsVisible = false;
        OnAreaSelected(sender, selectedProvince);
    }

    private void OnCitySelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedCity = _cityComboBox.SelectedItem as AreaTreeNodeDto;
        _countyComboBox.ItemsSource = selectedCity?.Children;
        _countyComboBox.ItemTemplate = CreateItemTemplate();
        _countyComboBox.SelectedItem = null;
        _countyComboBox.IsVisible = _countyComboBox.ItemsSource == null ? false : true;
        _townComboBox.ItemsSource = null;
        _townComboBox.IsVisible = false;
        OnAreaSelected(sender, selectedCity);
    }

    private void OnCountySelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedCounty = _countyComboBox.SelectedItem as AreaTreeNodeDto;
        _townComboBox.ItemsSource = selectedCounty?.Children;
        _townComboBox.ItemTemplate = CreateItemTemplate();
        _townComboBox.SelectedItem = null;
        _townComboBox.IsVisible = _townComboBox.ItemsSource == null ? false : true;
        OnAreaSelected(sender, selectedCounty);
    }

    private void OnTownSelected(object sender, SelectionChangedEventArgs e)
    {
        var selectedTown= _townComboBox.SelectedItem as AreaTreeNodeDto;
        OnAreaSelected(sender,selectedTown);
    }

    private void OnAreaSelected(object sender, AreaTreeNodeDto? selected)
    {
        if (selected != null)
        {
            // 触发回调
            SelectionChanged?.Invoke(this, selected);
        }
    }

    private void OnResetClicked(object sender, RoutedEventArgs e)
    {
        if (AccessLeve <= 0) return;
        if (AccessLeve <= 2) _provinceComboBox.SelectedItem = null;
        if (AccessLeve <= 3) _cityComboBox.SelectedItem = null;
        if (AccessLeve <= 4) _countyComboBox.SelectedItem = null;
        if (AccessLeve <= 5) _townComboBox.SelectedItem = null;
    }

    // 设置初始值
    public void SetInitialValue(AreaTreeNodeDto node)
    {
        if (node == null) return;

        // 根据初始值设置选中项
        var initNode = new AreaTreeNodeDto();
        List<AreaTreeNodeDto> list=new List<AreaTreeNodeDto>() ;
        if (node.AreaId.Length >= 2)
        {
            initNode = SelectNode(areas, node.AreaId.Substring(0, 2));
            if (initNode != null)
            {
                _provinceComboBox.SelectedItem = initNode;
                _cityComboBox.ItemsSource = initNode.Children;
                _cityComboBox.IsVisible = _cityComboBox.ItemsSource == null ? false : true;
                _cityComboBox.ItemTemplate = CreateItemTemplate();
                list = [.. initNode.Children];
            }
            else
            {
                _cityComboBox.ItemsSource = null;
            }
        }
        if (node.AreaId.Length >= 4)
        {
            initNode = SelectNode(list,node.AreaId.Substring(0, 4));
            if (initNode != null)
            {
                _cityComboBox.SelectedItem = initNode;
                _countyComboBox.ItemsSource = initNode.Children;
                _countyComboBox.IsVisible = _countyComboBox.ItemsSource == null ? false : true;
                _countyComboBox.ItemTemplate = CreateItemTemplate();
                list = [.. initNode.Children];
            }
            else
            {
                _countyComboBox.ItemsSource = null;
            }
        }
        if (node.AreaId.Length >= 6)
        {
            initNode = SelectNode(list,node.AreaId.Substring(0, 6));
            if (initNode != null)
            {
                _countyComboBox.SelectedItem = initNode;
                _townComboBox.ItemsSource = initNode.Children;
                _townComboBox.IsVisible = _townComboBox.ItemsSource == null ? false : true;
                _townComboBox.ItemTemplate = CreateItemTemplate();
                list = [.. initNode.Children];
            }
            else
            {
                _townComboBox.ItemsSource = null;
            }
        }
        if (node.AreaId.Length >= 9)
        {
            initNode = SelectNode(list,node.AreaId.Substring(0, 9));
            if (initNode != null)
            {
                _townComboBox.SelectedItem = initNode;
            }
            else
            {
                _townComboBox.SelectedItem = null;
            }
        }
    }

    private AreaTreeNodeDto SelectNode(List<AreaTreeNodeDto>? list,string AreaId)
    {
        var item = list?.Where(x => x.AreaId == AreaId).FirstOrDefault();
        return item;
    }
}


