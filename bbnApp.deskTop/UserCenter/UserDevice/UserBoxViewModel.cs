using AutoMapper;
using bbnApp.Common.Models;
using bbnApp.deskTop.Common;
using bbnApp.deskTop.ViewModels;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Consul;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.UserCenter.UserDevice
{
    partial class UserBoxViewModel : ViewModelBase
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
        private UserBoxDto _tempData;

        /// <summary>
        /// 盒子对象
        /// </summary>
        [ObservableProperty] private UserBoxDto _box = new UserBoxDto();
        /// <summary>
        /// 标准盒子设备字典
        /// </summary>
        [ObservableProperty] private ObservableCollection<DeviceCodeItemDto> _devices = new ObservableCollection<DeviceCodeItemDto>();
        /// <summary>
        /// 选中的设备分类
        /// </summary>
        [ObservableProperty] private DeviceCodeItemDto _selectedDevice = new DeviceCodeItemDto();
        /// <summary>
        /// 通信方式
        /// </summary>
        [ObservableProperty] private ObservableCollection<ComboboxItem> _wlanType = new ObservableCollection<ComboboxItem>();
        /// <summary>
        /// 选中的设备分类
        /// </summary>
        [ObservableProperty] private ComboboxItem _selectedWlanType = new ComboboxItem();
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

        public UserBoxViewModel()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="Node"></param>

        public void ViewModelInit(IDialog dialog, Action<bool, string, object> CallBack, UserBoxDto InitValue, UserDeviceGrpc.UserDeviceGrpcClient client, IMapper _mapper, List<DeviceCodeItemDto> devices)
        {
            this.dialog = dialog;
            _SubmitCallBack = CallBack;
            _tempData = InitValue;
            _client = client;
            this._mapper = _mapper;
            //初始化字典
            WlanType = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1221"));
            Devices = [.. devices.Where(x => x.DeviceType == "1101005").ToList()];
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
            Box = _tempData;
            SelectedDevice = Devices?.FirstOrDefault(x => x.DeviceId == Box.DeviceId) ?? new DeviceCodeItemDto();
            SelectedWlanType = CommAction.SetSelectedItem(WlanType, Box.WlanType);
            SelectedInstaller = CommAction.SetSelectedItem(Installers, Box.InstallerId);
            if (Box.InstallTime != DateTime.MinValue)
            {
                InstallTime = new DateTimeOffset(Convert.ToDateTime(Box.InstallTime));
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
        private async Task BoxSubmit()
        {
            try
            {
                Box.InstallerId = SelectedInstaller?.Id ?? string.Empty;
                Box.Installer = SelectedInstaller?.Name ?? string.Empty;
                Box.DeviceId = SelectedDevice.DeviceId;
                Box.WlanType = SelectedWlanType?.Id ?? string.Empty;
                Box.InstallTime = Convert.ToDateTime(InstallTime.ToString("yyyy-MM-dd"));
                if (string.IsNullOrEmpty(Box.BoxName))
                {
                    dialog.Error("提示", "盒子自定义名称不能为空");
                    return;
                }
                else if (string.IsNullOrEmpty(Box.InstallerId))
                {
                    dialog.Error("提示", "安装人不能为空");
                    return;
                }
                else
                {
                    IsBusy = true;
                    UserBoxSaveRequestDto request = new UserBoxSaveRequestDto
                    {
                        Box = Box,
                    };

                    var header = CommAction.GetHeader();

                    var response = await _client.UserBoxSaveAsync(_mapper.Map<UserBoxSaveRequest>(request), header);
                    if (response.Code)
                    {
                        _SubmitCallBack(response.Code, response.Message, response.Box);
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
            bool b = await dialog.Confirm("关闭提示", "确定要关闭当前盒子设备维护页面吗？", "关闭", "取消");
            if (b)
            {
                _SubmitCallBack(false, "关闭", null);
            }
        }
    }
}
