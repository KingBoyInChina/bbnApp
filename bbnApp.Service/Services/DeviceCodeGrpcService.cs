using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.Domain.Entities.Code;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class DeviceCodeGrpcService : DeviceCodeGrpc.DeviceCodeGrpcBase
    {
        private readonly IDeviceCodeService deviceCodeService;
        private readonly IMapper _mapper;

        public DeviceCodeGrpcService(IDeviceCodeService deviceCodeService,  IMapper mapper)
        {
            this.deviceCodeService = deviceCodeService;
            _mapper = mapper;
        }
        /// <summary>
        /// 设备代码树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DeviceCodeTreeResponse> DeviceTreeLoad(DeviceCodeTreeRequest request, ServerCallContext context)
        {
            DeviceCodeTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    DeviceCodeTreeRequestDto data = _mapper.Map<DeviceCodeTreeRequestDto>(request);
                    var (code, msg, list) = await deviceCodeService.GetDeviceTree(user);
                    response = new DeviceCodeTreeResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.DeviceCodeItems.AddRange(_mapper.Map<List<DeviceCodeTreeNode>>(list));
                }
                else
                {
                    response = new DeviceCodeTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DeviceCodeTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 设备信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DeviceCodeInfoResponse> DeviceInfoLoad(DeviceCodeInfoRequest request, ServerCallContext context)
        {
            DeviceCodeInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, data,list) = await deviceCodeService.GetDeviceInfo(request.DeviceId, user);
                    response = new DeviceCodeInfoResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<DeviceCodeItem>(data)
                    };
                    response.List.AddRange(_mapper.Map<List<DeviceStructItem>>(list));
                }
                else
                {
                    response = new DeviceCodeInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DeviceCodeInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 设备清单读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DeviceCodeSearchResponse> DeviceListLoad(DeviceCodeSearchRequest request, ServerCallContext context)
        {
            DeviceCodeSearchResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list) = await deviceCodeService.DeviceSearch(_mapper.Map <DeviceCodeSearchRequestDto>(request), user);
                    response = new DeviceCodeSearchResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.List.AddRange(_mapper.Map<List<DeviceCodeItem>>(list));
                }
                else
                {
                    response = new DeviceCodeSearchResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DeviceCodeSearchResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 设备信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DeviceCodePostResponse> DeviceCodePost(DeviceCodePostRequest request, ServerCallContext context)
        {
            DeviceCodePostResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, result,list) = await deviceCodeService.DeviceInfoPost(_mapper.Map<DeviceCodeItemDto>(request.DeviceCodeItem), _mapper.Map<List<DeviceStructItemDto>>(request.DeviceStructItems), user);
                    response = new DeviceCodePostResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<DeviceCodeItem>(result)
                    };
                    response.List.AddRange(_mapper.Map<List<DeviceStructItem>>(list));
                }
                else
                {
                    response = new DeviceCodePostResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DeviceCodePostResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 设备状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DeviceCodeStateResponse> DeviceCodeState(DeviceCodeStateRequest request, ServerCallContext context)
        {
            DeviceCodeStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, result, list) = await deviceCodeService.DeviceStateChange(request.State, request.DeviceId, request.Reason, user);
                    response = new DeviceCodeStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<DeviceCodeItem>(result)
                    };
                    response.List.AddRange(_mapper.Map<List<DeviceStructItem>>(list));
                }
                else
                {
                    response = new DeviceCodeStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DeviceCodeStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 设备状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DeviceStructStateResponse> DeviceStructCodeState(DeviceStructStateRequest request, ServerCallContext context)
        {
            DeviceStructStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list) = await deviceCodeService.DeviceStructState(request.State, request.StructId, request.Reason, user);
                    response = new DeviceStructStateResponse
                    {
                        Code = code,
                        Message = msg,
                    };
                    response.List.AddRange(_mapper.Map<List<DeviceStructItem>>(list));
                }
                else
                {
                    response = new DeviceStructStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DeviceStructStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
