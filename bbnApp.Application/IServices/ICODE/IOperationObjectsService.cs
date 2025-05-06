using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.Application.IServices.ICODE
{
    public interface IOperationObjectsService
    {
        /// <summary>
        /// 对象代码树
        /// </summary>
        /// <returns></returns>
        Task<(bool, string, List<OperationObjectNodeDto>)> OperationObjectTree();
        /// <summary>
        /// 对象代码清单
        /// </summary>
        /// <returns></returns>
        Task<(bool, string, List<OperationObjectCodeDto>)> OperationObjectCodeList();
        /// <summary>
        ///对象代码和操作代码读取
        /// </summary>
        /// <param name="ObjCode"></param>
        /// <returns></returns>
        Task<(bool, string, OperationObjectCodeDto, List<ObjectOperationTypeDto>)> GetOperationInfo(string ObjCode);
        /// <summary>
        /// 对象代码信息提交
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, OperationObjectCodeDto)> SaveOperationInfo(OperationObjectCodeDto data, UserModel user);
        /// <summary>
        /// 对象代码状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="ObjCode"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, OperationObjectCodeDto)> OperationState(string type, string ObjCode, UserModel user);
        /// <summary>
        /// 操作代码状态变更
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string)> ItemSave(ObjectOperationTypeDto data, UserModel user);
    }
}
