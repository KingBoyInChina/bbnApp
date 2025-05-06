using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.IServices.ICODE
{
    /// <summary>
    /// 系统配置服务接口
    /// </summary>
    public interface IAppSettingService
    {
        /// <summary>
        /// 依赖注入-初始化
        /// </summary>
        /// <returns></returns>
        Task AppSettingInit();
        /// <summary>
        /// 配置信息下载
        /// </summary>
        /// <returns></returns>
        Task<(bool, string, List<AppSettingDto>)> AppSettingDownLoad();
        /// <summary>
        /// 获取配置值-(当前值,默认值)
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="SettingCode"></param>
        /// <returns></returns>
        Task<(string, string)> GetAppSettingValue(string Yhid, string SettingCode, UserModel user);
        /// <summary>
        /// 配置信息查询
        /// </summary>
        /// <param name="reqeust"></param>
        /// <returns></returns>
        Task<(bool,string,IEnumerable<AppSettingDto>?, int)> AppSettingSearch(AppSettingSearchRequestDto reqeust, UserModel user);
        /// <summary>
        /// 配置信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<(bool,string, AppSettingDto)> AppSettingSave(AppSettingPostRequestDto request, UserModel user);
        /// <summary>
        /// 配置信息状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<(bool,string, AppSettingDto)> AppSettingStateSave(AppSettingStateRequestDto request, UserModel user);
    }
}
