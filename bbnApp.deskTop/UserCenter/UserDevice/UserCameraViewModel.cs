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
using System.Threading.Tasks;

namespace bbnApp.deskTop.UserCenter.UserDevice
{
    partial class UserCameraViewModel : ViewModelBase
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
        private UserCameraDto _tempData;

        /// <summary>
        /// 网关设备对象
        /// </summary>
        [ObservableProperty] private UserCameraDto _camera = new UserCameraDto();
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

        public UserCameraViewModel()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="Node"></param>

        public void ViewModelInit(IDialog dialog, Action<bool, string, object> CallBack, UserCameraDto InitValue, UserDeviceGrpc.UserDeviceGrpcClient client, IMapper _mapper, List<DeviceCodeItemDto> devices)
        {
            this.dialog = dialog;
            _SubmitCallBack = CallBack;
            _tempData = InitValue;
            _client = client;
            this._mapper = _mapper;
            //初始化字典
            Devices = [.. devices.Where(x => x.DeviceType == "1101003").ToList()];
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
            Camera = _tempData;
            SelectedInstaller = CommAction.SetSelectedItem(Installers, Camera.InstallerId);
            SelectedDevice = Devices?.FirstOrDefault(x => x.DeviceId == Camera.DeviceId) ?? new DeviceCodeItemDto();
            if (Camera.InstallTime != DateTime.MinValue)
            {
                InstallTime = new DateTimeOffset(Convert.ToDateTime(Camera.InstallTime));
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
        private async Task CameraSubmit()
        {
            try
            {
                Camera.InstallerId = SelectedInstaller?.Id ?? string.Empty;
                Camera.Installer = SelectedInstaller?.Name ?? string.Empty;
                Camera.DeviceId = SelectedDevice.DeviceId;
                Camera.InstallTime = Convert.ToDateTime(InstallTime.ToString("yyyy-MM-dd"));
                if (string.IsNullOrEmpty(Camera.DeviceId))
                {
                    dialog.Error("提示", "设备分类名称不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(Camera.CameraIp))
                {
                    dialog.Error("提示", "摄像头IP不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(Camera.CameraChannel))
                {
                    dialog.Error("提示", "摄像头通道不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(Camera.CameraAdmin))
                {
                    dialog.Error("提示", "摄像头用户名不能为空");
                    return;
                }
                if (string.IsNullOrEmpty(Camera.CameraPassword))
                {
                    dialog.Error("提示", "摄像头密码不能为空");
                    return;
                }
                else if (string.IsNullOrEmpty(Camera.InstallerId))
                {
                    dialog.Error("提示", "安装人不能为空");
                    return;
                }
                else
                {
                    IsBusy = true;
                    UserCameraSaveRequestDto request = new UserCameraSaveRequestDto
                    {
                        Camera = Camera,
                    };

                    var header = CommAction.GetHeader();

                    var response = await _client.UserCameraSaveAsync(_mapper.Map<UserCameraSaveRequest>(request), header);
                    if (response.Code)
                    {
                        _SubmitCallBack(response.Code, response.Message, response.Camera);
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
            bool b = await dialog.Confirm("关闭提示", "确定要关闭当前摄像头维护页面吗？", "关闭", "取消");
            if (b)
            {
                _SubmitCallBack(false, "关闭", null);
            }
        }
    }
}
