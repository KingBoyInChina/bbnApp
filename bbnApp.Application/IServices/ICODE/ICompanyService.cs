using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.Application.IServices.ICODE
{
    /// <summary>
    /// 机构服务接口
    /// </summary>
    public interface ICompanyService
    {
        /// <summary>
        /// 机构信息初始化
        /// </summary>
        /// <returns></returns>
        Task CompanyInit();
        /// <summary>
        /// 获取公司Combobox数据集
        /// </summary>
        /// <returns></returns>
        Task<List<CompanyItemDto>> GetCompanyItems(CompanyRequestDto CompanyRequest);
        /// <summary>
        /// 获取公司tree数据
        /// </summary>
        /// <param name="CompanyRequest"></param>
        /// <returns></returns>
        Task<List<CompanyTreeItemDto>> GetCompanyTree(CompanyTreeRequestDto CompanyRequest, UserModel user);
        /// <summary>
        /// 读取公司信息
        /// </summary>
        /// <param name="CompanyId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, CompanyInfoDto)> GetCompanyInfo(string CompanyId, UserModel user);
        /// <summary>
        /// 公司信息提交
        /// </summary>
        /// <param name="model"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, CompanyInfoDto)> SaveCompanyInfo(CompanyInfoDto model, UserModel user);
        /// <summary>
        /// 状态变更公司信息
        /// </summary>
        /// <param name="Type"></param>
        /// <param name="CompanyId"></param>
        /// <param name="Reason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, CompanyInfoDto)> StateCompanyInfo(string Type, string CompanyId, string Reason, UserModel user);
    }
}
