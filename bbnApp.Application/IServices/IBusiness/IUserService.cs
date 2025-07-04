using bbnApp.Common.Models;
using bbnApp.DTOs.BusinessDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.IServices.IBusiness
{
    public interface IUserService
    {
        /// <summary>
        /// 获取用户树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<UserTreeItemDto>)> UserInformationTree(UserInformationTreeRequestDto request, UserModel user);
        /// <summary>
        /// 获取用户清单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<UserInformationDto>)> UserInformationList(UserInformationListRequestDto request, UserModel user);
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name=""></param>
        /// <returns></returns>
        Task<(bool, string, UserInformationDto, List<UserContactDto>, List<UserAabInformationDto>)> UserInformationLoad(UserInformationLoadRequestDto request, UserModel user);
        /// <summary>
        /// 用户信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, UserInformationDto, List<UserContactDto>, List<UserAabInformationDto>)> UserInformationSave(UserInformationSaveRequestDto request, UserModel user);
        /// <summary>
        /// 用户信息状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, UserInformationDto, List<UserContactDto>, List<UserAabInformationDto>)> UserInformationState(UserInformationStateRequestDto request, UserModel user);
    }
}
