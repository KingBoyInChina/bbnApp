using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.IServices.ICODE
{
    public interface IMaterialsCodeService
    {
        /// <summary>
        /// 获取物资树
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<MaterialTreeItemDto>)> GetMaterailTree(UserModel user);
        /// <summary>
        /// 物资信息明细
        /// </summary>
        /// <param name="MaterialId"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, MaterialsCodeDto)> GetMaterialInfo(string MaterialId, UserModel user);
        /// <summary>
        /// 物资信息列表
        /// </summary>
        /// <param name="MaterialType"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<MaterialsCodeDto>)> GetMaterialList(string MaterialType, UserModel user);
        /// <summary>
        /// 物资信息提交
        /// </summary>
        /// <param name="data"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, MaterialsCodeDto)> MaterialSave(MaterialsCodeDto data, UserModel user);
        /// <summary>
        /// 物资状态变更
        /// </summary>
        /// <param name="type"></param>
        /// <param name="MaterialId"></param>
        /// <param name="LockReason"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, MaterialsCodeDto)> MaterialState(string type, string MaterialId,string LockReason, UserModel user);
    }
}
