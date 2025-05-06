using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.Application.IServices.IINIT
{
    /// <summary>
    /// 系统初始化-字典信息初始化
    /// </summary>
    public interface IDictionaryInitialization
    {
        /// <summary>
        /// 所有字典初始化
        /// </summary>
        /// <returns></returns>
        Task DictionaryInit();
        /// <summary>
        /// 操作员权限等信息初始化
        /// 用于后续可能出现较高频次的权限校验操作
        /// </summary>
        /// <returns></returns>
        Task AuthorPermissonInit();
        /// <summary>
        /// 获取顶部菜单
        /// </summary>
        /// <param name="Remarks">身份标识</param>
        /// <returns></returns>
        Task<List<TopMenuItemDto>> GetTopMenu(string Remarks);
    }
}
