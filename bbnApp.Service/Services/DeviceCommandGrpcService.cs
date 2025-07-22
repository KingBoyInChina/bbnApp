using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.Services.CODE;
using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class DeviceCommandGrpcService : DeviceCommandGrpc.DeviceCommandGrpcBase
    {
        private readonly IDeviceCommandService deviceCommandService;
        private readonly IMapper _mapper;

        public DeviceCommandGrpcService(IDeviceCommandService deviceCommandService, IMapper mapper)
        {
            this.deviceCommandService = deviceCommandService;
            _mapper = mapper;
        }

        /// <summary>
        /// 设备指令列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DeviceCommandListResponse> DeviceCommandList(DeviceCommandListRequest request, ServerCallContext context)
        {
            DeviceCommandListResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var data = _mapper.Map<DeviceCommandListRequestDto>(request);
                    var (code, msg, list) = await deviceCommandService.GetDeviceCommandList(data, user);
                    response = new DeviceCommandListResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.DeviceCommands.AddRange(_mapper.Map<List<DeviceCommand>>(list));
                }
                else
                {
                    response = new DeviceCommandListResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DeviceCommandListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 设备指令提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DeviceCommandSaveResponse> DeviceCommandSave(DeviceCommandSaveRequest request, ServerCallContext context)
        {
            DeviceCommandSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var data = _mapper.Map<DeviceCommandSaveRequestDto>(request);
                    var (code, msg, Item) = await deviceCommandService.DeviceCommandSave(data, user);
                    response = new DeviceCommandSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        DeviceCommand = _mapper.Map<DeviceCommand>(Item)
                    };
                }
                else
                {
                    response = new DeviceCommandSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DeviceCommandSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 设备指令状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DeviceCommandStateResponse> DeviceCommandState(DeviceCommandStateRequest request, ServerCallContext context)
        {
            DeviceCommandStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var data = _mapper.Map<DeviceCommandStateRequestDto>(request);
                    var (code, msg, Item) = await deviceCommandService.DeviceCommandState(data, user);
                    response = new DeviceCommandStateResponse
                    {
                        Code = code,
                        Message = msg,
                        DeviceCommand = _mapper.Map<DeviceCommand>(Item)
                    };
                }
                else
                {
                    response = new DeviceCommandStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DeviceCommandStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
