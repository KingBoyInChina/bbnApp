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
    partial class UserGetWayViewModel: ViewModelBase
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
        private UserGetWayDto _tempData;

        /// <summary>
        /// 网关对象
        /// </summary>
        [ObservableProperty] private UserGetWayDto _getWay = new UserGetWayDto();
        /// <summary>
        /// 标准网关设备字典
        /// </summary>
        [ObservableProperty] private ObservableCollection<DeviceCodeItemDto> _devices= new ObservableCollection<DeviceCodeItemDto>();
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
        public UserGetWayViewModel()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="areaSubmitCallBack"></param>
        /// <param name="Node"></param>

        public void ViewModelInit(IDialog dialog, Action<bool, string, object> CallBack, UserGetWayDto InitValue, UserDeviceGrpc.UserDeviceGrpcClient client, IMapper _mapper, List<DeviceCodeItemDto> devices)
        {
            this.dialog = dialog;
            _SubmitCallBack = CallBack;
            _tempData = InitValue;
            _client = client;
            this._mapper = _mapper;
            //初始化字典
            WlanType = new ObservableCollection<ComboboxItem>(CommAction.GetDicItems("1221"));
            Devices =[.. devices.Where(x=>x.DeviceType== "1101001").ToList()];
            //初始化安装人
            var installer = UserContext.CompanyUsers.Select(x=>new ComboboxItem { 
                Id=x.EmployeeId,
                Name=x.EmployeeName,
                Tag="worker"
            }).ToList();
            Installers = [..installer];
        }
        /// <summary>
        /// 
        /// </summary>
        public void ViewModelInit()
        {
            GetWay = _tempData;
            SelectedDevice = Devices?.FirstOrDefault(x=>x.DeviceId== GetWay.DeviceId)??new DeviceCodeItemDto();
            SelectedWlanType = CommAction.SetSelectedItem(WlanType, GetWay.WlanType);
            SelectedInstaller = CommAction.SetSelectedItem(Installers, GetWay.InstallerId);
            if (GetWay.InstallTime!=DateTime.MinValue)
            {
                InstallTime = new DateTimeOffset(Convert.ToDateTime(GetWay.InstallTime));
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
        private async Task GetWaySubmit()
        {
            try
            {
                GetWay.InstallerId = SelectedInstaller?.Id ?? string.Empty;
                GetWay.Installer = SelectedInstaller?.Name ?? string.Empty;
                GetWay.DeviceId = SelectedDevice.DeviceId;
                GetWay.WlanType = SelectedWlanType?.Id ?? string.Empty;
                GetWay.InstallTime =Convert.ToDateTime(InstallTime.ToString("yyyy-MM-dd"));
                if (string.IsNullOrEmpty(GetWay.GetWayName))
                {
                    dialog.Error("提示", "网关自定义名称不能为空");
                    return;
                }
                else if (string.IsNullOrEmpty(GetWay.InstallerId))
                {
                    dialog.Error("提示", "安装人不能为空");
                    return;
                }
                else
                {
                    IsBusy = true;
                    UserGetWaySaveRequestDto request = new UserGetWaySaveRequestDto
                    {
                        GetWay = GetWay,
                    };

                    var header = CommAction.GetHeader();

                    var response = await _client.UserGetWaySaveAsync(_mapper.Map<UserGetWaySaveRequest>(request), header);
                    if (response.Code)
                    {
                        _SubmitCallBack(response.Code, response.Message, response.GetWay);
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
