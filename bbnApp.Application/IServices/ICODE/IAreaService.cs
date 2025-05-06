using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using Newtonsoft.Json.Linq;

namespace bbnApp.Application.IServices.ICODE
{
    /// <summary>
    /// 行政区划代码服务接口
    /// </summary>
    public interface IAreaService
    {
        /// <summary>
        /// 行政区划代码初始化
        /// </summary>
        Task AreaInit();
        /// <summary>
        /// 行政区划代码更新
        /// </summary>
        Task AreaUpdate();
        /// <summary>
        /// 行政区划数据集读取-list
        /// </summary>
        /// <param name="AreaId">可以指定地区获取</param>
        /// <returns></returns>
        Task<List<AreaListNodeDto>> AreaListLoad(string AreaId);
        /// <summary>
        /// 行政区划数据集读取-tree
        /// </summary>
        /// <param name="AreaId">可以指定地区获取</param>
        /// <returns></returns>
        Task<List<AreaTreeNodeDto>> AreaTreeLoad(string AreaId);
        /// <summary>
        /// 查询数据库获取行政区划数据
        /// </summary>
        /// <param name="AreaId"></param>
        /// <param name="AreaName"></param>
        /// <param name="AreaLeve"></param>
        /// <returns></returns>
        Task<(bool, string, IEnumerable<AreaItemDto>?, int)> AreaSearch(UserModel user,string AreaId, string AreaName, int AreaLeve=0,int PageIndex=1,int PageSize=10);
        /// <summary>
        /// 行政区划数据提交
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        Task<(bool, string, AreaPostDataDto?)> AreaSave(AreaPostDataDto Data, UserModel user);
        /// <summary>
        /// 地区删除
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        Task<(bool, string)> AreaDelete(AreaDeleteRequestDto Data, UserModel user);
        /// <summary>
        /// 地区锁定
        /// </summary>
        /// <param name="Data"></param>
        /// <returns></returns>
        Task<(bool,string)> AreaLock(AreaLockRequestDto Data, UserModel user);
    }
}
