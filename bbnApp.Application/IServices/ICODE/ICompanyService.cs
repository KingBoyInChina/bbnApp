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
        Task<List<TreeItem>> GetCompanyTree(CompanyRequestDto CompanyRequest);
    }
}
