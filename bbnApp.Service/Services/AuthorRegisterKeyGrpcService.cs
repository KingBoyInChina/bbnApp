using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class AuthorRegisterKeyGrpcService : ReigisterKeyGrpcService.ReigisterKeyGrpcServiceBase
    {
        private readonly IAuthorRegisterKeyService _authorRegisterService;
        private readonly IMapper _mapper;
        public AuthorRegisterKeyGrpcService(IAuthorRegisterKeyService authorRegisterService, IMapper mapper)
        {
            _authorRegisterService = authorRegisterService;
            _mapper = mapper;
        }
        /// <summary>
        /// 机构清单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<CompanyAuthorRegistrKeySearchResponse> CompanyAuthorRegistrKeySearch(CompanyAuthorRegistrKeySearchRequest request, ServerCallContext context)
        {
            CompanyAuthorRegistrKeySearchResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list) = await _authorRegisterService.CompanyAuthorRegistrKeySearch(request.AreaId, user);
                    response = new CompanyAuthorRegistrKeySearchResponse
                    {
                        Code = code,
                        Message = msg,
                    };
                    response.Items.AddRange(_mapper.Map<List<CompanyAuthorRegistrKeyItem>>(list));
                }
                else
                {
                    throw new Exception("无效操作员身份信息");
                }
            }
            catch (Exception ex)
            {
                response = new CompanyAuthorRegistrKeySearchResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 注册密钥查询-分页
        /// </summary>
        /// <param name="request"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AuthorRegisterKeySearchResponse> AuthorRegisterKeySearch(AuthorRegisterKeySearchRequest request, ServerCallContext context)
        {
            AuthorRegisterKeySearchResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list,total) = await _authorRegisterService.AuthorRegisterKeySearch(_mapper.Map<AuthorRegisterKeySearchRequestDto>(request),user);
                    response = new AuthorRegisterKeySearchResponse
                    {
                        Code = code,
                        Message = msg,
                        Total = total
                    };
                    response.Items.AddRange(_mapper.Map<List<AuthorRegisterKeyItem>>(list));
                }
                else
                {
                    throw new Exception("无效操作员身份信息");
                }
            }
            catch (Exception ex)
            {
                response = new AuthorRegisterKeySearchResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 注册密钥查询-不分页
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AuthorRegisterKeyListResponse> AuthorRegisterKeyList(AuthorRegisterKeyListRequest request, ServerCallContext context)
        {
            AuthorRegisterKeyListResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list) = await _authorRegisterService.AuthorRegisterKeyList(_mapper.Map<AuthorRegisterKeyListRequestDto>(request), user);
                    response = new AuthorRegisterKeyListResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Items.AddRange(_mapper.Map<List<AuthorRegisterKeyItem>>(list));
                }
                else
                {
                    throw new Exception("无效操作员身份信息");
                }
            }
            catch (Exception ex)
            {
                response = new AuthorRegisterKeyListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 新注册密钥
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AuthorRegisterKeyAddResponse> AuthorRegisterKeyAdd(AuthorRegisterKeyAddRequest request, ServerCallContext context)
        {
            AuthorRegisterKeyAddResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var requestDto = _mapper.Map<AuthorRegisterKeyAddRequestDto>(request);
                    var (code, msg, data) = await _authorRegisterService.AuthorRegisterKeyAdd(requestDto.Item, user);
                    response = new AuthorRegisterKeyAddResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<AuthorRegisterKeyItem>(data)
                    };
                }
                else
                {
                    throw new Exception("无效操作员身份信息");
                }
            }
            catch (Exception ex)
            {
                response = new AuthorRegisterKeyAddResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 密钥状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AuthorRegisterKeyStateResponse> AuthorRegisterKeyState(AuthorRegisterKeyStateRequest request, ServerCallContext context)
        {
            AuthorRegisterKeyStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, data) = await _authorRegisterService.AuthorRegisterKeyState(_mapper.Map<AuthorRegisterKeyStateRequestDto>(request), user);
                    response = new AuthorRegisterKeyStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<AuthorRegisterKeyItem>(data)
                    };
                }
                else
                {
                    throw new Exception("无效操作员身份信息");
                }
            }
            catch (Exception ex)
            {
                response = new AuthorRegisterKeyStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 注册密钥关联设备信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public override async Task<AuthorReginsterKeyClientListResponse> AuthorRegisterClients(AuthorReginsterKeyClientListRequest request, ServerCallContext context)
        {
            AuthorReginsterKeyClientListResponse? response = null;
            try
            {
                var data = await _authorRegisterService.AuthorRegisterKeyClients();
                response = new AuthorReginsterKeyClientListResponse
                {
                    Code = true,
                    Message = "用户密钥信息读取完成"
                };
                response.Items.AddRange(_mapper.Map<List<AuthorReginsterKeyClient>>(data));
            }
            catch (Exception ex)
            {
                response = new AuthorReginsterKeyClientListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 密钥状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AuthorReginsterKeyInfoResponse> AuthorRegisterInfo(AuthorReginsterKeyInfoRequest request, ServerCallContext context)
        {
            AuthorReginsterKeyInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, data) =await _authorRegisterService.GetAuthorRegisterKey(request.Yhid,request.CompanyId,request.OperatorId);
                    response = new AuthorReginsterKeyInfoResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<AuthorReginsterKeyClient>(data)
                    };
                }
                else
                {
                    throw new Exception("无效操作员身份信息");
                }
            }
            catch (Exception ex)
            {
                response = new AuthorReginsterKeyInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
