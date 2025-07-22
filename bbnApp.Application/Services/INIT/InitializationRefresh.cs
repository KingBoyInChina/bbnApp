using bbnApp.Application.IServices.IBusiness;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.IServices.IINIT;
using bbnApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace bbnApp.Application.Services.INIT
{
    /// <summary>
    /// 手动刷新服务端初始化到redis中的数据
    /// </summary>
    public class InitializationRefresh: IInitializationRefresh
    {
        private readonly IDictionaryInitialization dictionaryInitialization;
        private readonly IOperatorService operatorService;
        private readonly ICompanyService companyService;
        private readonly IAreaService areaService;
        private readonly IAppSettingService appSettingService;
        private readonly IDataDictionaryService dataDictionaryService;
        private readonly IAuthorRegisterKeyService authorRegisterKeyService;
        private readonly IUserDevices userDevices;
        public InitializationRefresh(IDictionaryInitialization dictionaryInitialization, IOperatorService operatorService, ICompanyService companyService, IAreaService areaService, IAppSettingService appSettingService, IDataDictionaryService dataDictionaryService, IAuthorRegisterKeyService authorRegisterKeyService, IUserDevices userDevices)
        {
            this.dictionaryInitialization = dictionaryInitialization;
            this.operatorService = operatorService;
            this.companyService = companyService;
            this.areaService = areaService;
            this.appSettingService = appSettingService;
            this.dataDictionaryService = dataDictionaryService;
            this.authorRegisterKeyService = authorRegisterKeyService;
            this.userDevices = userDevices;
        }
        /// <summary>
        /// 字典刷新
        /// </summary>
        /// <param name="type"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string)> Refresh(string type,UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "centerdatarefresh", "permit"))
                {
                    if (type.Contains("PubCode"))
                    {
                        //公共字典初始化
                        await dictionaryInitialization.DictionaryInit();
                    }
                    if (type.Contains("Operator"))
                    {
                        //操作员信息初始化
                        await operatorService.OperatorInitlize();
                    }
                    if (type.Contains("Company"))
                    {
                        //机构信息初始化
                        await companyService.CompanyInit();
                    }
                    if (type.Contains("Area"))
                    {
                        //行政区划初始化
                        await areaService.AreaInit();
                    }
                    if (type.Contains("AppSetting"))
                    {
                        //系统配置初始化
                        await appSettingService.AppSettingInit();
                    }
                    if (type.Contains("Dictionary"))
                    {
                        //数据字典初始化
                        await dataDictionaryService.DicInit();
                    }
                    if (type.Contains("AuthorRegister"))
                    {
                        //初始化注册密钥
                        await authorRegisterKeyService.AuthorRegisterInit();
                    }
                    if (type.Contains("UserDevices"))
                    {
                        //初始化设备清单
                        await userDevices.UserGetWayDeviceInit(string.Empty);
                    }
                    return (true, "数据更新执行完成");
                }
                return (false,"无权进行操作");
            }
            catch (Exception ex)
            {
                return (false,$"服务端数据刷新异常：{ex.Message.ToString()}");
            }
        }
    }
}
