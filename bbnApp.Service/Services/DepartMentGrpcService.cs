using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class DepartMentGrpcService : DepartMentGrpc.DepartMentGrpcBase
    {
        private readonly IOperatorService _operatorService;
        private readonly IDepartMentService _departMentService;
        private readonly IMapper _mapper;
        public DepartMentGrpcService(IOperatorService operatorService, IDepartMentService departMentService, IMapper mapper)
        {
            _operatorService = operatorService;
            _departMentService = departMentService;
            _mapper = mapper;
        }

        /// <summary>
        /// 部门树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DepartMentTreeResponse> GetDepartMentTree(DepartMentTreeRequest request, ServerCallContext context)
        {
            DepartMentTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    // 使用 AutoMapper 映射请求
                    var requestDto = _mapper.Map<DepartMentTreeRequestDto>(request);
                    // 调用应用程序中的服务
                    var items = await _departMentService.GetDepartMentTree(requestDto, user);
                    var companyItems = _mapper.Map<List<DepartMentTreeItem>>(items);
                    response = new DepartMentTreeResponse
                    {
                        Code = true,
                        Message = "部门信息初始化完成"
                    };
                    response.Items.AddRange(companyItems);
                }
                else
                {
                    response = new DepartMentTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DepartMentTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DepartMentInfoResponse> GetDepartMentInfo(DepartMentInfoRequest request, ServerCallContext context)
        {
            DepartMentInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    // 使用 AutoMapper 映射请求
                    var requestDto = _mapper.Map<DepartMentInfoRequestDto>(request);
                    // 调用应用程序中的服务
                    var items = await _departMentService.GetDepartMentInfo(requestDto.DepartMentId, requestDto.CompanyId, user);
                    response = new DepartMentInfoResponse
                    {
                        Code = items.Item1,
                        Message = items.Item2,
                        Item = _mapper.Map<DepartMentInfo>(items.Item3)
                    };
                }
                else
                {
                    response = new DepartMentInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DepartMentInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 部门信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DepartMentSaveResponse> SaveDepartMent(DepartMentSaveRequest request, ServerCallContext context)
        {
            DepartMentSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    // 使用 AutoMapper 映射请求
                    var requestDto = _mapper.Map<DepartMentSaveRequestDto>(request);
                    // 调用应用程序中的服务
                    var items = await _departMentService.SaveDepartMent(requestDto.Item, user);
                    response = new DepartMentSaveResponse
                    {
                        Code = items.Item1,
                        Message = items.Item2,
                        Item = _mapper.Map<DepartMentInfo>(items.Item3)
                    };
                }
                else
                {
                    response = new DepartMentSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DepartMentSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 部门信息变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DepartMentStateResponse> StateDepartMent(DepartMentStateRequest request, ServerCallContext context)
        {
            DepartMentStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    // 使用 AutoMapper 映射请求
                    var requestDto = _mapper.Map<DepartMentStateRequestDto>(request);
                    // 调用应用程序中的服务
                    var items = await _departMentService.StateDepartMent(requestDto.Type, requestDto.DepartMentId, requestDto.CompanyId, requestDto.Reason, user);
                    response = new DepartMentStateResponse
                    {
                        Code = items.Item1,
                        Message = items.Item2,
                        Item = _mapper.Map<DepartMentInfo>(items.Item3)
                    };
                }
                else
                {
                    response = new DepartMentStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DepartMentStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
    }
}
