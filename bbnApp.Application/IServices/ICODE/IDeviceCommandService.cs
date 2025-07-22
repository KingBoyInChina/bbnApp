using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.IServices.ICODE
{
    public interface IDeviceCommandService
    {
        /// <summary>
        /// 设备命令清单查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<DeviceCommandDto>)> GetDeviceCommandList(DeviceCommandListRequestDto request, UserModel user);
        /// <summary>
        /// 设备命令保存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DeviceCommandDto)> DeviceCommandSave(DeviceCommandSaveRequestDto request, UserModel user);
        /// <summary>
        /// 状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DeviceCommandDto)> DeviceCommandState(DeviceCommandStateRequestDto request, UserModel user);
    }
}
