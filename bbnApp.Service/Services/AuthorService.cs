using AutoMapper;
using bbnApp.Application.DTOs.LoginDto;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Domain.Entities.User;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using bbnApp.Common.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace bbnApp.Service.Services
{
    public class AuthorService : Author.AuthorBase
    {
        private readonly IOperatorService _operatorService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        public AuthorService(IOperatorService operatorService, ICompanyService companyService, IMapper mapper)
        {
            _operatorService = operatorService;
            _companyService = companyService;
            _mapper = mapper;
        }
        /// <summary>
        /// 初始化机构信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override async Task<CompanyResponse> GetCompanyItems(CompanyRequest request,ServerCallContext context)
        {
            CompanyResponse? response = null;
            // 使用 AutoMapper 映射请求
            var companyRequestDto = _mapper.Map<CompanyRequestDto>(request);
            try
            {
                // 调用应用程序中的服务
                var items = await _companyService.GetCompanyItems(companyRequestDto);
                //使用 AutoMapper 映射响应 报错，不知道是不是因为对象复合嵌套问题
                var companyItems = _mapper.Map<List<CompanyItem>>(items);
                response = new CompanyResponse
                { 
                    Code=true,
                    Message="机构信息初始化完成"
                };
                response.CompanyItems.AddRange(companyItems);


            }
            catch (Exception ex)
            {
                response = new CompanyResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override async Task<LoginResponse> Login(LoginRequest request, ServerCallContext contextc)
        {
            LoginResponse? response =null;
            // 使用 AutoMapper 映射请求
            var loginRequestDto = _mapper.Map<LoginRequestDto>(request);
            try
            {
                // 调用应用程序中的服务
                var loginResponseDto = await _operatorService.OperatorLogin(loginRequestDto);

                LoginResponse u = _mapper.Map<LoginResponse>(loginResponseDto);
                response = new LoginResponse
                {
                    Code = loginResponseDto.Code,
                    Message = loginResponseDto.Message,
                    UserInfo =JsonConvert.DeserializeObject<UserInfo>(JsonConvert.SerializeObject(loginResponseDto.UserInfo))
                };
                response.TopMenus.AddRange(JsonConvert.DeserializeObject<List<TopMenuItem>>(JsonConvert.SerializeObject(loginResponseDto.TopMenus)));
                // 使用 AutoMapper 映射响应
                //var response = _mapper.Map<LoginResponse>(loginResponseDto);
            }
            catch(Exception ex)
            {
                response = new LoginResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 密码变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<PassWordResponse> UpdatePassWord(PassWordRequest request, ServerCallContext context)
        {
            PassWordResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    Operators _operator =await _operatorService.UpdatePassWord(user.Yhid,user.CompanyId,user.OperatorId,request.OldPassWord,request.NewPassWord);
                    if (_operator!=null)
                    {
                        response = new PassWordResponse
                        {
                            Code = true,
                            Message = "密码修改成功",
                            PassWordExpTime =  _operator.PassWordExpTime?.ToString("yyyy-MM-dd") ?? string.Empty
                        };
                    }
                }
                else
                {
                    throw new Exception("无效操作员身份信息");
                }
            }
            catch(Exception ex)
            {
                response = new PassWordResponse
                {
                    Code = false,
                    Message = ex.Message,
                    PassWordExpTime = string.Empty
                };
            }
            return response;
        }
    }
}
