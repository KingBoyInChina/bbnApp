using AutoMapper;
using bbnApp.Application.IServices.IBusiness;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.Services.INIT;
using bbnApp.Common.Models;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CommDto;
using bbnApp.Protos;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace bbnApp.Service.Services
{
    public class UserGrpcService : UserInformationGrpc.UserInformationGrpcBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public UserGrpcService(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取用户树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserInformationTreeResponse> UserInformationTree(UserInformationTreeRequest request, ServerCallContext context)
        {
            UserInformationTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserInformationTreeRequestDto data = _mapper.Map<UserInformationTreeRequestDto>(request);
                    var (code, msg, list) = await _userService.UserInformationTree(data,user);
                    response = new UserInformationTreeResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Items.AddRange(_mapper.Map<List<UserTreeItem>>(list));
                }
                else
                {
                    response = new UserInformationTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserInformationTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 获取用户清单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserInformationListResponse> UserInformationList(UserInformationListRequest request, ServerCallContext context)
        {
            UserInformationListResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserInformationListRequestDto data = _mapper.Map<UserInformationListRequestDto>(request);
                    var (code, msg, list) = await _userService.UserInformationList(data, user);
                    response = new UserInformationListResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Items.AddRange(_mapper.Map<List<UserInformation>>(list));
                }
                else
                {
                    response = new UserInformationListResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserInformationListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="request"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserInformationLoadResponse> UserInformationLoad(UserInformationLoadRequest request, ServerCallContext context)
        {
            UserInformationLoadResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserInformationLoadRequestDto data = _mapper.Map<UserInformationLoadRequestDto>(request);
                    var (code, msg, item,contacts,abbs) = await _userService.UserInformationLoad(data, user);
                    response = new UserInformationLoadResponse
                    {
                        Code = code,
                        Message = msg,
                        User= _mapper.Map<UserInformation>(item)
                    };
                    response.Contacts.AddRange(_mapper.Map<List<UserContact>>(contacts));
                    response.Aabs.AddRange(_mapper.Map<List<UserAabInformation>>(abbs));
                }
                else
                {
                    response = new UserInformationLoadResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserInformationLoadResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 用户信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserInformationSaveResponse> UserInformationSave(UserInformationSaveRequest request, ServerCallContext context)
        {
            UserInformationSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserInformationSaveRequestDto data = _mapper.Map<UserInformationSaveRequestDto>(request);
                    var (code, msg, item, contacts, abbs) = await _userService.UserInformationSave(data, user);
                    response = new UserInformationSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        User = _mapper.Map<UserInformation>(item)
                    };
                    response.Contacts.AddRange(_mapper.Map<List<UserContact>>(contacts));
                    response.Aabs.AddRange(_mapper.Map<List<UserAabInformation>>(abbs));
                }
                else
                {
                    response = new UserInformationSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserInformationSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 用户信息状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserInformationStateResponse> UserInformationState(UserInformationStateRequest request, ServerCallContext context)
        {
            UserInformationStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserInformationStateRequestDto data = _mapper.Map<UserInformationStateRequestDto>(request);
                    var (code, msg, item, contacts, abbs) = await _userService.UserInformationState(data, user);
                    response = new UserInformationStateResponse
                    {
                        Code = code,
                        Message = msg,
                        User = _mapper.Map<UserInformation>(item)
                    };
                    response.Contacts.AddRange(_mapper.Map<List<UserContact>>(contacts));
                    response.Aabs.AddRange(_mapper.Map<List<UserAabInformation>>(abbs));
                }
                else
                {
                    response = new UserInformationStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserInformationStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }

    }
}
