using bbnApp.Common.Models;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.Application.IServices.ICODE
{
    public interface IDataDictionaryService
    {
        /// <summary>
        /// 字典初始化
        /// </summary>
        /// <returns></returns>
        Task DicInit();
        /// <summary>
        /// 字典下载
        /// </summary>
        /// <returns></returns>
        Task<(bool, string, List<DicTreeItemDto>?)> DicLoad();
        /// <summary>
        /// 获取字典分类树
        /// </summary>
        /// <param name="filterKey"></param>
        /// <returns></returns>
        Task<(bool, string, List<DicTreeItemDto>)> DicTree(string filterKey);
        /// <summary>
        /// 获取字典分类信息和字典项目信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<(bool, string, DataDictionaryCodeDto?, List<DataDictionaryItemDto>?)> DicRead(string id);
        /// <summary>
        /// 字典分类状态
        /// </summary>
        /// <param name="type"></param>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DataDictionaryCodeDto)> DicState(string type, DicTreeItemDto data, UserModel user);
        /// <summary>
        /// 字典分类提交
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DataDictionaryCodeDto)> DicSave(DataDictionaryCodeDto data, UserModel user);
        /// <summary>
        /// 字典项目读取
        /// </summary>
        /// <param name="DicCode"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<DataDictionaryItemDto>)> DicItemLoad(string DicCode, UserModel user);
        /// <summary>
        /// 字典项目提交
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DataDictionaryItemDto)> DicItemSave(DataDictionaryItemDto data, UserModel user);
        /// <summary>
        /// 字典项目状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="itemid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, DataDictionaryItemDto)> DicItemState(string type, string itemid, UserModel user);
        /// <summary>
        /// 获取指定字典对象
        /// </summary>
        /// <param name="itemid"></param>
        /// <param name="yhid"></param>
        /// <returns></returns>
        DataDictionaryList GetDicItem(string itemid, string yhid = "000000");
    }
}
