using AutoMapper;
using bbnApp.Application.IServices.IBusiness;
using bbnApp.Application.Services.Business;
using bbnApp.Common.Models;
using bbnApp.DTOs.BusinessDto;
using bbnApp.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class UserDeviceGrpcService : UserDeviceGrpc.UserDeviceGrpcBase
    {
        private readonly IUserDevices userDevices;
        private readonly IMapper _mapper;
        public UserDeviceGrpcService(IUserDevices userDevices, IMapper mapper)
        {
            this.userDevices = userDevices;
            _mapper = mapper;
        }
        /// <summary>
        /// 获取用户树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserDeviceTreeResponse> UserDeviceTree(UserDeviceTreeRequest request, ServerCallContext context)
        {
            UserDeviceTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserDeviceTreeRequestDto data = _mapper.Map<UserDeviceTreeRequestDto>(request);
                    var (code, msg, list) = await userDevices.UserDeviceTree(data, user);
                    response = new UserDeviceTreeResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Items.AddRange(_mapper.Map<List<UserDeviceTreeItem>>(list));
                }
                else
                {
                    response = new UserDeviceTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserDeviceTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 用户信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserDeviceListResponse> UserDeviceList(UserDeviceListRequest request, ServerCallContext context)
        {
            UserDeviceListResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserDeviceListRequestDto data = _mapper.Map<UserDeviceListRequestDto>(request);
                    var (code, msg,userinformation, getways,boxs) = await userDevices.UserDeviceList(data, user);
                    response = new UserDeviceListResponse
                    {
                        Code = code,
                        Message = msg,
                        User= _mapper.Map<UserInfoData>(userinformation)
                    };
                    response.GetWays.AddRange(_mapper.Map<List<UserGetWay>>(getways));
                    response.Boxs.AddRange(_mapper.Map<List<UserBox>>(boxs));
                }
                else
                {
                    response = new UserDeviceListResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserDeviceListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 用户网关信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserGetWaySaveResponse> UserGetWaySave(UserGetWaySaveRequest request, ServerCallContext context)
        {
            UserGetWaySaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserGetWaySaveRequestDto data = _mapper.Map<UserGetWaySaveRequestDto>(request);
                    var (code, msg, result) = await userDevices.UserGetWaySave(data, user);
                    response = new UserGetWaySaveResponse
                    {
                        Code = code,
                        Message = msg,
                        GetWay= _mapper.Map<UserGetWay>(result)
                    };
                }
                else
                {
                    response = new UserGetWaySaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserGetWaySaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 网关设备提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserGetWayDeviceSaveResponse> UserGetWayDeviceSave(UserGetWayDeviceSaveRequest request, ServerCallContext context)
        {
            UserGetWayDeviceSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserGetWayDeviceSaveRequestDto data = _mapper.Map<UserGetWayDeviceSaveRequestDto>(request);
                    var (code, msg, result) = await userDevices.UserGetWayDeviceSave(data, user);
                    response = new UserGetWayDeviceSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        Device = _mapper.Map<UserGetWayDevice>(result)
                    };
                }
                else
                {
                    response = new UserGetWayDeviceSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserGetWayDeviceSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 边缘盒子提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserBoxSaveResponse> UserBoxSave(UserBoxSaveRequest request, ServerCallContext context)
        {
            UserBoxSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserBoxSaveRequestDto data = _mapper.Map<UserBoxSaveRequestDto>(request);
                    var (code, msg, result) = await userDevices.UserBoxSave(data, user);
                    response = new UserBoxSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        Box = _mapper.Map<UserBox>(result)
                    };
                }
                else
                {
                    response = new UserBoxSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserBoxSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 摄像头提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserCameraSaveResponse> UserCameraSave(UserCameraSaveRequest request, ServerCallContext context)
        {
            UserCameraSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserCameraSaveRequestDto data = _mapper.Map<UserCameraSaveRequestDto>(request);
                    var (code, msg, result) = await userDevices.UserCameraSave(data, user);
                    response = new UserCameraSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        Camera = _mapper.Map<UserCamera>(result)
                    };
                }
                else
                {
                    response = new UserCameraSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserCameraSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UserDeviceStateResponse> UserDeviceState(UserDeviceStateRequest request, ServerCallContext context)
        {
            UserDeviceStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UserDeviceStateRequestDto data = _mapper.Map<UserDeviceStateRequestDto>(request);
                    var (code, msg, getway,box) = await userDevices.UserDeviceState(data, user);
                    response = new UserDeviceStateResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.GetWay.AddRange(_mapper.Map<List<UserGetWay>>(getway));
                    response.Box.AddRange(_mapper.Map<List<UserBox>>(box));
                }
                else
                {
                    response = new UserDeviceStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UserDeviceStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
