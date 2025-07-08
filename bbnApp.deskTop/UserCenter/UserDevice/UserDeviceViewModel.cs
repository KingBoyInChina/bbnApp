using AutoMapper;
using Avalonia.Controls;
using Avalonia.Styling;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.Features;
using bbnApp.deskTop.Models.Device;
using bbnApp.deskTop.Models.User;
using bbnApp.deskTop.OrganizationStructure.ReigisterKey;
using bbnApp.deskTop.Services;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.MQTT.Client;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Icons;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.UserCenter.UserDevice
{
    partial class UserDeviceViewModel : BbnPageBase
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
        /// 用户设备
        /// </summary>
        private UserDeviceGrpc.UserDeviceGrpcClient _client;
        /// <summary>
        /// 设备代码
        /// </summary>
        private DeviceCodeGrpc.DeviceCodeGrpcClient _deviceClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// 用户tree
        /// </summary>
        [ObservableProperty] private ObservableCollection<UserDeviceTreeItemDto> _userDeviceTreeSource = new ObservableCollection<UserDeviceTreeItemDto>();
        /// <summary>
        /// 查询条件-行政区划代码
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _areaFilterSelected;
        /// <summary>
        /// 查询-默认选中项
        /// </summary>
        [ObservableProperty] private AreaTreeNodeDto _initFilterValue;
        /// <summary>
        /// 查询-联系电话
        /// </summary>
        [ObservableProperty] private string _filterPhoneNumber = string.Empty;
        /// <summary>
        /// 查询-用户名称
        /// </summary>
        [ObservableProperty] private string _filterUserName = string.Empty;
        /// <summary>
        /// 选中用户节点
        /// </summary>
        [ObservableProperty] private UserDeviceTreeItemDto _selectedNode = new UserDeviceTreeItemDto();
        /// <summary>
        /// 节点选中
        /// </summary>
        /// <param name="value"></param>
        partial void OnSelectedNodeChanged(UserDeviceTreeItemDto node)
        {
            try
            {
                if (node != null)
                {
                    if ((bool)node.IsLeaf)
                    {
                        NodeInfoLoad(node);
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 用户信息
        /// </summary>
        [ObservableProperty] private UserInformationDto _userInformation = new UserInformationDto();
        /// <summary>
        /// 网关信息
        /// </summary>
        [ObservableProperty] private ObservableCollection<UserGetWayCus> _getWays = new ObservableCollection<UserGetWayCus>();
        /// <summary>
        /// 边缘盒子信息
        /// </summary>
        [ObservableProperty] private ObservableCollection<UserBoxCus> _boxs = new ObservableCollection<UserBoxCus>();

        /// <summary>
        /// 用户类型
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _userType = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 表单选中的
        /// </summary>
        [ObservableProperty] private ComboboxItem _userSelectedType = new ComboboxItem("", "", "");
        /// <summary>
        /// 用户级别
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _userLeve = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 种养分类 1218
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _aabType = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 种养类型 1203+1217
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _categorys = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 计量单位 1209
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _units = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 分布情况 1219
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _distributionDatas = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 规模情况 1220
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _scales = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的规模
        /// </summary>
        [ObservableProperty] private ComboboxItem _userSelectedScale = new ComboboxItem("", "", "");

        /// <summary>
        /// 选中的用户级别
        /// </summary>
        [ObservableProperty] private ComboboxItem _userSelectedLeve = new ComboboxItem("", "", "");
        /// <summary>
        /// 标准设备代码
        /// </summary>
        private List<DeviceCodeItemDto> _deviceCodeItems = new List<DeviceCodeItemDto>();
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private UserControl _content;
        /// <summary>
        /// 编辑状态
        /// </summary>
        [ObservableProperty] private bool _isEdit = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public UserDeviceViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog, MqttClientService _mqttClientService) : base("UserCenter", "用户设备维护", MaterialIconKind.Devices, "", 2)
        {
            _ = ClientInit(grpcClientFactory);
            this.dialogManager = DialogManager;

            this.nav = nav;
            this.dialog = dialog;
            _mapper = mapper;
        }
        /// <summary>
        /// 初始化Client
        /// </summary>
        /// <param name="grpcClientFactory"></param>
        /// <returns></returns>
        private async Task ClientInit(IGrpcClientFactory grpcClientFactory)
        {
            _client = await grpcClientFactory.CreateClient<UserDeviceGrpc.UserDeviceGrpcClient>();
            _deviceClient = await grpcClientFactory.CreateClient<DeviceCodeGrpc.DeviceCodeGrpcClient>();
        }
        /// <summary>
        /// 字典初始化
        /// </summary>
        public async Task DataInit()
        {
            UserType = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1215"));
            UserLeve = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1216"));
            Units = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1209"));
            AabType = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1218"));
            DistributionDatas = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1219"));
            Scales = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1220"));

            AreaFilterSelected = new AreaTreeNodeDto
            {
                AreaId = UserContext.CurrentUser.AreaCode,
                AreaFullName = UserContext.CurrentUser.AreaName,
                AreaName = UserContext.CurrentUser.AreaName
            };
            InitFilterValue = new AreaTreeNodeDto
            {
                AreaId = UserContext.CurrentUser.AreaCode,
                AreaFullName = UserContext.CurrentUser.AreaName,
                AreaName = UserContext.CurrentUser.AreaName
            };
            await DeviceCodeLoad();
            UserLoadCommand.Execute(null);
        }
        /// <summary>
        /// 标准设备字典读取
        /// </summary>
        private async Task DeviceCodeLoad()
        {
            try
            {
                var request =new DeviceCodeSearchRequestDto{ 
                    DeviceBarCode="",
                    DeviceModel="",
                    DeviceName="",
                    DeviceType=""
                };
                var data = await _deviceClient.DeviceListLoadAsync(_mapper.Map<DeviceCodeSearchRequest>(request),CommAction.GetHeader());
                if (data.Code)
                {
                    _deviceCodeItems =_mapper.Map<List<DeviceCodeItemDto>>(data.List);
                }
            }
            catch (Exception ex) {
                dialog.Error("提示",$"设备字典初始化异常：{ex.Message.ToString()}");
            }
        }

        /// <summary>
        /// 字典初始化
        /// </summary>
        private void DicInit()
        {
            UserSelectedType = CommAction.SetSelectedItem(UserType, UserInformation.UserType);
            UserSelectedLeve = CommAction.SetSelectedItem(UserLeve, UserInformation.UserLeve);
            UserSelectedScale = CommAction.SetSelectedItem(Scales, UserInformation.Scale);
        }
        /// <summary>
        /// 读取节点信息
        /// </summary>
        /// <param name="node"></param>
        private void NodeInfoLoad(UserDeviceTreeItemDto node)
        {
            SelectedNode = node;
            dialog.ShowLoading("数据加载中...", async (e) =>
            {
                try
                {
                    var request = new UserDeviceListRequestDto
                    {
                        UserId = node.Id
                    };
                    var data = await _client.UserDeviceListAsync(_mapper.Map<UserDeviceListRequest>(request), CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        UserInformation = _mapper.Map<UserInformationDto>(data.User);
                        var _GetWays = _mapper.Map<List<UserGetWayDto>>(data.GetWays);
                        var _Boxs = _mapper.Map<List<UserBoxDto>>(data.Boxs);
                        GetWays =[.. _GetWays.Select(x=> GetWayDtoToCus(x))];
                        Boxs = [.._Boxs.Select(x=> GetBoxDtoToCus(x))];
                        DicInit();
                    }
                    else
                    {
                        dialog.Error("提示", $"用户信息读取失败：{data.Message}");
                    }
                }
                catch (Exception ex)
                {
                    dialog.LoadingClose(e);
                    dialog.Error("提示", $"用户信息读取异常：{ex.Message.ToString()}");
                }
            });
        }

        private UserGetWayCus GetWayDtoToCus(UserGetWayDto model)
        {
            return new UserGetWayCus
            {
                IdxNum = model.IdxNum,
                Yhid = model.Yhid,
                DeviceId = model.DeviceId,
                DeviceName = model.DeviceName,
                Installer = model.Installer,
                GetWayId = model.GetWayId,
                GetWayLocation = model.GetWayLocation,
                GetWayName = model.GetWayName,
                GetWayNumber = model.GetWayNumber,
                InstallerId = model.InstallerId,
                InstallTime = model.InstallTime,
                IsLock = model.IsLock,
                UserId = model.UserId,
                LockReason = model.LockReason,
                LockTime = model.LockTime,
                ReMarks = model.ReMarks,
                Warranty = model.Warranty,
                WlanType = model.WlanType,
                Devices = [..model.Devices]
            };
        }

        private UserBoxCus GetBoxDtoToCus(UserBoxDto model)
        {
            return new UserBoxCus
            {
                IdxNum = model.IdxNum,
                Yhid = model.Yhid,
                DeviceId = model.DeviceId,
                DeviceName = model.DeviceName,
                Installer = model.Installer,
                BoxId = model.BoxId,
                BoxLocation = model.BoxLocation,
                BoxName = model.BoxName,
                BoxNumber = model.BoxNumber,
                InstallerId = model.InstallerId,
                InstallTime = model.InstallTime,
                IsLock = model.IsLock,
                UserId = model.UserId,
                LockReason = model.LockReason,
                LockTime = model.LockTime,
                ReMarks = model.ReMarks,
                Warranty = model.Warranty,
                WlanType = model.WlanType,
                UserCameras = [.. model.UserCameras]
            };
        }

        /// <summary>
        /// 用户查询
        /// </summary>
        [RelayCommand]
        private void UserLoad()
        {
            dialog.ShowLoading("数据查询中...", async e =>
            {
                try
                {
                    var request = new UserDeviceTreeRequestDto
                    {
                        AreaId = AreaFilterSelected.AreaId,
                        PhoneNumber = FilterPhoneNumber,
                        UserName = FilterUserName
                    };
                    var data = await _client.UserDeviceTreeAsync(_mapper.Map<UserDeviceTreeRequest>(request), CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        UserDeviceTreeSource = [.. _mapper.Map<List<UserDeviceTreeItemDto>>(data.Items)];
                    }
                    else
                    {
                        dialog.Error("数据查询失败", data.Message);
                    }
                }
                catch (Exception ex)
                {
                    dialog.LoadingClose(e);
                    dialog.Error("数据查询异常", ex.Message.ToString());
                }
            });
        }
        #region 网关
        /// <summary>
        /// 新建网关
        /// </summary>
        [RelayCommand]
        private void GetWayAdd()
        {
            try
            {
                if (string.IsNullOrEmpty(UserInformation.UserId))
                {
                    dialog.Error("提示","请选择用户");
                    return;
                }
                GetWayContent(new UserGetWayDto
                {
                    UserId = UserInformation.UserId
                });
            }
            catch(Exception ex)
            {
                dialog.Error("提示",$"新建网关异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void GetWayContent(UserGetWayDto Item)
        {
            try
            {
                IsEdit = true;
                var viewModel = new UserGetWayViewModel();
                Content = new UserGetWayView { DataContext = viewModel };
                viewModel.ViewModelInit(dialog, SubmitCallBack, Item, _client, _mapper,_deviceCodeItems);

            }
            catch (Exception ex)
            {
                IsEdit = false;
                dialog.Error("异常提示", $"数据新增操作异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 网关状态变更
        /// </summary>
        /// <param name="Tag"></param>
        /// <param name="Item"></param>
        public async Task GetWayState(string Tag,UserGetWayDto Item)
        {
            try
            {
                if (Tag == "AddDevice")
                {
                    var device = new UserGetWayDeviceDto { 
                        UserId=SelectedNode?.Id??string.Empty,
                        GetWayId=Item.GetWayId
                    };
                    GetWayDeviceWindow(device);
                }
                else if (Tag == "GetWayEdit")
                {
                    GetWayContent(Item);
                }
                else
                {
                    var request = new UserDeviceStateRequestDto
                    {
                        Type = Tag,
                        UserId = SelectedNode?.Id ?? string.Empty,
                        GetWayId = Item.GetWayId,
                        GetWayDeviceId = "",
                        BoxId = "",
                        CameraId = "",
                        Reason = ""
                    };
                    if (Tag == "GetWayLock" && Item.IsLock == 0)
                    {
                        var reason = await dialog.Prompt("请填写停用原因说明", "确定");
                        if (reason.Item1)
                        {
                            request.Reason = reason.Item2;
                            StateSave(request);
                        }
                    }
                    else
                    {
                        StateSave(request);
                    }
                }
            }
            catch(Exception ex)
            {
                dialog.Error("提示",ex.Message.ToString());
            }
        }
        /// <summary>
        /// 网关关联设备信息提交
        /// </summary>
        /// <param name="item"></param>
        public void GetWayDeviceWindow(UserGetWayDeviceDto Item)
        {
            try
            {
                IsEdit = true;
                var viewModel = new UserGetWayDeviceViewModel();
                Content = new UserGetWayDeviceView { DataContext = viewModel };
                viewModel.ViewModelInit(dialog, SubmitCallBack, Item, _client, _mapper, _deviceCodeItems);
            }
            catch(Exception ex)
            {
                dialog.Error("提示",$"网关关联设备添加异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 网关状态变更
        /// </summary>
        /// <param name="Tag"></param>
        /// <param name="Item"></param>
        public async Task GetWayDeviceState(string Tag,UserGetWayDeviceDto Item)
        {
            try
            {
                if (Tag == "GetWayDeviceEdit")
                {
                    GetWayDeviceWindow(Item);
                }
                else
                {
                    var request = new UserDeviceStateRequestDto
                    {
                        Type = Tag,
                        UserId = SelectedNode?.Id ?? string.Empty,
                        GetWayId = Item.GetWayId,
                        GetWayDeviceId = Item.GetWayDeviceId,
                        BoxId = "",
                        CameraId = "",
                        Reason = ""
                    };

                    if (Tag == "GetWayDeviceLock" && Item.IsLock == 1)
                    {
                        var reason = await dialog.Prompt("请填写停用原因说明", "确定");
                        if (reason.Item1)
                        {
                            request.Reason = reason.Item2;
                            StateSave(request);
                        }
                    }
                    else
                    {
                        StateSave(request);
                    }

                }
                
            }
            catch(Exception ex)
            {
                dialog.Error("异常",$"网关状态变更异常：{ex.Message.ToString()}");
            }
        }
        #endregion
        #region 边缘盒子
        /// <summary>
        /// 新建盒子
        /// </summary>
        [RelayCommand]
        private void BoxAdd()
        {
            try
            {
                if (string.IsNullOrEmpty(UserInformation.UserId))
                {
                    dialog.Error("提示", "请选择用户");
                    return;
                }
                BoxContent(new UserBoxDto
                {
                    UserId = UserInformation.UserId
                });
            }
            catch (Exception ex)
            {
                dialog.Error("提示", $"新建盒子异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        private void BoxContent(UserBoxDto Item)
        {
            try
            {
                IsEdit = true;
                var viewModel = new UserBoxViewModel();
                Content = new UserBoxView { DataContext = viewModel };
                viewModel.ViewModelInit(dialog, SubmitCallBack, Item, _client, _mapper, _deviceCodeItems);

            }
            catch (Exception ex)
            {
                IsEdit = false;
                dialog.Error("异常提示", $"数据新增操作异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 盒子状态变更
        /// </summary>
        /// <param name="Tag"></param>
        /// <param name="Item"></param>
        public async Task BoxState(string Tag, UserBoxDto Item)
        {
            try
            {
                if (Tag == "AddCamera")
                {
                    var device = new UserCameraDto
                    {
                        UserId = SelectedNode?.Id ?? string.Empty,
                        BoxId = Item.BoxId
                    };
                    CameraWindow(device);
                }
                else if (Tag == "BoxEdit")
                {
                    BoxContent(Item);
                }
                else
                {
                    var request = new UserDeviceStateRequestDto
                    {
                        Type = Tag,
                        UserId = SelectedNode?.Id ?? string.Empty,
                        GetWayId = "",
                        GetWayDeviceId = "",
                        BoxId = Item.BoxId,
                        CameraId = "",
                        Reason = ""
                    };
                    if (Tag == "BoxLock" && Item.IsLock == 0)
                    {
                        var reason = await dialog.Prompt("请填写停用原因说明", "确定");
                        if (reason.Item1)
                        {
                            request.Reason = reason.Item2;
                            StateSave(request);
                        }
                    }
                    else
                    {
                        StateSave(request);
                    }
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 摄像头设备信息提交
        /// </summary>
        /// <param name="item"></param>
        public void CameraWindow(UserCameraDto Item)
        {
            try
            {
                IsEdit = true;
                var viewModel = new UserCameraViewModel();
                Content = new UserCameraView { DataContext = viewModel };
                viewModel.ViewModelInit(dialog, SubmitCallBack, Item, _client, _mapper, _deviceCodeItems);
            }
            catch (Exception ex)
            {
                dialog.Error("提示", $"摄像头设备添加异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 摄像头状态变更
        /// </summary>
        /// <param name="Tag"></param>
        /// <param name="Item"></param>
        public async Task CameraState(string Tag, UserCameraDto Item)
        {
            try
            {
                if (Tag == "CameraEdit")
                {
                    CameraWindow(Item);
                }
                else
                {
                    var request = new UserDeviceStateRequestDto
                    {
                        Type = Tag,
                        UserId = SelectedNode?.Id ?? string.Empty,
                        GetWayId = "",
                        GetWayDeviceId = "",
                        BoxId = Item.BoxId,
                        CameraId = Item.CameraId,
                        Reason = ""
                    };

                    if (Tag == "CameraLock" && Item.IsLock == 1)
                    {
                        var reason = await dialog.Prompt("请填写停用原因说明", "确定");
                        if (reason.Item1)
                        {
                            request.Reason = reason.Item2;
                            StateSave(request);
                        }
                    }
                    else
                    {
                        StateSave(request);
                    }

                }

            }
            catch (Exception ex)
            {
                dialog.Error("异常", $"摄像头状态变更异常：{ex.Message.ToString()}");
            }
        }
        #endregion
        /// <summary>
        /// 代码提交回调
        /// </summary>
        private void SubmitCallBack(bool success, string message, object data)
        {
            if (success)
            {
                dialog.Success("提示", message);
                IsEdit = false;
                Content = null;
                if (SelectedNode != null)
                {
                    NodeInfoLoad(SelectedNode);
                }
                else
                {
                    UserLoadCommand.Execute(null);
                }
            }
            else if (!success && message == "关闭")
            {
                IsEdit = false;
                Content = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void StateSave(UserDeviceStateRequestDto request)
        {
            dialog.ShowLoading("数据提交中...", async (e) =>
            {
                try
                {
                    var data = await _client.UserDeviceStateAsync(_mapper.Map<UserDeviceStateRequest>(request),CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        dialog.Success("提示",data.Message);
                        var _GetWays = _mapper.Map<List<UserGetWayDto>>(data.GetWay);
                        var _Boxs = _mapper.Map<List<UserBoxDto>>(data.Box);
                        GetWays = [.. _GetWays.Select(x => GetWayDtoToCus(x))];
                        Boxs = [.. _Boxs.Select(x => GetBoxDtoToCus(x))];
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }
                }
                catch (Exception ex)
                {
                    dialog.LoadingClose(e);
                    dialog.Error("状态变更异常", $"状态变更提交异常：{ex.Message.ToString()}");
                }
            });
        }
    }
}
