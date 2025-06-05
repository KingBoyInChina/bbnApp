using bbnApp.Application.DTOs.LoginDto;
using bbnApp.Common.Models;
using bbnApp.Domain.Entities.User;
using bbnApp.DTOs.CodeDto;
using Newtonsoft.Json.Linq;

namespace bbnApp.Application.IServices.ICODE
{
    /// <summary>
    /// 操作员相关接口
    /// </summary>
    public interface IOperatorService
    {
        /// <summary>
        /// 操作员初始化-操作员信息、操作员权限信息、存储到redis
        /// </summary>
        Task OperatorInitlize();
        /// <summary>
        /// 操作员信息更新
        /// </summary>
        Task OperatorRefresh();
        /// <summary>
        /// 通过操作员ID获取操作员对象
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        Task<UserModel> GetOperator(string Yhid, string OperatorId);
        /// <summary>
        /// 通过登录账号和密码获取操作员信息
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        Task<(bool, string, LoginResponseDto?)> OperatorLogin(LoginRequestDto loginRequest);
        /// <summary>
        /// 获取操作员可以使用的顶部菜单
        /// </summary>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        Task<List<TopMenuItemDto>> GetOperatorTopMenu(string Yhid, string CompanyId, string OperatorId);
        /// <summary>
        /// 判断操作员是否拥有指定的操作权限
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <param name="ObjCode"></param>
        /// <param name="PermissionCode"></param>
        /// <returns></returns>
        Task<bool> IsAccess(string Yhid, string CompanyId, string OperatorId, string ObjCode, string PermissionCode);
        /// <summary>
        /// 获取指定操作有拥有的所有操作权限
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        Task<JArray> OperatorAccess(string Yhid, string CompanyId, string OperatorId);
        /// <summary>
        /// 密码更新
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <param name="OldPassWord"></param>
        /// <param name="NewPassWord"></param>
        /// <returns></returns>
        Task<Operators> UpdatePassWord(string Yhid, string CompanyId, string OperatorId, string OldPassWord, string NewPassWord);
    }
}
