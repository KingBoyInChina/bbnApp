using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.Application.IServices.ICODE
{
    public interface IDeviceCodeService
    {
        /// <summary>
        /// 获取设备代码树
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<DeviceCodeTreeNodeDto>)> GetDeviceTree(UserModel user);
        /// <summary>
        /// 获取设备信息
        /// </summary>
        /// <param name="DeviceId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DeviceCodeItemDto, List<DeviceStructItemDto>)> GetDeviceInfo(string DeviceId, UserModel user);
        /// <summary>
        /// 设备信息提交
        /// </summary>
        /// <param name="deviceModel"></param>
        /// <param name="deviceStructlist"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DeviceCodeItemDto, List<DeviceStructItemDto>)> DeviceInfoPost(DeviceCodeItemDto deviceModel, List<DeviceStructItemDto> deviceStructlist, UserModel user);
        /// <summary>
        /// 设备状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="deviceid"></param>
        /// <param name="reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DeviceCodeItemDto, List<DeviceStructItemDto>)> DeviceStateChange(string type, string deviceid, string reason, UserModel user);
        /// <summary>
        /// 设备结构状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="StructId"></param>
        /// <param name="reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<DeviceStructItemDto>)> DeviceStructState(string type, string StructId, string reason, UserModel user);
        /// <summary>
        /// 设备清单查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<DeviceCodeItemDto>)> DeviceSearch(DeviceCodeSearchRequestDto request, UserModel user);
    }
}
