using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.BusinessDto
{
    /// <summary>
    /// 用户设备清单对象
    /// </summary>
    public class UserDeviceListItemDto { 
        /// <summary>
        /// 用户设备ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 设备/网关名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 设备类型ID
        /// </summary>
        public string DeviceId { get; set; }
        /// <summary>
        /// 设备类型代码
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 从站地址或IP
        /// </summary>
        public string SlaveId { get; set; }
        /// <summary>
        /// 从站名称
        /// </summary>
        public string SlaveName { get; set; }
        /// <summary>
        /// 值代码和顺序
        /// </summary>
        public string ValueCode { get; set; }
    }
    

    /// <summary>
    /// 树形对象
    /// </summary>
    public class UserDeviceTreeItemDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string? PId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public string? Tag { get; set; }
        /// <summary>
        /// 叶子节点
        /// </summary>
        public bool? IsLeaf { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        public bool? IsLock { get; set; }
        /// <summary>
        /// Children
        /// </summary>
        public List<UserDeviceTreeItemDto>? SubItems { get; set; }
    }
    /// <summary>
    /// 用户网关设备对象
    /// </summary>
    public class UserGetWayDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;

        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 网关ID
        /// </summary>
        public string GetWayId { get; set; } = string.Empty;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 网关设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// 网关设备名称
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// 网关名称
        /// </summary>
        public string GetWayName { get; set; } = string.Empty;

        /// <summary>
        /// 网关编号
        /// </summary>
        public string GetWayNumber { get; set; } = string.Empty;

        /// <summary>
        /// 安装位置
        /// </summary>
        public string GetWayLocation { get; set; } = string.Empty;

        /// <summary>
        /// 网络连接方式
        /// </summary>
        public string WlanType { get; set; } = string.Empty;

        /// <summary>
        /// 安装时间
        /// </summary>
        public DateTime InstallTime { get; set; }

        /// <summary>
        /// 安装人
        /// </summary>
        public string Installer { get; set; } = string.Empty;

        /// <summary>
        /// 安装人ID
        /// </summary>
        public string InstallerId { get; set; } = string.Empty;

        /// <summary>
        /// 维保到期时间
        /// </summary>
        public DateTime Warranty { get; set; }

        /// <summary>
        /// 停用
        /// </summary>
        public byte IsLock { get; set; } = 0;

        /// <summary>
        /// 停用时间
        /// </summary>
        public string? LockTime { get; set; } = string.Empty;

        /// <summary>
        /// 停用原因
        /// </summary>
        public string? LockReason { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;
        /// <summary>
        /// 网关关联设备
        /// </summary>
        public List<UserGetWayDeviceDto> Devices = new List<UserGetWayDeviceDto>();
    }
    /// <summary>
    /// 边缘盒子
    /// </summary>
    public class UserBoxDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;

        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 盒子ID
        /// </summary>
        public string BoxId { get; set; } = string.Empty;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// 盒子名称
        /// </summary>
        public string BoxName { get; set; } = string.Empty;

        /// <summary>
        /// 盒子编号
        /// </summary>
        public string BoxNumber { get; set; } = string.Empty;

        /// <summary>
        /// 安装位置
        /// </summary>
        public string BoxLocation { get; set; } = string.Empty;

        /// <summary>
        /// 网络连接方式
        /// </summary>
        public string WlanType { get; set; } = string.Empty;

        /// <summary>
        /// 带宽
        /// </summary>
        public int TapeWidth { get; set; }

        /// <summary>
        /// 安装时间
        /// </summary>
        public DateTime InstallTime { get; set; }

        /// <summary>
        /// 安装人
        /// </summary>
        public string Installer { get; set; } = string.Empty;

        /// <summary>
        /// 安装人ID
        /// </summary>
        public string InstallerId { get; set; } = string.Empty;

        /// <summary>
        /// 维保到期时间
        /// </summary>
        public DateTime Warranty { get; set; }

        /// <summary>
        /// 停用
        /// </summary>
        public byte IsLock { get; set; } = 0;

        /// <summary>
        /// 停用时间
        /// </summary>
        public string? LockTime { get; set; } = string.Empty;

        /// <summary>
        /// 停用原因
        /// </summary>
        public string? LockReason { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;
        /// <summary>
        /// 边缘盒子关联摄像头
        /// </summary>
        public List<UserCameraDto> UserCameras { get; set; } = new List<UserCameraDto>();
    }
    /// <summary>
    /// 用户网关关联设备
    /// </summary>
    public class UserGetWayDeviceDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;

        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 网关管理的设备ID
        /// </summary>
        public string GetWayDeviceId { get; set; } = string.Empty;

        /// <summary>
        /// 网关ID
        /// </summary>
        public string GetWayId { get; set; } = string.Empty;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNumber { get; set; } = string.Empty;
        /// <summary>
        /// 从站地址
        /// </summary>
        public string SlaveId { get; set; } = string.Empty;
        /// <summary>
        /// 从站名称
        /// </summary>
        public string SlaveName { get; set; } = string.Empty;

        /// <summary>
        /// 安装时间
        /// </summary>
        public DateTime InstallTime { get; set; }

        /// <summary>
        /// 安装人
        /// </summary>
        public string Installer { get; set; } = string.Empty;

        /// <summary>
        /// 安装人ID
        /// </summary>
        public string InstallerId { get; set; } = string.Empty;

        /// <summary>
        /// 维保到期时间
        /// </summary>
        public DateTime Warranty { get; set; }

        /// <summary>
        /// 停用
        /// </summary>
        public byte IsLock { get; set; } = 0;

        /// <summary>
        /// 停用时间
        /// </summary>
        public string? LockTime { get; set; } = string.Empty;

        /// <summary>
        /// 停用原因
        /// </summary>
        public string? LockReason { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;
    }
    /// <summary>
    /// 边缘盒子对应的摄像头
    /// </summary>
    public class UserCameraDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;

        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 摄像头ID
        /// </summary>
        public string CameraId { get; set; } = string.Empty;

        /// <summary>
        /// 盒子ID
        /// </summary>
        public string BoxId { get; set; } = string.Empty;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// 摄像头编号
        /// </summary>
        public string CameraNumber { get; set; } = string.Empty;

        /// <summary>
        /// 摄像头IP
        /// </summary>
        public string CameraIp { get; set; } = string.Empty;

        /// <summary>
        /// 摄像头通道号
        /// </summary>
        public string CameraChannel { get; set; } = string.Empty;

        /// <summary>
        /// 摄像头自定义名称
        /// </summary>
        public string CameraName { get; set; } = string.Empty;

        /// <summary>
        /// 摄像头登录名
        /// </summary>
        public string? CameraAdmin { get; set; } = string.Empty;

        /// <summary>
        /// 摄像头登录密码
        /// </summary>
        public string? CameraPassword { get; set; } = string.Empty;

        /// <summary>
        /// 安装时间
        /// </summary>
        public DateTime InstallTime { get; set; }

        /// <summary>
        /// 安装人
        /// </summary>
        public string Installer { get; set; } = string.Empty;

        /// <summary>
        /// 安装人ID
        /// </summary>
        public string InstallerId { get; set; } = string.Empty;

        /// <summary>
        /// 维保到期时间
        /// </summary>
        public DateTime Warranty { get; set; }

        /// <summary>
        /// 停用
        /// </summary>
        public byte IsLock { get; set; } = 0;

        /// <summary>
        /// 停用时间
        /// </summary>
        public string? LockTime { get; set; } = string.Empty;

        /// <summary>
        /// 停用原因
        /// </summary>
        public string? LockReason { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;
    }

    /// <summary>
    /// 用户设备信息树查询请求
    /// </summary>
    public class UserDeviceTreeRequestDto
    {
        /// <summary>
        /// 所在地区
        /// </summary>
        public string AreaId { get; set; } = string.Empty;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
    }
    /// <summary>
    /// 用户设备信息树查询响应
    /// </summary>
    public class UserDeviceTreeResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 响应数据集
        /// </summary>
        public List<UserTreeItemDto> Items = new List<UserTreeItemDto>();
    }
    /// <summary>
    /// 用户设备树查询请求
    /// </summary>
    public class UserDeviceListRequestDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 用户设备树查询响应
    /// </summary>
    public class UserDeviceListResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 用户信息
        /// </summary>
        public UserInformationDto User = new UserInformationDto();
        /// <summary>
        /// 网关-响应数据集
        /// </summary>
        public List<UserGetWayDto> GetWays = new List<UserGetWayDto>();
        /// <summary>
        /// 边缘盒子-响应数据集
        /// </summary>
        public List<UserBoxDto> Boxs = new List<UserBoxDto>();
    }
    /// <summary>
    /// 用户网关提交请求
    /// </summary>
    public class UserGetWaySaveRequestDto
    {
        /// <summary>
        /// 网关
        /// </summary>
        public UserGetWayDto? GetWay { get; set; } = new UserGetWayDto();
    }
    /// <summary>
    /// 用户网关提交响应
    /// </summary>
    public class UserGetWaySaveResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 网关
        /// </summary>
        public UserGetWayDto GetWay { get; set; } = new UserGetWayDto();
    }
    /// <summary>
    /// 用户网关设备提交请求
    /// </summary>
    public class UserGetWayDeviceSaveRequestDto
    {
        /// <summary>
        /// 网关
        /// </summary>
        public UserGetWayDeviceDto Device { get; set; } = new UserGetWayDeviceDto();
    }
    /// <summary>
    /// 用户网关设备提交响应
    /// </summary>
    public class UserGetWayDeviceSaveResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 网关
        /// </summary>
        public UserGetWayDeviceDto Device { get; set; } = new UserGetWayDeviceDto();
    }
    /// <summary>
    /// 用户盒子提交请求
    /// </summary>
    public class UserBoxSaveRequestDto
    {
        /// <summary>
        /// 边缘盒子
        /// </summary>
        public UserBoxDto Box = new UserBoxDto();
    }
    /// <summary>
    /// 用户黑子提交响应
    /// </summary>
    public class UserBoxSaveResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 边缘盒子
        /// </summary>

        /// <summary>
        /// 边缘盒子
        /// </summary>
        public UserBoxDto Box = new UserBoxDto();
    }
    /// <summary>
    /// 用户摄像头提交请求
    /// </summary>
    public class UserCameraSaveRequestDto
    {
        /// <summary>
        /// 边缘盒子
        /// </summary>
        public UserCameraDto Camera = new UserCameraDto();
    }
    /// <summary>
    /// 用户黑子提交响应
    /// </summary>
    public class UserCameraSaveResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 边缘盒子
        /// </summary>

        /// <summary>
        /// 边缘盒子
        /// </summary>
        public UserCameraDto Camera = new UserCameraDto();
    }
    /// <summary>
    /// 用户设备状态变更请求
    /// </summary>
    public class UserDeviceStateRequestDto
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// 网关ID
        /// </summary>
        public string GetWayId { get; set; } = string.Empty;
        /// <summary>
        /// 网关设备ID
        /// </summary>
        public string GetWayDeviceId { get; set;} = string.Empty;
        /// <summary>
        /// 盒子ID
        /// </summary>
        public string BoxId { get; set; } = string.Empty;
        /// <summary>
        /// 摄像头ID
        /// </summary>
        public string CameraId { get; set; } = string.Empty;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
    /// <summary>
    /// 用户设备状态变更响应
    /// </summary>
    public class UserDeviceStateResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 网关
        /// </summary>
        public List<UserGetWayDto>? GetWay { get; set; } = new List<UserGetWayDto>();
        /// <summary>
        /// 边缘盒子
        /// </summary>
        public List<UserBoxDto>? Box = new List<UserBoxDto>();
    }
    /// <summary>
    /// 网关设备数据集读取
    /// </summary>
    public class UserDeviceDataRequestDto {
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get;set; } = string.Empty;
    }
    /// <summary>
    /// 网关设备数据集响应
    /// </summary>
    public class UserDeviceDataResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 网关
        /// </summary>
        public List<UserDeviceListItemDto>? Devices { get; set; } = new List<UserDeviceListItemDto>();
    }
}
