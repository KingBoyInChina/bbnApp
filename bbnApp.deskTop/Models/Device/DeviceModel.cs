using bbnApp.DTOs.BusinessDto;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
}
