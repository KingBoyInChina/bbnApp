using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.IServices.ICODE
{
    public interface IRoleService
    {
        /// <summary>
        /// 角色清单读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<RoleItemDto>)> RoleListLoad(RoleListRequestDto request, UserModel user);
        /// <summary>
        /// 角色应用读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<RoleAppsDto>)> RoleAppListLoad(RoleAppListRequestDto request, UserModel user);
        /// <summary>
        /// 角色信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, RoleItemDto, List<RoleAppsDto>)> RoleInfo(RoleInfoRequestDto request, UserModel user);
        /// <summary>
        /// 角色信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, RoleItemDto)> RolePost(RoleSaveRequestDto request, UserModel user);
        /// <summary>
        /// 橘色状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, RoleItemDto)> RoleState(RoleStateRequestDto request, UserModel user);
    }
}
