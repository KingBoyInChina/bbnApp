using AutoMapper;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.PlatformManagement.TopicCode;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.PlatformManagement.TopicCode
{
    partial class TopicCodeViewModel : BbnPageBase
    {
        /// <summary>
        /// 当前控件
        /// </summary>
        private UserControl NowControl;
        /// <summary>
        /// 
        /// </summary>
        public readonly IDialog dialog;
        /// <summary>
        /// 
        /// </summary>
        public readonly ISukiDialogManager dialogManager;
        /// <summary>
        /// 
        /// </summary>
        private readonly PageNavigationService nav;
        /// <summary>
        /// 图片加载状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 订阅代码服务
        /// </summary>
        private TopicCodesGrpc.TopicCodesGrpcClient _client;
        private DeviceCodeGrpc.DeviceCodeGrpcClient _deviceClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 资产树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<TopicCodesTreeNodeDto> _topicTreeSource = new ObservableCollection<TopicCodesTreeNodeDto>();
        /// <summary>
        /// 选中的树节点
        /// </summary>
        private TopicCodesTreeNodeDto _selectedTreeNode = new TopicCodesTreeNodeDto();
        /// <summary>
        /// 选中的订阅
        /// </summary>
        [ObservableProperty] private TopicCodesItemDto _topicCodeSelected = new TopicCodesItemDto();
        /// <summary>
        /// 查询条件
        /// </summary>
        [ObservableProperty] private string _filterName = string.Empty;
        /// <summary>
        /// 1214 订阅类型
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _topicTypeList = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _topicTypeListSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 1101 设备类型
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _deviceTypeList = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _deviceTypeListSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 设备清单
        /// </summary>
        [ObservableProperty] private ObservableCollection<DeviceCodeItemDto> _deviceList = new ObservableCollection<DeviceCodeItemDto>();
        [ObservableProperty] private ObservableCollection<DeviceItem> _tempDeviceList = new ObservableCollection<DeviceItem>();
        /// <summary>
        /// 自动生成的局部方法，会在属性值变更时被调用
        /// </summary>
        /// <param name="value"></param>
        partial void OnDeviceTypeListSelectedChanged(ComboboxItem item)
        {
            if (item != null)
            {
                DeviceFilter(item.Id);
            }
        }
        /// <summary>
        /// 过滤
        /// </summary>
        /// <param name="value"></param>
        private void DeviceFilter(string value)
        {
            var data = DeviceList.Where(x => x.DeviceType == value).Select(d => new DeviceItem { DeviceId=d.DeviceId, DeviceName=d.DeviceName, DeviceModel=d.DeviceModel, IsChecked = TopicCodeSelected.DeviceIds.Contains(d.DeviceId) }).ToList();
            TempDeviceList =new ObservableCollection<DeviceItem>(data);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public TopicCodeViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("PlatformManagement", "订阅代码", MaterialIconKind.NoticeBoard, "", 6)
        {
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _client = grpcClientFactory.CreateClient<TopicCodesGrpc.TopicCodesGrpcClient>();
            _deviceClient = grpcClientFactory.CreateClient<DeviceCodeGrpc.DeviceCodeGrpcClient>();
            _mapper = mapper;
            this.dialog = dialog;
        }

        /// <summary>
        /// 初始化字典
        /// </summary>
        public void TopicCodesDicInit(UserControl uc)
        {
            NowControl = uc;//当前控件

            TopicTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1214"));
            DeviceTypeList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1101"));
            DeviceTypeList.Add(new ComboboxItem("-1","不限","ALL"));
            _ = DeviceListLoad();
            TopicTreeLoad();
        }
        /// <summary>
        /// 设备清单
        /// </summary>
        private async Task DeviceListLoad()
        {
            try
            {
                DeviceCodeSearchRequestDto request = new DeviceCodeSearchRequestDto
                {
                    DeviceType = "",
                    DeviceBarCode="",
                    DeviceModel="",
                    DeviceName=""
                };
                var data = await _deviceClient.DeviceListLoadAsync(_mapper.Map<DeviceCodeSearchRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    DeviceList =new ObservableCollection<DeviceCodeItemDto>(_mapper.Map<List<DeviceCodeItemDto>>(data.List));
                    TempDeviceList =new ObservableCollection<DeviceItem>(DeviceList.Select(x=>new DeviceItem { DeviceId=x.DeviceId,DeviceModel=x.DeviceModel,DeviceName=x.DeviceName,IsChecked=TopicCodeSelected.DeviceIds.Contains(x.DeviceId)}).ToList());
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 订阅树加载
        /// </summary>
        private void TopicTreeLoad()
        {
            try
            {
                TopicCodesTreeRequestDto request = new TopicCodesTreeRequestDto
                {
                    FilterValue = FilterName
                };
                dialog.ShowLoading("加载订阅树中...", async e =>
                {
                    var data = await _client.TopicCodesTreeLoadAsync(_mapper.Map<TopicCodesTreeRequest>(request), CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        TopicTreeSource = new ObservableCollection<TopicCodesTreeNodeDto>(_mapper.Map<List<TopicCodesTreeNodeDto>>(data.TopicCodesItems));
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }

                });
            }
            catch (Exception ex)
            {
                dialog.Error("加载订阅树失败", ex.Message);
            }
        }
        /// <summary>
        /// 订阅树刷新
        /// </summary>
        [RelayCommand]
        private void TopicTreeReload()
        {
            TopicTreeLoad();
        }
        /// <summary>
        /// 树选中
        /// </summary>
        /// <param name="node"></param>
        public void TreeSelecte(TopicCodesTreeNodeDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if (node.IsLeaf)
                {
                    NodeInfoLoad(node);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 订阅信息
        /// </summary>
        private void NodeInfoLoad(TopicCodesTreeNodeDto node)
        {
            dialog.ShowLoading("数据读取中...", async e => {
                TopicCodesInfoRequestDto request = new TopicCodesInfoRequestDto { TopicId = node.Id };
                var header = CommAction.GetHeader();
                var data = await _client.TopicCodesInfoLoadAsync(_mapper.Map<TopicCodesInfoRequest>(request), header);
                dialog.LoadingClose(e);
                if (data.Code)
                {
                    TopicCodeSelected = _mapper.Map<TopicCodesItemDto>(data.Item);
                    //设置选中项目
                    SetItemSelected();
                }
                else
                {
                    dialog.Error("提示", data.Message);
                }
            });

        }
        /// <summary>
        /// 设置选中项目
        /// </summary>
        private void SetItemSelected()
        {
            TopicTypeListSelected = CommAction.SetSelectedItem(TopicTypeList, TopicCodeSelected.TopicType);
            DeviceTypeListSelected= CommAction.SetSelectedItem(DeviceTypeList, TopicCodeSelected.DeviceType);
            TempDeviceList = new ObservableCollection<DeviceItem>(DeviceList.Select(x => new DeviceItem { DeviceId = x.DeviceId, DeviceModel = x.DeviceModel, DeviceName = x.DeviceName, IsChecked = TopicCodeSelected.DeviceIds.Contains(x.DeviceId) }).ToList());
        }
        /// <summary>
        /// 新建订阅
        /// </summary>
        [RelayCommand]
        private async Task AddTopicCode()
        {
            TopicCodeSelected = new TopicCodesItemDto();
            await Task.Delay(200);
            TopicCodeSelected.IdxNum = 0;
            TopicCodeSelected.DeviceType = "-1";
            SetItemSelected();
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private void TopicCodeSave()
        {
            try
            {
                StringBuilder _error = new StringBuilder();
                TopicCodeSelected.TopicType = TopicTypeListSelected?.Id;
                TopicCodeSelected.DeviceType = DeviceTypeListSelected?.Id;
                var devicesids= TempDeviceList.Where(x => x.IsChecked).ToList();
                string devicesid = string.Empty;
                foreach (var device in devicesids) {
                    devicesid += device.DeviceId + ",";
                }
                TopicCodeSelected.DeviceIds = devicesid.TrimEnd(',');
                if (string.IsNullOrEmpty(TopicCodeSelected.TopicName))
                {
                    _error.AppendLine($"订阅名称不能为空");
                }
                if (string.IsNullOrEmpty(TopicCodeSelected.TopicType))
                {
                    _error.AppendLine($"订阅类型不能为空");
                }
                if (string.IsNullOrEmpty(TopicCodeSelected.TopicRoter))
                {
                    _error.AppendLine($"订阅路由不能为空");
                }
                if (!string.IsNullOrEmpty(_error.ToString()))
                {
                    dialog.Error("提示", _error.ToString());
                    return;
                }

                dialog.ShowLoading("数据提交中...", async e => {
                    try
                    {
                        TopicCodesPostRequestDto request = new TopicCodesPostRequestDto
                        {
                            TopicCodesItem = TopicCodeSelected
                        };
                        var header = CommAction.GetHeader();
                        var data = await _client.TopicCodesPostAsync(_mapper.Map<TopicCodesPostRequest>(request), header);

                        if (data.Code)
                        {
                            dialog.Success("提示", data.Message);

                            TopicCodeSelected = _mapper.Map<TopicCodesItemDto>(data.Item);
                            TopicTreeLoad();
                        }
                        else
                        {
                            dialog.Error("提示", data.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        dialog.Error("提示", ex.Message.ToString());
                    }
                    finally
                    {
                        dialog.LoadingClose(e);
                    }
                });

            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 订阅状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="node"></param>
        public async Task TopicState(string type, TopicCodesTreeNodeDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if (type == "IsLock")
                {
                    TopicCodeStatePost(type, node.Id, "");
                }
                else
                {
                    if (!node.IsLock)
                    {
                        var data = await dialog.Prompt("请输入锁定原因", "确定");
                        if (data.Item1)
                        {
                            TopicCodeStatePost(type, node.Id, data.Item2);
                        }
                    }
                    else
                    {
                        TopicCodeStatePost(type, node.Id, "");
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 订阅状态变更请求
        /// </summary>
        /// <param name="type"></param>
        /// <param name="Id"></param>
        /// <param name="Reason"></param>
        private void TopicCodeStatePost(string type, string Id, string Reason)
        {
            dialog.ShowLoading("数据处理中...", async e => {
                TopicCodesStateRequestDto request = new TopicCodesStateRequestDto
                {
                    State = "IsDelete",
                    TopicId = Id,
                    Reason = ""
                };
                var data = await _client.TopicCodesStateAsync(_mapper.Map<TopicCodesStateRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    dialog.Success("提示", data.Message);
                    //刷新树
                    TopicTreeLoad();
                    if (type == "IsDelete")
                    {
                        _selectedTreeNode = new TopicCodesTreeNodeDto();
                        TopicCodeSelected = new TopicCodesItemDto();
                    }
                    else
                    {
                        NodeInfoLoad(_selectedTreeNode);
                    }
                }
                else
                {
                    dialog.Error("提示", data.Message);
                }
            });
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DeviceItem
    {
        public string DeviceId { get; set; } = string.Empty;
        public string DeviceName { get; set; } = string.Empty;  
        public string DeviceModel { get; set; } = string.Empty;
        public bool IsChecked { get; set; } = false;
    }
}
