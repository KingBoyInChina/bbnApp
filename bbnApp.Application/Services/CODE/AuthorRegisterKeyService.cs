using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Business;
using bbnApp.Domain.Entities.Code;
using bbnApp.Domain.Entities.User;
using bbnApp.DTOs.CodeDto;
using bbnApp.Share;
using Dapper;
using Exceptionless;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Application.Services.CODE
{
    public class AuthorRegisterKeyService:IAuthorRegisterKeyService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbContext dbContext;
        /// <summary>
        /// 
        /// </summary>
        private readonly IRedisService redisService;
        /// <summary>
        /// 
        /// </summary>
        private readonly IDapperRepository dapperRepository;

        /// <summary>
        /// 
        /// </summary>
        private readonly ILogger<OperatorService> _logger;
        /// <summary>
        /// 
        /// </summary>
        private readonly ExceptionlessClient _exceptionlessClient;
        /// <summary>
        /// 
        /// </summary>
        private readonly IOperatorService operatorService;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="redisService"></param>
        public AuthorRegisterKeyService(IApplicationDbContext dbContext, IRedisService redisService, IDapperRepository _dapperRepository, ILogger<OperatorService> logger, ExceptionlessClient exceptionlessClient, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this.redisService = redisService;
            this.dapperRepository = _dapperRepository;
            this._logger = logger;
            this._exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
        }
        #region 注册密钥查询(内部)
        /// <summary>
        /// 注册密钥查询-分页
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, List<AuthorRegisterKeyItemDto>, Int32)> AuthorRegisterKeySearch(AuthorRegisterKeySearchRequestDto reqeust, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "authorregisterkeys", "browse"))
                {
                    StringBuilder SQL = new StringBuilder();
                    SQL.Append($"select * from {StaticModel.DbName.bbn}.authorregisterkeys where Isdelete=0");

                    var param = new DynamicParameters { };
                    if (!string.IsNullOrEmpty(reqeust.SetAppName))
                    {
                        SQL.Append(" and SetAppName like @SetAppName");
                        param.Add("SetAppName", $"%{reqeust.SetAppName}%");
                    }
                    if (!string.IsNullOrEmpty(reqeust.SetAppCode))
                    {
                        SQL.Append(" and SetAppCode like @SetAppCode");
                        param.Add("SetAppCode", $"%{reqeust.SetAppCode}%");
                    }
                    if (!string.IsNullOrEmpty(reqeust.AppId))
                    {
                        SQL.Append(" and AppId = @AppId");
                        param.Add("AppId", $"{reqeust.AppId}");
                    }
                    if (!string.IsNullOrEmpty(reqeust.CompanyId))
                    {
                        SQL.Append(" and CompanyId = @CompanyId");
                        param.Add("CompanyId", $"{reqeust.CompanyId}");
                    }
                    var (list, total) = await dapperRepository.QueryPagedAsync<AuthorRegisterKeys>(SQL.ToString(), param, reqeust.PageIndex, reqeust.PageSize);
                    return (true, "数据读取成功", ModelsToDto([.. list]), total);

                }
                return (false, "没有权限访问注册密钥查询", new List<AuthorRegisterKeyItemDto>(), 0);
            }
            catch (Exception ex)
            {
                return (false, $"注册密钥查询分页查询执行异常：{ex.Message.ToString()}", new List<AuthorRegisterKeyItemDto>(), 0);
            }
        }
        /// <summary>
        /// 注册密钥查询-不分页
        /// </summary>
        /// <param name="reqeust"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, List<AuthorRegisterKeyItemDto>)> AuthorRegisterKeyList(AuthorRegisterKeyListRequestDto reqeust, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "authorregisterkeys", "browse"))
                {
                    StringBuilder SQL = new StringBuilder();
                    SQL.Append($"select * from {StaticModel.DbName.bbn}.authorregisterkeys where Isdelete=0");

                    var param = new DynamicParameters { };
                    if (!string.IsNullOrEmpty(reqeust.SetAppName))
                    {
                        SQL.Append(" and SetAppName like @SetAppName");
                        param.Add("SetAppName", $"%{reqeust.SetAppName}%");
                    }
                    if (!string.IsNullOrEmpty(reqeust.SetAppCode))
                    {
                        SQL.Append(" and SetAppCode like @SetAppCode");
                        param.Add("SetAppCode", $"%{reqeust.SetAppCode}%");
                    }
                    if (!string.IsNullOrEmpty(reqeust.AppId))
                    {
                        SQL.Append(" and AppId = @AppId");
                        param.Add("AppId", $"{reqeust.AppId}");
                    }
                    if (!string.IsNullOrEmpty(reqeust.CompanyId))
                    {
                        SQL.Append(" and CompanyId = @CompanyId");
                        param.Add("CompanyId", $"{reqeust.CompanyId}");
                    }
                    var list = await dapperRepository.QueryAsync<AuthorRegisterKeys>(SQL.ToString(), param);
                    return (true, "数据读取成功", ModelsToDto([.. list]));

                }
                return (false, "没有权限访问注册密钥查询", new List<AuthorRegisterKeyItemDto>());
            }
            catch (Exception ex)
            {
                return (false, $"注册密钥查询不分页查询执行异常：{ex.Message.ToString()}", new List<AuthorRegisterKeyItemDto>());
            }
        }
        /// <summary>
        /// 新建密钥
        /// </summary>
        /// <param name="reqeust"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, AuthorRegisterKeyItemDto)> AuthorRegisterKeyAdd(AuthorRegisterKeyItemDto reqeust, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "authorregisterkeys", "add"))
                {
                    var EFObj = dbContext.Set<AuthorRegisterKeys>();

                    var model = EFObj.FirstOrDefault(x => x.AuthorId == reqeust.AuthorId && x.Yhid == user.Yhid && x.Isdelete == 0);
                    bool b = false;
                    if (model == null)
                    {
                        model = new AuthorRegisterKeys();
                        model.Yhid = user.Yhid;
                        model.AuthorId = Guid.NewGuid().ToString("N");
                        model.OperatorId = user.OperatorId;
                        model.IsLock = 0;
                        model.Isdelete = 0;
                        model.AppId = Guid.NewGuid().ToString("N").Substring(0, 12);
                        model.SecriteKey = Guid.NewGuid().ToString("N");
                        b = true;
                    }
                    model.CompanyId = reqeust.CompanyId;
                    model.SetAppName = reqeust.SetAppName;
                    model.SetAppCode = CommMethod.GetChineseSpell(model.SetAppName, false);
                    model.SetAppDescription = reqeust.SetAppDescription;
                    model.SelectedAppId = reqeust.SelectedAppId;
                    model.LastModified = DateTime.Now;
                    #region 逻辑校验
                    StringBuilder _error = new StringBuilder();
                    if (string.IsNullOrEmpty(model.SetAppName))
                    {
                        _error.AppendLine("应用名称不能为空");
                    }
                    if (string.IsNullOrEmpty(model.SetAppDescription))
                    {
                        _error.AppendLine("应用用途不能为空");
                    }
                    if (string.IsNullOrEmpty(model.CompanyId))
                    {
                        _error.AppendLine("所属机构不能为空");
                    }
                    if (model.OperatorId != user.OperatorId)
                    {
                        _error.AppendLine("非创建人不能操作");
                    }
                    if (EFObj.Any(x => x.Isdelete == 0 && x.SetAppName == model.SetAppName && x.CompanyId == model.CompanyId))
                    {
                        _error.AppendLine($"应用名称{model.SetAppName}已存在，请修改后重新提交");
                    }
                    #endregion
                    if (string.IsNullOrEmpty(_error.ToString()))
                    {
                        if (b)
                        {
                            await EFObj.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "注册成功", ModelToDto(model));
                    }
                    _ = AuthorRegisterInit(); //注册密钥变更后，重新初始化到redis
                    return (false, _error.ToString(), new AuthorRegisterKeyItemDto());
                }
                return (false, "没有权限访问注册密钥查询", new AuthorRegisterKeyItemDto());
            }
            catch (Exception ex)
            {
                return (false, $"新建密钥执行异常：{ex.Message.ToString()}", new AuthorRegisterKeyItemDto());
            }
        }
        /// <summary>
        /// 状态变更
        /// </summary>
        /// <param name="reqeust"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, AuthorRegisterKeyItemDto)> AuthorRegisterKeyState(AuthorRegisterKeyStateRequestDto reqeust, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "authorregisterkeys", reqeust.Type == "IsDelete" ? "delete" : "edit"))
                {
                    var EFObj = dbContext.Set<AuthorRegisterKeys>();

                    var model = EFObj.FirstOrDefault(x => x.AuthorId == reqeust.AuthorId && x.Yhid == user.Yhid && x.Isdelete == 0);
                    if (model == null)
                    {
                        return (false, "无效的密钥注册信息", new AuthorRegisterKeyItemDto());
                    }
                    if (reqeust.Type == "IsDelete")
                    {
                        #region 删除
                        model.Isdelete = 1;
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        _ = AuthorRegisterInit(); //注册密钥变更后，重新初始化到redis
                        return (true, "注册密钥删除成功", new AuthorRegisterKeyItemDto { });
                        #endregion
                    }
                    else if (reqeust.Type == "IsLock")
                    {
                        #region 锁定或解锁
                        model.IsLock = model.IsLock == 0 ? Convert.ToByte(1) : Convert.ToByte(0);
                        model.LockReason = model.IsLock == 0 ? string.Empty : reqeust.Reason;
                        model.LockTime = model.IsLock == 0 ? DateTime.MinValue : DateTime.Now;
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        _ = AuthorRegisterInit(); //注册密钥变更后，重新初始化到redis
                        return (true, "状态变更成功", ModelToDto(model));
                        #endregion
                    }
                    else if (reqeust.Type == "IsRead")
                    {
                        #region 读取信息
                        return (true, "数据读取成功", ModelToDto(model));
                        #endregion
                    }
                }
                return (false, "没有权限访问注册密钥查询", new AuthorRegisterKeyItemDto());
            }
            catch (Exception ex)
            {
                return (false, $"注册密钥状态变更执行异常：{ex.Message.ToString()}", new AuthorRegisterKeyItemDto());
            }
        }
        /// <summary>
        /// 注册机构密钥清单
        /// </summary>
        /// <param name="AreaCod"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool, string, List<CompanyAuthorRegistrKeyItemDto>)> CompanyAuthorRegistrKeySearch(string AreaCod, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "authorregisterkeys", "browse"))
                {
                    var EFCompany = dbContext.Set<CompanyInfo>();
                    var EFAuthorReg = dbContext.Set<AuthorRegisterKeys>();
                    var keyslist = EFAuthorReg.Where(x => x.Isdelete == 0 && x.Yhid == user.Yhid).OrderBy(x => x.LastModified).ToList();
                    var companyList = EFCompany.Where(x => x.Isdelete == 0 && x.AreaCode.StartsWith(AreaCod)).OrderBy(x => x.CompanyId).OrderBy(x => x.CompanyLeve).ToList();

                    var list=companyList.Select(x => new CompanyAuthorRegistrKeyItemDto
                    {
                        CompanyId = x.CompanyId,
                        CompanyName = x.CompanyName,
                        RegisterKeyCount = keyslist.Count(i => i.CompanyId == x.CompanyId),
                        RegisterKeys = keyslist.Where(i => i.CompanyId == x.CompanyId).Select(i => ModelToDto(i, 1)).ToList()
                    }).ToList();

                    return (true, "数据读取成功", list);
                }
                return (false, "无权进行操作", new List<CompanyAuthorRegistrKeyItemDto>());
            }
            catch (Exception ex)
            {
                return (false, $"机构密钥清单查询异常：{ex.Message.ToString()}", new List<CompanyAuthorRegistrKeyItemDto>());
            }
        }
        #endregion
        #region 初始化到redis
        /// <summary>
        /// 初始化注册密钥到redis，MQTT服务启动时调用接口得到所有的注册信息
        /// </summary>
        /// <returns></returns>
        public async Task AuthorRegisterInit()
        {
            try
            {
                var EFObj = dbContext.Set<AuthorRegisterKeys>();
                var models = EFObj.Where(x => x.IsLock == 0 && x.Isdelete == 0).Select(x => new AuthorReginsterKeyClientDto
                {
                    ClientId="",
                    ClientName="",
                    SetAppName=x.SetAppName,
                    AuthorId=x.AuthorId,
                    AppId=x.AppId,
                    SecriteKey=x.SecriteKey
                });
                //写入redis
                await redisService.SetAsync("RegisterKeys", JsonConvert.SerializeObject(models));
            }
            catch (Exception ex)
            {
                _logger.LogError($"AuthorRegisterInit 初始化操作异常：{ex.Message.ToString()}");
                _exceptionlessClient.CreateException(ex).AddTags("AuthorRegisterInit").Submit();
            }
        }
        /// <summary>
        /// MQTT注册密钥初始化，MQTT服务启动时调用接口得到所有的注册信息
        /// 注册的密钥关联设备信息,目前还没做到设备管理哪里，做到涉笔管理后，这里需要进行调整
        /// </summary>
        /// <returns></returns>
        public async Task<List<AuthorReginsterKeyClientDto>> AuthorRegisterKeyClients()
        {
            try
            {
                var redisData = await redisService.GetAsync("RegisterKeys");
                if (string.IsNullOrEmpty(redisData))
                {
                    return new List<AuthorReginsterKeyClientDto>();
                }
                else
                {
                    var EFObj = dbContext.Set<AuthorRegisterKeys>();
                    var models = EFObj.Where(x => x.IsLock == 0 && x.Isdelete == 0).Select(x => new AuthorReginsterKeyClientDto
                    {
                        ClientId = "",
                        ClientName = "",
                        SetAppName = x.SetAppName,
                        AuthorId = x.AuthorId,
                        AppId = x.AppId,
                        SecriteKey = x.SecriteKey,
                        Yhid=x.Yhid,
                        CompanyId=x.CompanyId,
                        OperatorId=x.OperatorId
                    }).ToList();
                    return models;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"MQTTAuthorRegisterKeysInit 初始化操作异常：{ex.Message.ToString()}");
                _exceptionlessClient.CreateException(ex).AddTags("MQTTAuthorRegisterKeysInit").Submit();
                return new List<AuthorReginsterKeyClientDto>();
            }
        }
        /// <summary>
        /// 获取注册密钥（一般用于平台）
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        public async Task<(bool, string, AuthorReginsterKeyClientDto)> GetAuthorRegisterKey(string Yhid,string CompanyId,string OperatorId)
        {
            try
            {
                var EFObj = dbContext.Set<AuthorRegisterKeys>();
                var model =await EFObj.Where(x => x.IsLock == 0 && x.Isdelete == 0&&x.Yhid==Yhid&&x.CompanyId==CompanyId&&x.OperatorId==OperatorId).Select(x => new AuthorReginsterKeyClientDto
                {
                    ClientId = "",
                    ClientName = "",
                    SetAppName = x.SetAppName,
                    AuthorId = x.AuthorId,
                    AppId = x.AppId,
                    SecriteKey = x.SecriteKey
                }).FirstOrDefaultAsync();
                return (true,"密钥获取成功", model??new AuthorReginsterKeyClientDto());
            }
            catch(Exception ex)
            {
                return (false,$"密钥信息获取异常：{ex.Message.ToString()}",new AuthorReginsterKeyClientDto());
            }
        }
        #endregion
        #region 数据对象解析
        /// <summary>
        /// list转换为dto
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        private List<AuthorRegisterKeyItemDto> ModelsToDto(List<AuthorRegisterKeys> models)
        {
            List<AuthorRegisterKeyItemDto> list = new List<AuthorRegisterKeyItemDto>();
            int index = 1;
            foreach (var model in models)
            {
                list.Add(ModelToDto(model, index));
                index++;
            }
            return list;
        }
        /// <summary>
        /// model转换为dto
        /// </summary>
        /// <param name="model"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private AuthorRegisterKeyItemDto ModelToDto(AuthorRegisterKeys model, Int32 index = 1)
        {
            return new AuthorRegisterKeyItemDto
            {
                IdxNum = index,
                Yhid = model.Yhid,
                AuthorId = CommMethod.GetValueOrDefault(model.AuthorId, ""),
                SelectedAppId = CommMethod.GetValueOrDefault(model.SelectedAppId, ""),
                CompanyId = CommMethod.GetValueOrDefault(model.CompanyId, ""),
                AppId = CommMethod.GetValueOrDefault(model.AppId, ""),
                SetAppName = CommMethod.GetValueOrDefault(model.SetAppName, ""),
                SetAppCode = CommMethod.GetValueOrDefault(model.SetAppCode, ""),
                SetAppDescription = CommMethod.GetValueOrDefault(model.SetAppDescription, ""),
                SecriteKey = CommMethod.GetValueOrDefault(model.SecriteKey, ""),
                OperatorId = CommMethod.GetValueOrDefault(model.OperatorId, ""),
                IsLock = CommMethod.GetValueOrDefault(model.IsLock, Convert.ToByte(0)),
                LockReason = CommMethod.GetValueOrDefault(model.LockReason, ""),
                LockTime = CommMethod.GetValueOrDefault(model.LockTime, ""),
                ReMarks = CommMethod.GetValueOrDefault(model.ReMarks, "")
            };
        }
        #endregion
    }
}
