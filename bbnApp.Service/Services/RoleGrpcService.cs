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
    public class RoleGrpcService : RoleGrpc.RoleGrpcBase
    {
        private readonly IRoleService roleService;
        private readonly IMapper _mapper;

        public RoleGrpcService(IRoleService roleService, IMapper mapper)
        {
            this.roleService = roleService;
            _mapper = mapper;
        }

        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<RoleListResponse> RoleListLoad(RoleListRequest request, ServerCallContext context)
        {
            RoleListResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list) = await roleService.RoleListLoad(_mapper.Map<RoleListRequestDto>(request),user);
                    response = new RoleListResponse
                    {
                        Code = code,
                        Message = msg,
                    };
                    response.Data.AddRange(_mapper.Map<List<RoleItem>>(list));
                }
                else
                {
                    response = new RoleListResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new RoleListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<RoleInfoResponse> RoleInfo(RoleInfoRequest request, ServerCallContext context)
        {
            RoleInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg,item, list) = await roleService.RoleInfo(_mapper.Map<RoleInfoRequestDto>(request), user);
                    response = new RoleInfoResponse
                    {
                        Code = code,
                        Message = msg,
                        Role= _mapper.Map<RoleItem>(item)
                    };
                    response.RoleApps.AddRange(_mapper.Map<List<RoleApps>>(list));
                }
                else
                {
                    response = new RoleInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new RoleInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 角色应用列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<RoleAppListResponse> RoleAppListLoad(RoleAppListRequest request, ServerCallContext context)
        {
            RoleAppListResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg,list) = await roleService.RoleAppListLoad(_mapper.Map<RoleAppListRequestDto>(request), user);
                    response = new RoleAppListResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Data.AddRange(_mapper.Map<List<RoleApps>>(list));
                }
                else
                {
                    response = new RoleAppListResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new RoleAppListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 角色应用列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<RoleSaveResponse> RolePost(RoleSaveRequest request, ServerCallContext context)
        {
            RoleSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, item) = await roleService.RolePost(_mapper.Map<RoleSaveRequestDto>(request), user);
                    response = new RoleSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        Role = _mapper.Map <RoleItem>(item)
                    };
                }
                else
                {
                    response = new RoleSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new RoleSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 角色应用列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<RoleStateResponse> RoleState(RoleStateRequest request, ServerCallContext context)
        {
            RoleStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, item) = await roleService.RoleState(_mapper.Map<RoleStateRequestDto>(request), user);
                    response = new RoleStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Data = _mapper.Map<RoleItem>(item)
                    };
                }
                else
                {
                    response = new RoleStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new RoleStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
