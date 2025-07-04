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
    public class OperatorGrpcService : OperatorGrpc.OperatorGrpcBase
    {
        private readonly IOperatorService operatorService;
        private readonly IMapper _mapper;

        public OperatorGrpcService(IOperatorService operatorService, IMapper mapper)
        {
            this.operatorService = operatorService;
            _mapper = mapper;
        }
        /// <summary>
        /// 操作员列表加载
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<OperatorListResponse> OperatorListLoad(OperatorListRequest request, ServerCallContext context)
        {
            OperatorListResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list) = await operatorService.OperatorListLoad(_mapper.Map<OperatorListRequestDto>(request), user);
                    response = new OperatorListResponse
                    {
                        Code = code,
                        Message = msg,
                    };
                    response.Items.AddRange(_mapper.Map<List<OperatorItem>>(list));
                }
                else
                {
                    response = new OperatorListResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new OperatorListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 操作员信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<OperatorSaveResponse> OperatorSave(OperatorSaveRequest request, ServerCallContext context)
        {
            OperatorSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, data) = await operatorService.OperatorSave(_mapper.Map<OperatorSaveRequestDto>(request), user);
                    response = new OperatorSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        Item= _mapper.Map<OperatorItem>(data)
                    };
                }
                else
                {
                    response = new OperatorSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new OperatorSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 操作员状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<OperatorStateResponse> OperatorState(OperatorStateRequest request, ServerCallContext context)
        {
            OperatorStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, data) = await operatorService.OperatorState(_mapper.Map<OperatorStateRequestDto>(request), user);
                    response = new OperatorStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<OperatorItem>(data)
                    };
                }
                else
                {
                    response = new OperatorStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new OperatorStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 同事信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<OperatorsLoadResponse> OperatorsLoad(OperatorsLoadRequest request, ServerCallContext context)
        {
            OperatorsLoadResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, data) = await operatorService.GetWorkers(request.Yhid,request.CompanyId);
                    response = new OperatorsLoadResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Operators.AddRange(_mapper.Map<List<WorkerItem>>(data));
                }
                else
                {
                    response = new OperatorsLoadResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new OperatorsLoadResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }

    }
}
