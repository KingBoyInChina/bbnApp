using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.IServices.ICODE
{
    public interface IDepartMentService
    {
        /// <summary>
        /// 部门清单读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<DepartMentInfoDto>)> GetDepartMentItems(DepartMentSearchRequestDto request, UserModel user);
        /// <summary>
        /// 部门树读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<DepartMentTreeItemDto>)> GetDepartMentTree(DepartMentTreeRequestDto request, UserModel user);
        /// <summary>
        /// 部门信息读取
        /// </summary>
        /// <param name="DepartMentId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DepartMentInfoDto)> GetDepartMentInfo(string DepartMentId, string CompanyId, UserModel user);
        /// <summary>
        /// 部门信息保存
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DepartMentInfoDto)> SaveDepartMent(DepartMentInfoDto model, UserModel user);
        /// <summary>
        /// 部门状态变更
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="DepartMentId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="Reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DepartMentInfoDto)> StateDepartMent(string Type, string DepartMentId, string CompanyId, string Reason, UserModel user);
    }
}
