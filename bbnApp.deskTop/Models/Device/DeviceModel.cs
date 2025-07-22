using bbnApp.deskTop.Models.Role;
using bbnApp.DTOs.BusinessDto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.deskTop.Models.Device
{
    /// <summary>
    /// 用户网关设备对象
    /// </summary>
    public class UserGetWayCus: UserGetWayDto
    {
        /// <summary>
        /// 网关关联设备
        /// </summary>
        public new ObservableCollection<UserGetWayDeviceDto> Devices { get; set; } = new ObservableCollection<UserGetWayDeviceDto>();
    }
    /// <summary>
    /// 边缘盒子
    /// </summary>
    public class UserBoxCus: UserBoxDto
    {
        /// <summary>
        /// 边缘盒子关联摄像头
        /// </summary>
        public new ObservableCollection<UserCameraDto> UserCameras { get; set; } = new ObservableCollection<UserCameraDto>();
    }

    /// <summary>
    /// 
    /// </summary>
    public class DeviceCommandItemDto : INotifyPropertyChanged
    {
        private string _result = string.Empty;

        /// <summary>
        /// 是否时TCP
        /// </summary>
        public bool IsTcp { get; set; } = false;
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; } = string.Empty;
        /// <summary>
        /// 端口号
        /// </summary>
        public string Port { get; set; } = string.Empty;
        /// <summary>
        /// 串口号
        /// </summary>
        public string Com { get; set; } = string.Empty;
        /// <summary>
        /// 从站地址
        /// </summary>
        public string SlaveId { get; set; } = string.Empty;
        /// <summary>
        /// 起始地址
        /// </summary>
        public string StartAddress { get; set; } = string.Empty;
        /// <summary>
        /// 寄存器数量
        /// </summary>
        public string NumRegisters { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); }
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
