using AutoMapper;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;

using bbnApp.deskTop.Models.Device;
using bbnApp.deskTop.PlatformManagement.DeviceCode;
using bbnApp.deskTop.Services;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.Protos;
using bbnApp.Share;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Grpc.Net.Client.Balancer;
using Material.Icons;
using Org.BouncyCastle.Asn1.Ocsp;
using SukiUI.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static bbnApp.Share.ModbusHelper;

namespace bbnApp.deskTop.PlatformManagement.DeviceCommand
{
    partial class DeviceCommandViewModel : BbnPageBase
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
        /// 设备指令服务
        /// </summary>
        private DeviceCommandGrpc.DeviceCommandGrpcClient _client;
        /// <summary>
        /// 设备代码服务
        /// </summary>
        private DeviceCodeGrpc.DeviceCodeGrpcClient _clientCode;
        /// <summary>
        /// 上传服务
        /// </summary>
        private UploadFileGrpc.UploadFileGrpcClient _uploadClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 资产树数据源
        /// </summary>
        [ObservableProperty] private ObservableCollection<DeviceCodeTreeNodeDto> _deviceTreeSource = new ObservableCollection<DeviceCodeTreeNodeDto>();
        /// <summary>
        /// 选中的树节点
        /// </summary>
        private DeviceCodeTreeNodeDto _selectedTreeNode = new DeviceCodeTreeNodeDto();
        /// <summary>
        /// 选中的设备
        /// </summary>
        [ObservableProperty] private DeviceCodeItemDto _deviceCodeSelected = new DeviceCodeItemDto();
        /// <summary>
        /// 临时设备构成对象，添加或编辑时使用
        /// </summary>
        [ObservableProperty] private DeviceStructGridItemDto _tempDeviceStructObj = new DeviceStructGridItemDto();
        /// <summary>
        /// 查询条件
        /// </summary>
        [ObservableProperty] private string _filterName = string.Empty;
        /// <summary>
        /// 1222 硬件通信方式
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _hardwareCPList = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _hardwareCPSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 1223  应用通信方式
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _applicationCPList = new ObservableCollection<ComboboxItem>();
        [ObservableProperty] private ComboboxItem _applicationCPSelected = new ComboboxItem("", "", "");
        /// <summary>
        /// 指令清单
        /// </summary>
        [ObservableProperty] private ObservableCollection<DeviceCommandDto> _deviceCommandSource = new ObservableCollection<DeviceCommandDto>();
        [ObservableProperty] private DeviceCommandDto _deviceCommandSelected = new DeviceCommandDto();
        /// <summary>
        /// 指令
        /// </summary>
        [ObservableProperty] private string _modbuscommand = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        partial void OnDeviceCommandSelectedChanged(DeviceCommandDto? oldValue, DeviceCommandDto newValue)
        {
            SetItemSelected();
        }
        /// <summary>
        /// 显示图片
        /// </summary>
        [ObservableProperty] private Bitmap _deviceCodeImage;
        /// <summary>
        /// 是否有图片
        /// </summary>
        [ObservableProperty] private bool _imageShow = false;
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private FileItemsDto _deviceCodeImageInfo = new FileItemsDto();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToastManager"></param>
        /// <param name="DialogManager"></param>
        /// <param name="nav"></param>
        public DeviceCommandViewModel(ISukiDialogManager DialogManager, PageNavigationService nav, IGrpcClientFactory grpcClientFactory, IMapper mapper, IDialog dialog) : base("PlatformManagement", "从站指令设置", MaterialIconKind.Sitemap, "", 6)
        {
            this.dialogManager = DialogManager;
            this.nav = nav;
            this.dialog = dialog;
            _clientCode = grpcClientFactory.CreateClient<DeviceCodeGrpc.DeviceCodeGrpcClient>().GetAwaiter().GetResult();
            _client = grpcClientFactory.CreateClient<DeviceCommandGrpc.DeviceCommandGrpcClient>().GetAwaiter().GetResult();
            _uploadClient = grpcClientFactory.CreateClient<UploadFileGrpc.UploadFileGrpcClient>().GetAwaiter().GetResult();
            _mapper = mapper;
            this.dialog = dialog;
        }
        /// <summary>
        /// 初始化字典
        /// </summary>
        public void MaterialDicInit(UserControl uc)
        {
            NowControl = uc;//当前控件

            HardwareCPList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1222"));
            ApplicationCPList = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1223"));
            DeviceTreeLoad();
        }
        /// <summary>
        /// 设备树加载
        /// </summary>
        private void DeviceTreeLoad()
        {
            try
            {
                DeviceCodeTreeRequestDto request = new DeviceCodeTreeRequestDto
                {
                    FilterValue = FilterName
                };
                dialog.ShowLoading("加载设备树中...", async e =>
                {
                    var data = await _clientCode.DeviceTreeLoadAsync(_mapper.Map<DeviceCodeTreeRequest>(request), CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        DeviceTreeSource = new ObservableCollection<DeviceCodeTreeNodeDto>(_mapper.Map<List<DeviceCodeTreeNodeDto>>(data.DeviceCodeItems));
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }

                });
            }
            catch (Exception ex)
            {
                dialog.Error("加载设备树失败", ex.Message);
            }
        }
        /// <summary>
        /// 设置选中项目
        /// </summary>
        private void SetItemSelected()
        {
            HardwareCPSelected = CommAction.SetSelectedItem(HardwareCPList, DeviceCommandSelected?.HardwareCP??string.Empty);
            ApplicationCPSelected = CommAction.SetSelectedItem(ApplicationCPList, DeviceCommandSelected?.ApplicationCP ?? string.Empty);
        }
        /// <summary>
        /// 设备树刷新
        /// </summary>
        [RelayCommand]
        private void DeviceTreeReload()
        {
            DeviceTreeLoad();
        }
        /// <summary>
        /// 树选中
        /// </summary>
        /// <param name="node"></param>
        public async Task TreeSelected(DeviceCodeTreeNodeDto node)
        {
            try
            {
                _selectedTreeNode = node;
                if (node.IsLeaf)
                {
                    CommandListLoad(node.Id);
                    await FileRead(node.Id);
                }
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
        }
        /// <summary>
        /// 指令重载
        /// </summary>
        [RelayCommand]
        private void CommandReload()
        {
            if (_selectedTreeNode != null && _selectedTreeNode.IsLeaf)
            {
                CommandListLoad(_selectedTreeNode?.Id ?? string.Empty);
            }
        }
        /// <summary>
        /// 获取指令清单
        /// </summary>
        /// <param name="node"></param>
        private void CommandListLoad(string DeviceId)
        {
            try
            {
                var request = new DeviceCommandListRequestDto
                {
                    DeviceId = DeviceId
                };
                dialog.ShowLoading("设备指令加载中...", async e =>
                {
                    var data = await _client.DeviceCommandListAsync(_mapper.Map<DeviceCommandListRequest>(request), CommAction.GetHeader());
                    dialog.LoadingClose(e);
                    if (data.Code)
                    {
                        DeviceCommandSource = new ObservableCollection<DeviceCommandDto>(_mapper.Map<List<DeviceCommandDto>>(data.DeviceCommands));
                    }
                    else
                    {
                        dialog.Error("提示", data.Message);
                    }

                });
            }
            catch (Exception ex)
            {
                dialog.Error("加载设备树失败", ex.Message);
            }
        }
        /// <summary>
        /// 图片信息清空
        /// </summary>
        private void ImageClear()
        {
            ImageShow = false;
            if (DeviceCodeImage != null)
            {
                DeviceCodeImage.Dispose();
                DeviceCodeImage = null;
            }
            DeviceCodeImageInfo = new FileItemsDto();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="linkKey"></param>
        /// <param name="linkTable"></param>
        /// <returns></returns>
        private async Task FileRead(string linkKey, string linkTable = "devicecode")
        {
            try
            {
                IsBusy = true;
                var header = CommAction.GetHeader();
                UploadFileReadRequestDto request = new UploadFileReadRequestDto
                {
                    LinkKey = linkKey,
                    LinkTable = linkTable
                };

                var response = await _uploadClient.UploadFileReadAsync(_mapper.Map<UploadFileReadRequest>(request), header);
                if (response.Code)
                {
                    UploadFileItemDto item = _mapper.Map<UploadFileItemDto>(response.Item);
                    if (item != null)
                    {
                        DeviceCodeImageInfo = item.Files[0];
                        DeviceCodeImage = CommAction.ByteArrayToBitmap(DeviceCodeImageInfo.FileBytes);
                    }
                }
                ImageShow = true;
            }
            catch (Exception ex)
            {
                dialog.Error("提示", ex.Message.ToString());
            }
            finally
            {
                IsBusy = false;
            }
        }
        /// <summary>
        /// 新建命令
        /// </summary>
        [RelayCommand]
        private async Task DeviceCommandAdd()
        {
            try
            {
                DeviceCommandSelected = new DeviceCommandDto
                {
                    DeviceId = _selectedTreeNode.Id,
                };
                await Task.Delay(100);
                SetItemSelected();
            }
            catch(Exception ex)
            {

            }
        }
        /// <summary>
        /// 命令保存
        /// </summary>
        [RelayCommand]
        private async Task DeviceCommandSave()
        {
            try
            {
                DeviceCommandSelected.HardwareCP = HardwareCPSelected?.Id ?? string.Empty;
                DeviceCommandSelected.ApplicationCP = ApplicationCPSelected?.Id ?? string.Empty;

                if (string.IsNullOrEmpty(DeviceCommandSelected.CommandName))
                {
                    dialog.Error("提示", "命令名称不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(DeviceCommandSelected.CommandCode))
                {
                    dialog.Error("提示", "命令代码不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(DeviceCommandSelected.HardwareCP))
                {
                    dialog.Error("提示", "硬件通信协议不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(DeviceCommandSelected.ApplicationCP))
                {
                    dialog.Error("提示", "应用层通信协议不能为空");
                    return;
                }
                var request = new DeviceCommandSaveRequestDto
                {
                    DeviceCommand = DeviceCommandSelected
                };
                var data = await _client.DeviceCommandSaveAsync(_mapper.Map<DeviceCommandSaveRequest>(request), CommAction.GetHeader());
                if (data.Code)
                {
                    dialog.Success("提示", "命令成功");
                    if (string.IsNullOrEmpty(DeviceCommandSelected.CommandId))
                    {
                        DeviceCommandSelected = _mapper.Map<DeviceCommandDto>(data.DeviceCommand);
                        CommandListLoad(DeviceCommandSelected.DeviceId);
                    }
                }
                else
                {
                    dialog.Error("提示", data.Message);
                    return;
                }
            }
            catch (Exception ex)
            {
                dialog.Error("保存异常", ex.Message);
            }
        }
        /// <summary>
        /// 指令执行测试
        /// </summary>
        [RelayCommand]
        private void CommandExec()
        {
            if (ApplicationCPSelected.Id == "1223001" || ApplicationCPSelected.Id == "1223002")//Modbus rtu/tcp
            {
                TempDeviceCommandObj = new DeviceCommandItemDto
                {
                    IsTcp = ApplicationCPSelected.Id == "1223002" ? true : false,
                    SlaveId = DeviceCommandSelected.DeviceAddr,
                    StartAddress = DeviceCommandSelected.StartAddr,
                    NumRegisters = DeviceCommandSelected.RegCount,
                    Com = "COM1"
                };
                CommandPanelOpen = true;
            }
            else
            {
                dialog.Error("提示", "当前仅支持Modbus协议测试");
            }
        }
        /// <summary>
        /// 执行
        /// </summary>
        [RelayCommand]
        private async Task CommandExecTest()
        {
            if (TempDeviceCommandObj.IsTcp&&string.IsNullOrEmpty(TempDeviceCommandObj.Ip))
            {
                dialog.Error("提示", "IP地址不能为空");
                return;
            }
            if (TempDeviceCommandObj.IsTcp && string.IsNullOrEmpty(TempDeviceCommandObj.Port))
            {
                dialog.Error("提示", "端口号不能为空");
                return;
            }
            if (!TempDeviceCommandObj.IsTcp && string.IsNullOrEmpty(TempDeviceCommandObj.Com))
            {
                dialog.Error("提示", "串口号不能为空");
                return;
            }
            if (string.IsNullOrEmpty(TempDeviceCommandObj.SlaveId))
            {
                dialog.Error("提示", "从站地址不能为空");
                return;
            }
            if (string.IsNullOrEmpty(TempDeviceCommandObj.StartAddress))
            {
                dialog.Error("提示", "起始地址不能为空");
                return;
            }
            if (string.IsNullOrEmpty(TempDeviceCommandObj.NumRegisters))
            {
                dialog.Error("提示", "寄存器数量不能为空");
                return;
            }
            StringBuilder resultData = new StringBuilder();
            if (TempDeviceCommandObj.IsTcp)
            {
                using (var modbus = new ModbusHelper(TempDeviceCommandObj.Ip,Convert.ToInt32(TempDeviceCommandObj.Port)))
                {
                    var values = await modbus.ReadHoldingRegistersAsync(slaveId: Convert.ToByte(TempDeviceCommandObj.SlaveId), startAddress: Convert.ToUInt16(TempDeviceCommandObj.StartAddress, 16), numRegisters: Convert.ToUInt16(TempDeviceCommandObj.NumRegisters, 16));
                    resultData.AppendLine($"DATA：{string.Join(",", values)}");
                    resultData.AppendLine($"HEX：{string.Join(",", values.Select(v => v.ToString("X4")))}");
                    TempDeviceCommandObj.Result = resultData.ToString();
                }
            }
            else
            {
                using (var modbus = new ModbusHelper(TempDeviceCommandObj.Com, ModbusTransportType.SerialRtu))
                {
                    ushort[] values =await modbus.ReadHoldingRegistersAsync(slaveId: Convert.ToByte(TempDeviceCommandObj.SlaveId), startAddress: Convert.ToUInt16(TempDeviceCommandObj.StartAddress, 16), numRegisters: Convert.ToUInt16(TempDeviceCommandObj.NumRegisters, 16));
                    resultData.AppendLine($"DATA：{string.Join(",", values)}");
                    resultData.AppendLine($"HEX：{string.Join(",", values.Select(v => v.ToString("X4")))}");
                    TempDeviceCommandObj.Result = resultData.ToString();
                }
                
            }
            Modbuscommand = ModbusHelper.GenerateModbusCommand(Convert.ToByte(TempDeviceCommandObj.SlaveId), Convert.ToByte(DeviceCommandSelected.FunctionCode), Convert.ToUInt16(TempDeviceCommandObj.StartAddress, 16), Convert.ToUInt16(TempDeviceCommandObj.NumRegisters, 16));
        }
        /// <summary>
        /// 
        /// </summary>
        [ObservableProperty] private bool _commandPanelOpen = false;
        [ObservableProperty] private DeviceCommandItemDto _tempDeviceCommandObj = new DeviceCommandItemDto();
        /// <summary>
        /// 抽屉关闭
        /// </summary>
        [RelayCommand]
        private void DeviceCommandClose()
        {
            CommandPanelOpen = false;
            TempDeviceCommandObj = new DeviceCommandItemDto();
        }
    }
}
