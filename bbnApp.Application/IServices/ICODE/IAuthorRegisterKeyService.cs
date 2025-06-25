using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using Newtonsoft.Json.Linq;

namespace bbnApp.Application.IServices.ICODE
{
    public interface IAuthorRegisterKeyService
    {
        /// <summary>
        /// 注册机构密钥清单
        /// </summary>
        /// <param name="AreaCod"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<CompanyAuthorRegistrKeyItemDto>)> CompanyAuthorRegistrKeySearch(string AreaCod, UserModel user);
        /// <summary>
        /// 注册密钥查询-分页
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<AuthorRegisterKeyItemDto>, Int32)> AuthorRegisterKeySearch(AuthorRegisterKeySearchRequestDto reqeust, UserModel user);
        /// <summary>
        /// 注册密钥查询-不分页
        /// </summary>
        /// <param name="reqeust"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, List<AuthorRegisterKeyItemDto>)> AuthorRegisterKeyList(AuthorRegisterKeyListRequestDto reqeust, UserModel user);
        /// <summary>
        /// 新建密钥
        /// </summary>
        /// <param name="reqeust"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, AuthorRegisterKeyItemDto)> AuthorRegisterKeyAdd(AuthorRegisterKeyItemDto reqeust, UserModel user);
        /// <summary>
        /// 状态变更
        /// </summary>
        /// <param name="reqeust"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<(bool, string, AuthorRegisterKeyItemDto)> AuthorRegisterKeyState(AuthorRegisterKeyStateRequestDto reqeust, UserModel user);
        /// <summary>
        /// 初始化注册密钥到redis，MQTT服务启动时调用接口得到所有的注册信息
        /// </summary>
        /// <returns></returns>
        Task AuthorRegisterInit();
        /// <summary>
        /// MQTT注册密钥初始化，MQTT服务启动时调用接口得到所有的注册信息
        /// 注册的密钥关联设备信息,目前还没做到设备管理哪里，做到涉笔管理后，这里需要进行调整
        /// </summary>
        /// <returns></returns>
        Task<List<AuthorReginsterKeyClientDto>> AuthorRegisterKeyClients();
        /// <summary>
        /// 获取注册密钥（一般用于平台）
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        Task<(bool, string, AuthorReginsterKeyClientDto)> GetAuthorRegisterKey(string Yhid, string CompanyId, string OperatorId);
    }
}
