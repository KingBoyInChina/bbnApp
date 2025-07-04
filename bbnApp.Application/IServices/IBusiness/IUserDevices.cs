using bbnApp.Common.Models;
using bbnApp.DTOs.BusinessDto;

namespace bbnApp.Application.IServices.IBusiness
{
    public interface IUserDevices
    {
        /// <summary>
        /// 获取用户树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<UserDeviceTreeItemDto>)> UserDeviceTree(UserDeviceTreeRequestDto request, UserModel user);
        /// <summary>
        /// 指定用户的网关查询,一般不会有很多网关，就不用分页了
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, UserInformationDto, List<UserGetWayDto>, List<UserBoxDto>)> UserDeviceList(UserDeviceListRequestDto request, UserModel user);
        /// <summary>
        /// 网关保存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, UserGetWayDto)> UserGetWaySave(UserGetWaySaveRequestDto request, UserModel user);
        /// <summary>
        /// 网关设备-提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, UserGetWayDeviceDto)> UserGetWayDeviceSave(UserGetWayDeviceSaveRequestDto request, UserModel user);
        /// <summary>
        /// 边缘盒子提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, UserBoxDto)> UserBoxSave(UserBoxSaveRequestDto request, UserModel user);
        /// <summary>
        /// 摄像头提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, UserCameraDto)> UserCameraSave(UserCameraSaveRequestDto request, UserModel user);
        /// <summary>
        /// 用户设备状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<UserGetWayDto>, List<UserBoxDto>)> UserDeviceState(UserDeviceStateRequestDto request, UserModel user);
    }
}
