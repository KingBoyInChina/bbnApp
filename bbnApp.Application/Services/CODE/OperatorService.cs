using bbnApp.Application.DTOs.LoginDto;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.IServices.IJWT;
using bbnApp.Common.Models;
using bbnApp.Core;
using bbnApp.Domain.Entities.Code;
using bbnApp.Domain.Entities.User;
using bbnApp.Domain.Entities.UserLogin;
using bbnApp.DTOs.CodeDto;
using bbnApp.Infrastructure.Data;
using bbnApp.Share;
using Dapper;
using Exceptionless;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private readonly IApplicationDbContext dbContext;
        /// <summary>
        /// 
        /// </summary>
        private readonly IApplicationDbCodeContext dbCodeContext;
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
            IApplicationDbContext dbContext,
            IApplicationDbCodeContext codeContext,
            ILogger<OperatorService> logger,
            ExceptionlessClient exceptionlessClient
            ) 
        {
            _dapperRepository = dapperRepository;
            _redisService = redisService;
            _jwtService = jwtService;
            this.dbContext = dbContext;
            dbCodeContext = codeContext;
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
                        var loginInfoList= dbContext.Set<LoginInfo>();
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
                var logininfo = dbContext.Set<LoginInfo>();
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
                var loginrecored = dbContext.Set<LoginRecord>();
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
                await dbContext.SaveChangesAsync();
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
            var menu = dbContext.Set<AppsCode>();
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
                    var operators = dbContext.Set<Operators>();
                   Operators _Operator = operators.ToList().FirstOrDefault(x => x.OperatorId == OperatorId&&x.CompanyId==CompanyId&&x.Yhid==Yhid);
                    if (_Operator != null)
                    {
                        _Operator.PassWord = EncodeAndDecode.SM2Encrypt(NewPassWord);
                        _Operator.PassWordExpTime = DateTime.Now.AddMonths(3);
                        //operators.Update(_Operator);
                        await dbContext.SaveChangesAsync();
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
        #region 操作员权限管理
        /// <summary>
        /// 操作员加载
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, List<OperatorItemDto>)> OperatorListLoad(OperatorListRequestDto request,UserModel user)
        {
            try
            {
                if (await IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "operators", "browse"))
                {
                    StringBuilder SQL = new StringBuilder($"select * from {StaticModel.DbName.bbn}.vw_operator_permission where 1=1 ");
                    #region 过滤条件
                    string CompanyId = string.IsNullOrEmpty(request.CompanyId) ? user.CompanyId : request.CompanyId;
                    var param = new DynamicParameters { };

                    SQL.Append(" and CompanyId =@CompanyId");
                    param.Add("CompanyId", $"{CompanyId}");

                    SQL.Append(" and PositionLeve >=@PositionLeve");
                    param.Add("PositionLeve", $"{user.PositionLeve}");


                    if (!string.IsNullOrEmpty(request.EmployeeNum))
                    {
                        SQL.Append(" and EmployeeNum like @EmployeeNum");
                        param.Add("EmployeeNum", $"%{request.EmployeeNum}%");
                    }
                    if (!string.IsNullOrEmpty(request.EmployeeNum))
                    {
                        SQL.Append(" and EmployeeNum like @EmployeeNum");
                        param.Add("EmployeeNum", $"%{request.EmployeeNum}%");
                    }
                    if (!string.IsNullOrEmpty(request.DepartMentId))
                    {
                        SQL.Append(" and DepartMentId = @DepartMentId");
                        param.Add("DepartMentId", $"{request.DepartMentId}");
                    }
                    #endregion
                    //获取操作员信息
                    var list =await _dapperRepository.QueryAsync<OperatorItemDto>(SQL.ToString(),param);
                    List<OperatorItemDto> operators = new List<OperatorItemDto>();
                    //获取角色信息
                    var roles = dbContext.Set<RoleManagment>().Where(x => x.CompanyId == CompanyId && x.Yhid == user.Yhid && x.Isdelete == 0 && x.IsLock == 0).OrderBy(x => x.RoleId).ToList();
                    //操作员角色信息
                    var permissons=dbContext.Set<PermissionAssignment>().Where(x => x.CompanyId == CompanyId && x.Yhid == user.Yhid && x.Isdelete == 0 && x.IsLock == 0).OrderBy(x => x.RoleId).ToList();
                    foreach (var item in list)
                    {
                        OperatorItemDto node = new OperatorItemDto {
                            IdxNum = item.IdxNum,
                            Yhid = item.Yhid,
                            EmployeeId = item.EmployeeId,
                            PEmployeeId = item.PEmployeeId,
                            EmployeeName = item.EmployeeName,
                            EmployeeNum = item.EmployeeNum,
                            DepartMentId = CommMethod.GetValueOrDefault(item.DepartMentId, ""),
                            Position = CommMethod.GetValueOrDefault(item.Position, ""),
                            PositionLeve = item.PositionLeve,
                            IsOperator = CommMethod.GetValueOrDefault(item.IsOperator, false),
                            OperatorId =CommMethod.GetValueOrDefault(item.OperatorId,""),
                            PassWord =string.IsNullOrEmpty(item.PassWord)?string.Empty: EncodeAndDecode.SM2Decrypt(item.PassWord) ,
                            PassWordExpTime = CommMethod.GetValueOrDefault(item.PassWordExpTime, ""),
                            IsLock = item.IsLock,
                            CompanyId = CommMethod.GetValueOrDefault(item.CompanyId, ""),
                            OperatorRoles =await OperatorRoleList(item.OperatorId,item.CompanyId,item.Yhid)
                        };
                        operators.Add(node);
                    }
                    return (true,"数据读取成功", operators);
                }
                return (false,"无权进行操作",new List<OperatorItemDto>());
            }
            catch(Exception ex)
            {
                return (false,$"操作员加载异常：{ex.Message.ToString()}",new List<OperatorItemDto>());
            }
        }
        /// <summary>
        /// 操作员角色列表
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private async Task<List<OperatorRoleDto>> OperatorRoleList(string OperatorId,string CompanyId,string Yhid)
        {
            List<OperatorRoleDto> result = new List<OperatorRoleDto>();
            
            var list = await _dapperRepository.QueryAsync<OperatorRoleDto>($"CALL {StaticModel.DbName.bbn}.proc_operator_roles(@Yhid,@CompanyId,@OperatorId) ", new { Yhid, CompanyId, OperatorId });
            foreach(var item in list)
            {
                result.Add(item);
            }
            return result;
        }
        /// <summary>
        /// 权限分配
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string, OperatorItemDto)> OperatorSave(OperatorSaveRequestDto request,UserModel user)
        {
            try
            {
                if (await IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "operators", "permit"))
                {
                    var OperatorItem = request.Item;
                    //系统配置
                    var EFSetting = dbCodeContext.Set<AppSettings>();
                    //获取操作员信息
                    var EFObj = dbContext.Set<Operators>();
                    var EFPermissionObj = dbContext.Set<PermissionAssignment>();
                    Operators? model = await EFObj.FirstOrDefaultAsync(x => x.OperatorId == OperatorItem.OperatorId && x.CompanyId == OperatorItem.CompanyId && x.Yhid == user.Yhid);
                    bool badd = false;
                    if (model == null)
                    {
                        model = new Operators();
                        model.Yhid= user.Yhid;
                        model.OperatorId= OperatorItem.EmployeeId+DateTime.Now.ToString("yyMMddHHmmssfff");
                        model.EmployeeId = OperatorItem.EmployeeId;
                        model.CompanyId= OperatorItem.CompanyId;
                        model.Isdelete = 0;
                        model.IsLock = 0;
                        badd = true;
                    }
                    #region 写操作员信息
                    int day = 3650; //默认密码有效期3650天
                    var settinginfo = await EFSetting.FirstOrDefaultAsync(x => x.Yhid == user.Yhid && x.SettingCode == "PassWordExpri" && x.Yhid == user.Yhid);
                    if (settinginfo != null)
                    {
                        day = string.IsNullOrEmpty(settinginfo.NowValue) || settinginfo.NowValue == "0" ? 3650 : Convert.ToInt32(settinginfo.NowValue);
                    }
                    model.PassWord = EncodeAndDecode.SM2Encrypt(OperatorItem.PassWord);
                    model.PassWordExpTime = DateTime.Now.AddDays(day);
                    model.LastModified = DateTime.Now;
                    #endregion
                    if (badd)
                    {
                        await EFObj.AddAsync(model);
                    }
                    #region 写操作员权限
                    foreach (var item in OperatorItem.OperatorRoles)
                    {
                        if (item.RoleId != "0")//不能分配超级管理员角色
                        {
                            var rolepermission = EFPermissionObj.FirstOrDefault(x => x.RoleId == item.RoleId && x.OperatorId == model.OperatorId && x.CompanyId == model.CompanyId);
                            if (item.IsChecked)
                            {
                                bool bpermission = false;
                                if (rolepermission == null)
                                {
                                    rolepermission = new PermissionAssignment();
                                    rolepermission.Yhid = user.Yhid;
                                    rolepermission.PermissionId = Guid.NewGuid().ToString("N");
                                    rolepermission.OperatorId = model.OperatorId;
                                    rolepermission.RoleId = item.RoleId;
                                    rolepermission.IsLock = 0;
                                    rolepermission.CompanyId = item.CompanyId;
                                    bpermission = true;
                                }
                                rolepermission.Isdelete = 0;
                                rolepermission.LastModified = DateTime.Now;
                                if (bpermission)
                                {
                                    await EFPermissionObj.AddAsync(rolepermission);
                                }
                            }
                            else if (rolepermission != null && rolepermission.Isdelete == 0)
                            {
                                rolepermission.Isdelete = 1;
                                rolepermission.LastModified = DateTime.Now;
                            }
                        }
                    }
                    #endregion
                    await dbContext.SaveChangesAsync();
                    OperatorItem.OperatorId = model.OperatorId;
                    return (true,"权限分配完成", OperatorItem);
                }
                return (false,"无权进行操作",new OperatorItemDto());
            }
            catch(Exception ex)
            {
                return (false, $"操作员权限分配异常：{ex.Message.ToString()}", new OperatorItemDto());
            }
        }
        /// <summary>
        /// 操作员状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<(bool,string,OperatorItemDto)> OperatorState(OperatorStateRequestDto request,UserModel user)
        {
            try
            {
                if (await IsAccess(user.Yhid, user.CompanyId, user.OperatorId, "operators", "permit"))
                {
                    var EFObj = dbContext.Set<Operators>();
                    var EFPermissionObj = dbContext.Set<PermissionAssignment>();
                    Operators? model = await EFObj.FirstOrDefaultAsync(x => x.OperatorId == request.OperatorId && x.Yhid == user.Yhid);
                    if (model == null)
                    {
                        return (false,"无效的操作员信息",new OperatorItemDto());
                    }
                    
                    if (request.Type == "IsLock")
                    {
                        #region 锁定
                        model.IsLock = model.IsLock==0 ?Convert.ToByte(1) : Convert.ToByte(0);
                        model.LastModified = DateTime.Now;
                        await dbContext.SaveChangesAsync();
                        return (true,"操作员状态变更完成", await GetOperatorDto(model));
                        #endregion
                    }
                    else if (request.Type == "IsDelete")
                    {
                        #region 删除
                        model.Isdelete = 0;
                        model.LastModified = DateTime.Now;
                        //权限同步删除
                        var list=EFPermissionObj.Where(x => x.OperatorId == request.OperatorId && x.Yhid == user.Yhid && x.CompanyId == model.CompanyId).ToList();
                        if (list.Count > 0)
                        {
                            foreach (var item in list)
                            {
                                item.Isdelete = 1;
                                item.LastModified = DateTime.Now;
                            }
                        }
                        await dbContext.SaveChangesAsync();
                        return (true,"操作员删除完成",new OperatorItemDto());
                        #endregion
                    }
                }
                return (false, "无权进行操作", new OperatorItemDto());
            }
            catch(Exception ex)
            {
                return (false, $"操作员状态变更异常：{ex.Message.ToString()}", new OperatorItemDto());
            }
        }
        /// <summary>
        /// model 转换为操作员DTO
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<OperatorItemDto> GetOperatorDto(Operators model)
        {
            StringBuilder SQL = new StringBuilder($"select * from {StaticModel.DbName.bbn}.vw_operator_permission where 1=1 ");
            #region 过滤条件
            string CompanyId = model.CompanyId;
            string OperatorId = model.OperatorId;
            string EmployeeId = model.EmployeeId;
            var param = new DynamicParameters { };

            SQL.Append(" and Yhid =@Yhid");
            param.Add("Yhid", $"{model.Yhid}");

            SQL.Append(" and CompanyId =@CompanyId");
            param.Add("CompanyId", $"{CompanyId}");

            SQL.Append(" and OperatorId =@OperatorId");
            param.Add("OperatorId", $"{OperatorId}");

            SQL.Append(" and EmployeeId =@EmployeeId");
            param.Add("EmployeeId", $"{EmployeeId}");

            #endregion
            OperatorItemDto node = new OperatorItemDto();
            //获取操作员信息
            var list = await _dapperRepository.QueryAsync<OperatorItemDto>(SQL.ToString(), param);
            foreach (var item in list)
            {
                node = new OperatorItemDto
                {
                    IdxNum = item.IdxNum,
                    Yhid = item.Yhid,
                    EmployeeId = item.EmployeeId,
                    PEmployeeId = item.PEmployeeId,
                    EmployeeName = item.EmployeeName,
                    EmployeeNum = item.EmployeeNum,
                    DepartMentId = CommMethod.GetValueOrDefault(item.DepartMentId, ""),
                    Position = CommMethod.GetValueOrDefault(item.Position, ""),
                    PositionLeve = item.PositionLeve,
                    IsOperator = CommMethod.GetValueOrDefault(item.IsOperator, false),
                    OperatorId = CommMethod.GetValueOrDefault(item.OperatorId, ""),
                    PassWord = string.IsNullOrEmpty(item.PassWord) ? string.Empty : EncodeAndDecode.SM2Decrypt(item.PassWord),
                    PassWordExpTime = CommMethod.GetValueOrDefault(item.PassWordExpTime, ""),
                    IsLock = item.IsLock,
                    CompanyId = CommMethod.GetValueOrDefault(item.CompanyId, ""),
                    OperatorRoles = await OperatorRoleList(item.OperatorId, item.CompanyId, item.Yhid)
                };
                break;
            }
            return node;
        }
        #endregion
    }
}
