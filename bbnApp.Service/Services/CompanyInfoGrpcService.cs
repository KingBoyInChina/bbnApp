using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.Services.CODE;
using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class CompanyInfoGrpcService:CompanyGrpcService.CompanyGrpcServiceBase
    {
        private readonly IOperatorService _operatorService;
        private readonly ICompanyService _companyService;
        private readonly IMapper _mapper;
        public CompanyInfoGrpcService(IOperatorService operatorService, ICompanyService companyService, IMapper mapper)
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
        public override async Task<CompanyResponse> GetCompanyItems(CompanyRequest request, ServerCallContext context)
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
                    Code = true,
                    Message = "机构信息初始化完成"
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
        /// 公司树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<CompanyTreeResponse> GetCompanyTree(CompanyTreeRequest request, ServerCallContext context)
        {
            CompanyTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    // 使用 AutoMapper 映射请求
                    var companyRequestDto = _mapper.Map<CompanyTreeRequestDto>(request);
                    // 调用应用程序中的服务
                    var items = await _companyService.GetCompanyTree(companyRequestDto, user);
                    var companyItems = _mapper.Map<List<CompanyTreeItem>>(items);
                    response = new CompanyTreeResponse
                    {
                        Code = true,
                        Message = "机构信息初始化完成"
                    };
                    response.Items.AddRange(companyItems);
                }
                else
                {
                    response = new CompanyTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new CompanyTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
        [Authorize]
        public override async Task<CompanyInfoResponse> GetCompanyInfo(CompanyInfoRequest request, ServerCallContext context)
        {
            CompanyInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    // 使用 AutoMapper 映射请求
                    var companyRequestDto = _mapper.Map<CompanyInfoRequestDto>(request);
                    // 调用应用程序中的服务
                    var items = await _companyService.GetCompanyInfo(companyRequestDto.CompanyId, user);
                    response = new CompanyInfoResponse
                    {
                        Code = items.Item1,
                        Message = items.Item2,
                        Item= _mapper.Map<CompanyInfo>(items.Item3)
                    };
                }
                else
                {
                    response = new CompanyInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new CompanyInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 公司信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<CompanySaveResponse> SaveCompanyInfo(CompanySaveRequest request, ServerCallContext context)
        {
            CompanySaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    // 使用 AutoMapper 映射请求
                    var companyRequestDto = _mapper.Map<CompanySaveRequestDto>(request);
                    // 调用应用程序中的服务
                    var items = await _companyService.SaveCompanyInfo(companyRequestDto.Item, user);
                    response = new CompanySaveResponse
                    {
                        Code = items.Item1,
                        Message = items.Item2,
                        Item = _mapper.Map<CompanyInfo>(items.Item3)
                    };
                }
                else
                {
                    response = new CompanySaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new CompanySaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 公司信息变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<CompanyStateResponse> StateCompanyInfo(CompanyStateRequest request, ServerCallContext context)
        {
            CompanyStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    // 使用 AutoMapper 映射请求
                    var companyRequestDto = _mapper.Map<CompanyStateRequestDto>(request);
                    // 调用应用程序中的服务
                    var items = await _companyService.StateCompanyInfo(companyRequestDto.Type, companyRequestDto.CompanyId, companyRequestDto.Reason, user);
                    response = new CompanyStateResponse
                    {
                        Code = items.Item1,
                        Message = items.Item2,
                        Item = _mapper.Map<CompanyInfo>(items.Item3)
                    };
                }
                else
                {
                    response = new CompanyStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new CompanyStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
    }
}
