using AutoMapper;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.ViewModels;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.UserCenter.UserDevice
{
    partial class UserGetWayDeviceViewModel : ViewModelBase
    {
        /// <summary>
        /// 映射
        /// </summary>
        private IMapper _mapper;
        /// <summary>
        /// 提交回调
        /// </summary>
        private Action<bool, string, object> _SubmitCallBack;
        /// <summary>
        /// 
        /// </summary>
        private IDialog dialog;
        /// <summary>
        /// 状态
        /// </summary>
        [ObservableProperty] private bool _isBusy = false;
        /// <summary>
        /// 
        /// </summary>
        private UserDeviceGrpc.UserDeviceGrpcClient _client;
        /// <summary>
        /// 
        /// </summary>
        private UserGetWayDeviceDto _tempData;

        /// <summary>
        /// 网关设备对象
        /// </summary>
        [ObservableProperty] private UserGetWayDeviceDto _device = new UserGetWayDeviceDto();
        /// <summary>
        /// 标准网关设备字典
        /// </summary>
        [ObservableProperty] private ObservableCollection<DeviceCodeItemDto> _devices = new ObservableCollection<DeviceCodeItemDto>();
        /// <summary>
        /// 选中的设备分类
        /// </summary>
        [ObservableProperty] private DeviceCodeItemDto _selectedDevice = new DeviceCodeItemDto();
        /// <summary>
        /// 安装人
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _installers = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的安装人
        /// </summary>
        [ObservableProperty] private ComboboxItem _selectedInstaller = new ComboboxItem();
        /// <summary>
        /// 安装时间
        /// </summary>
        [ObservableProperty] private DateTimeOffset _installTime = new DateTimeOffset(DateTime.Now);

        public UserGetWayDeviceViewModel()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="Node"></param>

        public void ViewModelInit(IDialog dialog, Action<bool, string, object> CallBack, UserGetWayDeviceDto InitValue, UserDeviceGrpc.UserDeviceGrpcClient client, IMapper _mapper, List<DeviceCodeItemDto> devices)
        {
            this.dialog = dialog;
            _SubmitCallBack = CallBack;
            _tempData = InitValue;
            _client = client;
            this._mapper = _mapper;
            //初始化字典
            Devices = [.. devices.Where(x => x.DeviceType == "1101006").ToList()];
            //初始化安装人
            var installer = UserContext.CompanyUsers.Select(x => new ComboboxItem
            {
                Id = x.EmployeeId,
                Name = x.EmployeeName,
                Tag = "worker"
            }).ToList();
            Installers = [.. installer];
        }
        /// <summary>
        /// 
        /// </summary>
        public void ViewModelInit()
        {
            Device = _tempData;
            SelectedInstaller = CommAction.SetSelectedItem(Installers, Device.InstallerId);
            SelectedDevice = Devices?.FirstOrDefault(x=>x.DeviceId==Device.DeviceId)??new DeviceCodeItemDto();
            if (Device.InstallTime != DateTime.MinValue)
            {
                InstallTime = new DateTimeOffset(Convert.ToDateTime(Device.InstallTime));
            }
            else
            {
                InstallTime = DateTime.Now;
            }
        }
        /// <summary>
        /// 数据提交
        /// </summary>
        [RelayCommand]
        private async Task DeviceSubmit()
        {
            try
            {
                Device.InstallerId = SelectedInstaller?.Id ?? string.Empty;
                Device.Installer = SelectedInstaller?.Name ?? string.Empty;
                Device.DeviceId = SelectedDevice.DeviceId;
                Device.InstallTime = Convert.ToDateTime(InstallTime.ToString("yyyy-MM-dd"));
                if (string.IsNullOrEmpty(Device.DeviceId))
                {
                    dialog.Error("提示", "设备分类名称不能为空");
                    return;
                }
                else if (string.IsNullOrEmpty(Device.InstallerId))
                {
                    dialog.Error("提示", "安装人不能为空");
                    return;
                }
                else
                {
                    IsBusy = true;
                    UserGetWayDeviceSaveRequestDto request = new UserGetWayDeviceSaveRequestDto
                    {
                        Device = Device,
                    };

                    var header = CommAction.GetHeader();

                    var response = await _client.UserGetWayDeviceSaveAsync(_mapper.Map<UserGetWayDeviceSaveRequest>(request), header);
                    if (response.Code)
                    {
                        _SubmitCallBack(response.Code, response.Message, response.Device);
                    }
                    else
                    {
                        dialog.Error("错误提示", $"数据提交错误：{response.Message}");
                    }
                    IsBusy = false;
                }
            }
            catch (Exception ex)
            {
                dialog.Error("异常提示", $"数据提交异常：{ex.Message.ToString()}");
            }
            finally
            {
                IsBusy = false;
            }
        }
        /// <summary>
        /// 关闭
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        private async Task Close()
        {
            bool b = await dialog.Confirm("关闭提示", "确定要关闭当前网关设备维护页面吗？", "关闭", "取消");
            if (b)
            {
                _SubmitCallBack(false, "关闭", null);
            }
        }
    }
}
