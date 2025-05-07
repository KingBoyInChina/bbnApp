using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;
using bbnApp.Share;
using Dapper;
using Exceptionless;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace bbnApp.Application.Services.CODE
{
    public class AppSettingService : IAppSettingService
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbCodeContext dbContext;
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
        public AppSettingService(IApplicationDbCodeContext dbContext, IRedisService redisService, IDapperRepository _dapperRepository, ILogger<OperatorService> logger, ExceptionlessClient exceptionlessClient, IOperatorService operatorService)
        {
            this.dbContext = dbContext;
            this.redisService = redisService;
            this.dapperRepository = _dapperRepository;
            this._logger = logger;
            this._exceptionlessClient = exceptionlessClient;
            this.operatorService = operatorService;
        }
        /// <summary>
        /// 依赖注入-初始化
        /// </summary>
        /// <returns></returns>
        public async Task AppSettingInit()
        {
            try
            {
                string SQL = $"select ROW_NUMBER() OVER (ORDER BY SettingIndex ASC) as IdxNum,Yhid,SettingId,SettingCode,SettingName,SettingDesc,SettingIndex,SettingType,NowValue,DefaultValue,ValueRange,IsFiexed,IsLock,ifnull(DATE_FORMAT(LockTime, '%Y-%m-%d'),'') as LockTime,ifnull(LockReason,'') as LockReason,ifnull(ReMarks,'') as ReMarks from {StaticModel.DbName.bbn_code}.appsettings where Isdelete=0";
                var list = await dapperRepository.QueryJArrayAsync(SQL.ToString());
                await redisService.SetAsync("AppSetting", list.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"AppSettingInit 系统配置信息初始化异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"AppSettingInit 系统配置信息初始化异常：{ex.Message.ToString()}"));
                throw;
            }
        }
        /// <summary>
        /// 下载系统配置
        /// </summary>
        /// <returns></returns>
        public async Task<(bool,string,List<AppSettingDto>)> AppSettingDownLoad()
        {
            try
            {
                string settings = await redisService.GetAsync("AppSetting");
                if (!string.IsNullOrEmpty(settings))
                {
                    return (true,"配置项读取成功",JsonConvert.DeserializeObject<List<AppSettingDto>>(settings));
                }
                return (false, "没有找有配置项的缓存数据", new List<AppSettingDto>());
            }
            catch(Exception ex)
            {
                return (false,ex.Message.ToString(),new List<AppSettingDto>());
            }
        }
        /// <summary>
        /// 获取当前配置值
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="SettingCode"></param>
        /// <returns>当前值,默认值</returns>
        public async Task<(string, string)> GetAppSettingValue(string Yhid, string SettingCode, UserModel user)
        {
            try
            {
                string settings = await redisService.GetAsync("AppSetting");
                if (!string.IsNullOrEmpty(settings))
                {
                    JArray jsonArray = JArray.Parse(settings);
                    var code = jsonArray.Where(x => x["SettingCode"].ToString() == SettingCode && x["Yhid"].ToString() == Yhid).FirstOrDefault();
                    if (code != null)
                    {
                        return (code["NowValue"].ToString(), code["DefaultValue"].ToString());
                    }
                }
                return ("", "");
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetAppSettingValue {Yhid}获取配置信息{SettingCode}异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"GetAppSettingValue {Yhid}获取配置信息{SettingCode}异常：{ex.Message.ToString()}"));
                throw;
            }
        }
        /// <summary>
        /// 配置信息查询
        /// </summary>
        /// <param name="reqeust"></param>
        /// <returns></returns>
        public async Task<(bool,string,IEnumerable<AppSettingDto>?,int)> AppSettingSearch(AppSettingSearchRequestDto reqeust, UserModel user)
        {
            try
            {

                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "appsettings", "browse"))
                {
                    StringBuilder SQL = new StringBuilder();
                    SQL.Append($"select ROW_NUMBER() OVER (ORDER BY SettingIndex ASC) as IdxNum,Yhid,SettingId,SettingCode,SettingName,SettingDesc,SettingIndex,SettingType,NowValue,DefaultValue,ValueRange,IsFiexed,IsLock,ifnull(DATE_FORMAT(LockTime, '%Y-%m-%d'),'') as LockTime,ifnull(LockReason,'') as LockReason,ifnull(ReMarks,'') as ReMarks from {StaticModel.DbName.bbn_code}.appsettings where Isdelete=0");

                    var param = new DynamicParameters { };
                    if (!string.IsNullOrEmpty(reqeust.SettingName))
                    {
                        SQL.Append(" and SettingName like @SettingName");
                        param.Add("SettingName", $"%{reqeust.SettingName}%");
                    }
                    if (!string.IsNullOrEmpty(reqeust.SettingCode))
                    {
                        SQL.Append(" and SettingCode like @SettingCode");
                        param.Add("SettingCode", $"%{reqeust.SettingCode}%");
                    }
                    if (!string.IsNullOrEmpty(reqeust.SettingDesc))
                    {
                        SQL.Append(" and SettingDesc like @SettingDesc");
                        param.Add("SettingDesc", $"%{reqeust.SettingDesc}%");
                    }
                    if (reqeust.IsLock != byte.MinValue)
                    {
                        SQL.Append(" and IsLock =@SettingCode");
                        param.Add("IsLock", $"{reqeust.IsLock}");
                    }
                    if (reqeust.IsFiexed != byte.MinValue)
                    {
                        SQL.Append(" and IsFiexed =@IsFiexed");
                        param.Add("IsFiexed", $"{reqeust.IsFiexed}");
                    }
                    var (list, total) = await dapperRepository.QueryPagedAsync<AppSettingDto>(SQL.ToString(), param, reqeust.PageIndex, reqeust.PageSize);
                    return (true,"数据读取成功",list, total);
                }
                return (false, "无权进行操作", null,0);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AppSettingSearch 系统配置信息查询异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"AppSettingSearch 系统配置信息查询异常：{ex.Message.ToString()}"));

                return (false, $"配置信息查询异常：{ex.Message.ToString()}", null, 0);
            }
        }
        /// <summary>
        /// 配置信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<(bool,string, AppSettingDto?)> AppSettingSave(AppSettingPostRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "appsettings", "browse"))
                {
                    var postData = request.Item;
                    bool b = true;
                    var appsetting = dbContext.Set<AppSettings>();
                    AppSettings model = GetAppSetting(appsetting, postData.SettingId, postData.Yhid, user);
                    if (model != null)
                    {
                        b = false;
                    }
                    #region 写数据
                    string settingid = "1";
                    int settingindex = 1;
                    if (b)
                    {
                        model = new AppSettings();
                        model.Yhid = postData.Yhid;
                        var maxindex = appsetting.Max(x => x.SettingIndex);
                        if (maxindex > 0)
                        {
                            settingindex = maxindex++;
                            settingid = settingindex.ToString();
                        }
                        model.SettingId = settingid;
                        model.IsFiexed = postData.IsFiexed;
                        model.IsLock = postData.IsLock;
                        model.LockTime = CommMethod.GetValueOrDefault(postData.LockTime, DateTime.MinValue);
                        model.LockReason = postData.LockReason;
                        model.Isdelete = 0;
                    }
                    if (model != null)
                    {
                        model.SettingCode = postData.SettingCode;
                        model.SettingName = postData.SettingName;
                        model.SettingDesc = postData.SettingDesc;
                        model.SettingIndex = postData.SettingIndex == int.MinValue ? settingindex : postData.SettingIndex;
                        model.SettingType = postData.SettingType;
                        model.NowValue = postData.NowValue;
                        model.DefaultValue = postData.DefaultValue;
                        model.ValueRange = postData.ValueRange;
                        model.ReMarks = postData.ReMarks;
                        model.LastModified = DateTime.Now;
                    }
                    #endregion
                    #region 逻辑校验
                    string strMsg = string.Empty;
                    if (string.IsNullOrEmpty(model.SettingName))
                    {
                        strMsg = "配置名称不能为空";
                    }
                    else if (string.IsNullOrEmpty(model.SettingCode))
                    {
                        strMsg = "配置代码不能为空";
                    }
                    else if (string.IsNullOrEmpty(model.SettingType))
                    {
                        strMsg = "配置参数类型不能为空";
                    }
                    else if (string.IsNullOrEmpty(model.NowValue))
                    {
                        strMsg = "当前值不能为空";
                    }
                    else if (string.IsNullOrEmpty(model.DefaultValue))
                    {
                        strMsg = "默认值不能为空";
                    }
                    else if (string.IsNullOrEmpty(model.SettingDesc))
                    {
                        strMsg = "配置说明不能为空";
                    }
                    else if (model.SettingIndex <= 0)
                    {
                        strMsg = "序号必须大于0";
                    }
                    var list = appsetting.Where(x => x.SettingCode == postData.SettingCode && x.Yhid == postData.Yhid && x.SettingId != postData.SettingId).ToList();
                    if (list.Count > 0)
                    {
                        strMsg = $"{postData.SettingCode}已存在,请勿重复添加";
                    }
                    #endregion
                    if (string.IsNullOrEmpty(strMsg))
                    {

                        if (b)
                        {
                            await appsetting.AddAsync(model);
                        }
                        await dbContext.SaveChangesAsync();
                        #region 更新缓存
                        _ = AppSettingInit();
                        #endregion
                        return (true, "配置参数保存完成", ModelToDto(model));
                    }
                    else
                    {
                        return (false, strMsg, null);
                    }
                }
                else
                {
                    return (false,"无权进行操作",null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"AppSettingSave 系统配置信息提交异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"AppSettingSave 系统配置信息提交异常：{ex.Message.ToString()}"));
                throw;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="SettingId"></param>
        /// <param name="Yhid"></param>
        /// <returns></returns>
        private AppSettings GetAppSetting(DbSet<AppSettings> appsetting, string SettingId, string Yhid, UserModel user)
        {
            AppSettings model = new AppSettings();
            model = appsetting.Where(x => x.SettingId == SettingId && x.Yhid == Yhid).FirstOrDefault();
            return model;
        }
        /// <summary>
        /// 生成DTO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private AppSettingDto ModelToDto(AppSettings model)
        {
            return new AppSettingDto
            {
                SettingCode = model.SettingCode,
                SettingName = model.SettingName,
                SettingDesc = model.SettingDesc,
                SettingIndex = model.SettingIndex,
                SettingType = model.SettingType,
                NowValue = model.NowValue,
                DefaultValue = model.DefaultValue,
                ValueRange = model.ValueRange,
                IsFiexed = model.IsFiexed,
                IsLock = model.IsLock,
                LockTime = model.LockTime==null?DateTime.MinValue.ToString(): model.LockTime.ToString(),
                LockReason =string.IsNullOrEmpty(model.LockReason)?"": model.LockReason,
                ReMarks = string.IsNullOrEmpty(model.ReMarks) ? "" : model.ReMarks,
                SettingId = model.SettingId,
                Yhid = model.Yhid
            };
        }
        /// <summary>
        /// 配置信息状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<(bool,string, AppSettingDto?)> AppSettingStateSave(AppSettingStateRequestDto request, UserModel user)
        {
            try
            {
                if (await operatorService.IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "appsettings", "permit"))
                {
                    AppSettingStateResponseDto response = new AppSettingStateResponseDto();
                    string type = request.Type;
                    AppSettingDto appSettingDto = request.Item;
                    var appsetting = dbContext.Set<AppSettings>();
                    AppSettings model = GetAppSetting(appsetting, appSettingDto.SettingId, appSettingDto.Yhid, user);
                    if (model != null)
                    {

                        switch (type)
                        {
                            case "IsLock":
                                model.IsLock = model.IsLock == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                                model.LockTime = model.IsLock == 1 ? DateTime.Now : DateTime.MinValue;
                                model.LockReason = model.IsLock == 1 ? appSettingDto.LockReason : string.Empty;
                                model.LastModified = DateTime.Now;
                                //appsetting.Update(model);
                                break;
                            case "IsFixed":
                                model.IsFiexed = model.IsFiexed == 1 ? Convert.ToByte(0) : Convert.ToByte(1);
                                model.LastModified = DateTime.Now;
                                //appsetting.Update(model);
                                break;
                            case "IsDelete":
                                model.Isdelete = 1;
                                model.LastModified = DateTime.Now;
                                //appsetting.Update(model);
                                break;
                        }
                        await dbContext.SaveChangesAsync();
                        return (true, "操作成功", ModelToDto(model));
                    }
                    else
                    {
                        return (false, "数据不存在", null);
                    }
                }
                return (false,"无权进行操作",null);
            }
            catch (Exception ex)
            {
                _logger.LogError($"AppSettingStateSave 系统配置信息状态变更异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"AppSettingStateSave 系统配置信息状态变更异常：{ex.Message.ToString()}"));
                return (false, $"状态变更异常：{ex.Message.ToString()}", null);
            }
        }
    }
}
