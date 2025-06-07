using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.IServices.ICODE
{
    public interface IEmployeeService
    {
        /// <summary>
        /// 员工分页查询
        /// </summary>
        /// <param name="reqeust"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, IEnumerable<EmployeeItemDto>?, int)> EmployeeSearch(EmployeeSearchRequestDto reqeust, UserModel user);
        /// <summary>
        /// 员工清单查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<EmployeeItemDto>)> EmployeeListLoad(EmployeeItemsRequestDto request, UserModel user);
        /// <summary>
        /// 员工树查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<EmployeeTreeItemDto>)> EmployeeTreeLoad(EmployeeTreeRequestDto request, UserModel user);
        /// <summary>
        /// 员工信息加载
        /// </summary>
        /// <param name="EmployeeId"></param>
        /// <param name="CompanyId"></param>
        /// <param name="CompanyName"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, EmployeeItemDto)> EmployeeInfoLoad(string EmployeeId, string CompanyId, string CompanyName, UserModel user);
        /// <summary>
        /// 员工信息提交
        /// </summary>
        /// <param name="item"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, EmployeeItemDto)> EmployeePost(EmployeeItemDto item, UserModel user);
        /// <summary>
        /// 员工状态变更
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="EmployeeId"></param>
        /// <param name="Reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, EmployeeItemDto)> EmployeeState(string Type, string EmployeeId, string Reason, UserModel user);
    }
}
