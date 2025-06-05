using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using bbnApp.Domain.Entities.User;
using bbnApp.Share;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using bbnApp.Application.DTOs.LoginDto;
using Exceptionless;
using bbnApp.Application.IServices.IJWT;
using bbnApp.Domain.Entities.Code;
using bbnApp.Domain.Entities.UserLogin;
using Microsoft.Extensions.Logging;
using bbnApp.Infrastructure.Data;
using bbnApp.DTOs.CodeDto;

namespace bbnApp.Application.Services.CODE
{
    public class OperatorService: IOperatorService
    {
        /// <summary>
        /// darpper
        /// </summary>
        private readonly IDapperRepository _dapperRepository;
        /// <summary>
        /// jwt服务
        /// </summary>
        private readonly IJwtService _jwtService;
        /// <summary>
        /// redis 服务
        /// </summary>
        private readonly IRedisService _redisService;
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbContext _codeContext;
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
        /// <param name="dapperRepository"></param>
        /// <param name="redisService"></param>
        /// <param name="jwtService"></param>
        /// <param name="codeContext"></param>
        public OperatorService(
            IDapperRepository dapperRepository,
            IRedisService redisService,
            IJwtService jwtService,
            IApplicationDbContext codeContext,
            ILogger<OperatorService> logger,
            ExceptionlessClient exceptionlessClient
            ) 
        {
            _dapperRepository = dapperRepository;
            _redisService = redisService;
            _jwtService = jwtService;
            _codeContext = codeContext;
            _logger = logger;
            _exceptionlessClient = exceptionlessClient;
        }
        /// <summary>
        /// 操作员信息初始化到Redis
        /// 将所有操作员信息写入Redis
        /// </summary>
        public async Task OperatorInitlize()
        {
            try
            {
                string sql = $"select * from {StaticModel.DbName.bbn}.vm_operators";
                var data = await _dapperRepository.QueryJArrayAsync(sql);
                //写入redis中
                await _redisService.SetAsync("Operator", data.ToString());
            }
            catch(Exception ex)
            {
                _logger.LogError($"OperatorInitlize 操作员信息初始化到Redis异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"OperatorInitlize 操作员信息初始化到Redis异常：{ex.Message.ToString()}"));
            }
        }
        /// <summary>
        /// redis中操作员信息更新
        /// </summary>
        public async Task OperatorRefresh()
        {
            await OperatorInitlize();
        }
        /// <summary>
        /// 通过操作员ID获取操作员信息
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        public async Task<UserModel>? GetOperator(string Yhid, string OperatorId)
        {
            try
            {
                JObject Obj = await GetOperatorObj(Yhid,OperatorId);
                if (Obj != null)
                {
                    Obj.Remove("PassWord");
                    UserModel model =  JsonConvert.DeserializeObject<UserModel>(Obj.ToString());
                    return model;
                }
                else
                {
                    throw new Exception("无效的身份信息");
                }
                //var operators = await _redisService.GetAsync("Operator");
                //if (string.IsNullOrEmpty(operators))
                //{
                //    return null;
                //}
                //JArray ArrOperators = JArray.Parse(operators);
                //var Operator = ArrOperators.Where(x => CommMethod.GetValueOrDefault(x["Yhid"], string.Empty) == Yhid && CommMethod.GetValueOrDefault(x["OperatorId"], string.Empty) == OperatorId).ToList();
                //if (Operator.Count == 0)
                //{
                //    throw new Exception("无效的身份信息");
                //}
                //else if (Operator.Count > 1)
                //{
                //    throw new Exception("检测到重复的身份信息，请联系管理处理");
                //}
                //else
                //{
                //    JObject ObjOperator = ArrOperators.FirstOrDefault() as JObject;
                //    ObjOperator.Remove("PassWord");
                //    UserModel model = JsonConvert.DeserializeObject<UserModel>(ObjOperator.ToString());
                //}
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetOperator 通过操作员ID获取操作员信息：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"GetOperator 通过操作员ID获取操作员信息：{ex.Message.ToString()}"));
            }
            return null;
        }
        /// <summary>
        /// 获取操作员Object对象
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        private async Task<JObject>? GetOperatorObj(string Yhid, string OperatorId)
        {
            try
            {
                var operators = await _redisService.GetAsync("Operator");
                if (string.IsNullOrEmpty(operators))
                {
                    return null;
                }
                JArray ArrOperators = JArray.Parse(operators);
                var Operator = ArrOperators.Where(x => CommMethod.GetValueOrDefault(x["Yhid"], string.Empty) == Yhid && CommMethod.GetValueOrDefault(x["OperatorId"], string.Empty) == OperatorId).ToList();
                if (Operator.Count == 0)
                {
                    throw new Exception("无效的身份信息");
                }
                else if (Operator.Count > 1)
                {
                    throw new Exception("检测到重复的身份信息，请联系管理处理");
                }
                else
                {
                    JObject ObjOperator = ArrOperators.FirstOrDefault() as JObject;
                    return ObjOperator;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetOperator 通过操作员ID获取操作员信息：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"GetOperator 通过操作员ID获取操作员信息：{ex.Message.ToString()}"));
            }
            return null;
        }
        /// <summary>
        /// 通过账号，密码获取操作员信息
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public async Task<(bool,string,LoginResponseDto?)> OperatorLogin(LoginRequestDto loginRequest)
        {
            try
            {
                var operators = await _redisService.GetAsync("Operator");
                if (string.IsNullOrEmpty(operators))
                {
                    return (false,"平台操作员信息未初始化,请联系运维人员",new LoginResponseDto());
                }
                else if (string.IsNullOrEmpty(loginRequest.Yhid) || string.IsNullOrEmpty(loginRequest.UserName) || string.IsNullOrEmpty(loginRequest.PassWord))
                {
                    return (false, "无效的登录参数",new LoginResponseDto());
                }
                JArray ArrOperators = JArray.Parse(operators);
                var Operator = (from x in ArrOperators
                                where CommMethod.GetValueOrDefault(x["Yhid"], string.Empty) == loginRequest.Yhid
                              && (CommMethod.GetValueOrDefault(x["EmployeeNum"], string.Empty) == loginRequest.UserName
                              || CommMethod.GetValueOrDefault(x["PhoneNum"], string.Empty) == loginRequest.UserName
                              || CommMethod.GetValueOrDefault(x["EmailNum"], string.Empty) == loginRequest.UserName
                              )
                                select x
                              ).ToList();
                if (Operator.Count == 0)
                {
                    return (false, "无效的身份信息", new LoginResponseDto());
                }
                else if (Operator.Count > 1)
                {
                    return (false, "检测到重复的身份信息，请联系管理处理", new LoginResponseDto());
                }
                else
                {
                    //密码校验
                    var OperatorsPassWordCheck = Operator.Where(x => EncodeAndDecode.SM2Decrypt(x["PassWord"].ToString()) == loginRequest.PassWord).FirstOrDefault();
                    if (OperatorsPassWordCheck == null)
                    {
                        return (false, "密码错误", new LoginResponseDto());
                    }
                    else
                    {
                        JObject? ObjOperator = OperatorsPassWordCheck as JObject;
                        ObjOperator?.Remove("PassWord");
                        UserInfoDto? model = ObjOperator?.ToObject<UserInfoDto>();// JsonConvert.DeserializeObject<UserModel>(ObjOperator.ToString());
                        //校验是否已有有效的登录数据
                        var loginInfoList= _codeContext.Set<LoginInfo>();
                        LoginInfo? loginInfo = loginInfoList.Where(x => x.Yhid == model.Yhid && x.CompanyId == model.CompanyId && x.EmployeeId == model.EmployeeId && x.Exptime > DateTime.Now.AddHours(2)).FirstOrDefault();

                        //获取操作员可以使用的应用
                        List<TopMenuItemDto> menus = await GetOperatorTopMenu(model.Yhid, model.CompanyId, model.OperatorId);
                        

                        if (loginInfo == null)
                        {
                            //获取身份认证令牌
                            JwtToken _token = _jwtService.GetJwtToken(model.OperatorId);
                            model.Token=_token.Token;
                            model.Expires= _token.Expires.ToString();
                            LoginStateSave(loginRequest, model, _token);
                        }
                        else {
                            model.Token = loginInfo.Token;
                            model.Expires = loginInfo.Exptime.ToString() ?? DateTime.MinValue.ToString();
                        }
                        LoginResponseDto _loginResponse = new LoginResponseDto
                        {
                            Code = true,
                            Message = "身份验证通过",
                            UserInfo = model,
                            TopMenus = menus
                        };
                        return (true,"身份验证成功",_loginResponse);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"获取操作员信息 操作员信息初始化到Redis异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"获取操作员信息 操作员信息初始化到Redis异常：{ex.Message.ToString()}"));
                return (true, $"获取操作员信息 操作员信息初始化到Redis异常：{ex.Message.ToString()}", new LoginResponseDto());
            }
        }
        /// <summary>
        /// 登录信息记录
        /// </summary>
        /// <param name="loginRequest"></param>
        /// <param name="model"></param>
        /// <param name="_token"></param>
        private async void LoginStateSave(LoginRequestDto loginRequest, UserInfoDto model, JwtToken _token)
        {
            try
            {
                #region 记录登录信息
                #region 登录信息
                var logininfo = _codeContext.Set<LoginInfo>();
                LoginInfo _logininfo = logininfo.FirstOrDefault(x => x.EmployeeId == model.EmployeeId);

                if (_logininfo == null)
                {
                    _logininfo = new LoginInfo
                    {
                        Yhid = model.Yhid,
                        EmployeeId = model.EmployeeId,
                        LoginFrom = loginRequest.LoginFrom,
                        Token = _token.Token,
                        Exptime = _token.Expires,
                        Remarks = "登录成功",
                        CompanyId = model.CompanyId,
                        Isdelete = 0,
                        LastModified = DateTime.Now
                    };
                    await logininfo.AddAsync(_logininfo);
                }
                else
                {

                    _logininfo.LoginFrom = loginRequest.LoginFrom;
                    _logininfo.Token = _token.Token;
                    _logininfo.Exptime = _token.Expires;
                    // 记录存在，执行更新操作
                    //logininfo.Update(_logininfo);
                }

                #endregion
                #region 登录记录
                var loginrecored = _codeContext.Set<LoginRecord>();
                LoginRecord _record = new LoginRecord
                {
                    Yhid = model.Yhid,
                    LoginId = Guid.NewGuid().ToString("N"),
                    EmployeeId = model.EmployeeId,
                    LoginTime = DateTime.Now,
                    LoginFrom = loginRequest.LoginFrom,
                    IpAddress = loginRequest.IpAddress,
                    AreaInfo = loginRequest.AreaInfo,
                    LoginState = "登录成功",
                    Token = _token.Token,
                    Exptime = _token.Expires,
                    CompanyId = model.CompanyId,
                    Isdelete = 0,
                    LastModified = DateTime.Now
                };
                await loginrecored.AddAsync(_record);
                #endregion
                await _codeContext.SaveChangesAsync();
                #endregion
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "LoginStateSave");
                _exceptionlessClient.SubmitException(new Exception($"LoginStateSave异常：{ex.StackTrace?.ToString()}"));
            }
        }
        /// <summary>
        /// 获取操作员可以使用的顶部菜单
        /// </summary>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        public async Task<List<TopMenuItemDto>> GetOperatorTopMenu(string Yhid, string CompanyId, string OperatorId)
        {
            try { 
            List<TopMenuItemDto> menus = new List<TopMenuItemDto>();
            string? menusinfo = await _redisService.GetAsync("TopMenus");
            if (!string.IsNullOrEmpty(menusinfo))
            {
                menus = JsonConvert.DeserializeObject<List<TopMenuItemDto>>(menusinfo) ?? new List<TopMenuItemDto>();
            }
            else
            {
                menus = await TopMenu();
            }
            bool isSuperUser = await IsThisUser(Yhid, CompanyId, OperatorId, "0");
            if (isSuperUser)
            {
                return menus;
            }
            else
            {
                //获取操作员可以使用的应用
                JArray operatorApps = await OperatorAllRoles(Yhid, CompanyId, OperatorId);
                var applsit = menus.Where(a => operatorApps.Select(o => o["AppId"]?.ToObject<string>()).Contains(a.Id)).ToList();
                return applsit;
            }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetOperatorTopMenu 获取操作员可以使用的顶部菜单异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"GetOperatorTopMenu 获取操作员可以使用的顶部菜单异常：{ex.Message.ToString()}"));
            }
            return null;
        }
        /// <summary>
        /// 判断操作员是否拥有指定的操作权限
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <param name="ObjCode"></param>
        /// <param name="PermissionCode"></param>
        /// <returns></returns>
        public async Task<bool> IsAccess(string Yhid, string CompanyId, string OperatorId, string ObjCode, string PermissionCode)
        {
            try
            {
                var _operator = await _redisService.GetAsync("operatoraccess");
                if (!string.IsNullOrEmpty(_operator))
                {
                    JArray _operators = JArray.Parse(_operator);
                    return _operators.Any(x => (x["RoleId"]?.ToString()=="0"||(x["ObjCode"]?.ToString() == ObjCode && x["PermissionCode"]?.ToString() == PermissionCode))
                    && x["OperatorId"]?.ToString() == OperatorId && x["CompanyId"]?.ToString() == CompanyId && x["Yhid"]?.ToString() == Yhid);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"IsAccess 判断操作员是否拥有指定的操作权限异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"IsAccess 判断操作员是否拥有指定的操作权限异常：{ex.Message.ToString()}"));
            }
            return false;
        }
        /// <summary>
        /// 获取指定操作有拥有的所有操作权限
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        public async Task<JArray> OperatorAccess(string Yhid, string CompanyId, string OperatorId)
        {
            try
            {
                var _operator = await _redisService.GetAsync("Operator");
                if (!string.IsNullOrEmpty(_operator))
                {
                    JArray _operators = JArray.FromObject(_operator);
                    var list = _operators.Where(x => x["OperatorId"]?.ToString() == OperatorId && x["CompanyId"]?.ToString() == CompanyId && x["Yhid"]?.ToString() == Yhid).Distinct().ToList();
                    return JArray.FromObject(list);
                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"OperatorAccess 获取指定操作有拥有的所有操作权限异常：{ex.Message.ToString()}");
                _exceptionlessClient.SubmitException(new Exception($"OperatorAccess 获取指定操作有拥有的所有操作权限异常：{ex.Message.ToString()}"));
            }
            return new JArray();
        }
        /// <summary>
        /// 获取指定操作员拥有的所有角色
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <returns></returns>
        private async Task<JArray> OperatorAllRoles(string Yhid, string CompanyId, string OperatorId)
        {
            var roler = await _redisService.GetAsync("roler");
            if (!string.IsNullOrEmpty(roler))
            {
                JArray rolers = JArray.FromObject(roler);
                var list = rolers.Where(x => x["OperatorId"]?.ToString() == OperatorId && x["CompanyId"]?.ToString() == CompanyId && x["Yhid"]?.ToString() == Yhid).Distinct().ToList();
                return JArray.FromObject(list);
            }
            return new JArray();
        }
        /// <summary>
        /// 获取顶部菜单
        /// </summary>
        /// <returns></returns>
        private async Task<List<TopMenuItemDto>> TopMenu()
        {
            List<TopMenuItemDto> items = new List<TopMenuItemDto>();
            var menu = _codeContext.Set<AppsCode>();
            var menus = await menu.Where(i => i.Isdelete == 0).OrderBy(x => x.AppId).ToListAsync();
            foreach (AppsCode model in menus)
            {
                items.Add(new TopMenuItemDto
                {
                    Id = model.AppId,
                    Name = model.AppName,
                    Tag = model.AppCode,
                    Description = model.RoleDescription
                });
            }
            return items;
        }
        /// <summary>
        /// 判断当前操作员是否拥有指定角色的角色信息
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        private async Task<bool> IsThisUser(string Yhid, string CompanyId, string OperatorId, string RoleId)
        {
            var roler = await _redisService.GetAsync("roler");
            if (!string.IsNullOrEmpty(roler))
            {
                JArray rolers = JArray.Parse(roler);
                return rolers.Any(x => x["RoleId"]?.ToString() == RoleId && x["OperatorId"]?.ToString() == OperatorId && x["CompanyId"]?.ToString() == CompanyId && x["Yhid"]?.ToString() == Yhid);
            }
            return false;
        }
        /// <summary>
        /// 密码更新
        /// </summary>
        /// <param name="Yhid"></param>
        /// <param name="CompanyId"></param>
        /// <param name="OperatorId"></param>
        /// <param name="OldPassWord"></param>
        /// <param name="NewPassWord"></param>
        /// <returns></returns>
        public async Task<Operators> UpdatePassWord(string Yhid, string CompanyId, string OperatorId, string OldPassWord, string NewPassWord)
        {
            JObject Obj = await GetOperatorObj(Yhid, OperatorId);
            if (Obj != null)
            {
                string password = EncodeAndDecode.SM2Decrypt(Obj["PassWord"].ToString());
                if (OldPassWord != password)
                {
                    throw new Exception("输入的旧密码有误!");
                }
                else
                {
                    var operators = _codeContext.Set<Operators>();
                   Operators _Operator = operators.ToList().FirstOrDefault(x => x.OperatorId == OperatorId&&x.CompanyId==CompanyId&&x.Yhid==Yhid);
                    if (_Operator != null)
                    {
                        _Operator.PassWord = EncodeAndDecode.SM2Encrypt(NewPassWord);
                        _Operator.PassWordExpTime = DateTime.Now.AddMonths(3);
                        //operators.Update(_Operator);
                        await _codeContext.SaveChangesAsync();
                        return _Operator;
                    }
                    else
                    {
                        throw new Exception("未找到有效的操作员对象");
                    }
                }
            }
            else
            {
                throw new Exception("无效的身份信息");
            }
        }
    }
}
