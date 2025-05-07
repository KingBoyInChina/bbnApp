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
    public class OperationObjectGrpcService: OperationObjectCodeGrpc.OperationObjectCodeGrpcBase
    {
        private readonly IOperationObjectsService _operationObjectsService;
        private readonly IOperatorService _operatorService;
        private readonly IMapper _mapper;

        public OperationObjectGrpcService(IOperationObjectsService operationObjectsService, IOperatorService operatorService, IMapper mapper)
        {
            _operationObjectsService = operationObjectsService;
            _operatorService = operatorService;
            _mapper = mapper;
        }
        /// <summary>
        /// 标准操作代码树读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<OperationObjectTreeResponse> OperationObjectTree(OperationObjectTreeRequest request, ServerCallContext context)
        {
            OperationObjectTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    OperationObjectTreeRequestDto data = _mapper.Map<OperationObjectTreeRequestDto>(request);
                    var (code, msg, list) =await _operationObjectsService.OperationObjectTree();
                    response = new OperationObjectTreeResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Item.AddRange(_mapper.Map<List<OperationObjectNode>>(list));
                }
                else
                {
                    response = new OperationObjectTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new OperationObjectTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 标准操作代码清单读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<OperationObjectCodeListResponse> OperationObjectCodeList(OperationObjectCodeListRequest request, ServerCallContext context)
        {
            OperationObjectCodeListResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    OperationObjectCodeListRequestDto data = _mapper.Map<OperationObjectCodeListRequestDto>(request);
                    var (code, msg, list) = await _operationObjectsService.OperationObjectCodeList();
                    response = new OperationObjectCodeListResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Item.AddRange(_mapper.Map<List<OperationObjectCode>>(list));
                }
                else
                {
                    response = new OperationObjectCodeListResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new OperationObjectCodeListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 对象代码信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<GetOperationInfoResponse> GetOperationInfo(GetOperationInfoRequest request, ServerCallContext context)
        {
            GetOperationInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    GetOperationInfoRequestDto data = _mapper.Map<GetOperationInfoRequestDto>(request);
                    var (code, msg,Obj, list) = await _operationObjectsService.GetOperationInfo(data.ObjCode);
                    response = new GetOperationInfoResponse
                    {
                        Code = code,
                        Message = msg,
                        Obj = _mapper.Map<OperationObjectCode>(Obj)
                    };
                    response.Item.AddRange(_mapper.Map<List<ObjectOperationType>>(list));
                }
                else
                {
                    response = new GetOperationInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new GetOperationInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 对象代码信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<SaveOperationInfoResponse> SaveOperationInfo(SaveOperationInfoRequest request, ServerCallContext context)
        {
            SaveOperationInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    SaveOperationInfoRequestDto data = _mapper.Map<SaveOperationInfoRequestDto>(request);
                    var (code, msg, item) = await _operationObjectsService.SaveOperationInfo(data.Data,user);
                    response = new SaveOperationInfoResponse
                    {
                        Code = code,
                        Message = msg,
                        Obj = _mapper.Map<OperationObjectCode>(item)
                    };
                }
                else
                {
                    response = new SaveOperationInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new SaveOperationInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 对象代码状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<OperationStateResponse> OperationState(OperationStateRequest request, ServerCallContext context)
        {
            OperationStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    OperationStateRequestDto data = _mapper.Map<OperationStateRequestDto>(request);
                    var (code, msg, item) = await _operationObjectsService.OperationState(data.Type,data.ObjCode, user);
                    response = new OperationStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Obj = _mapper.Map<OperationObjectCode>(item)
                    };
                }
                else
                {
                    response = new OperationStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new OperationStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 操作代码状态勾选状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<ItemSaveResponse> ItemSave(ItemSaveRequest request, ServerCallContext context)
        {
            ItemSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    ItemSaveRequestDto data = _mapper.Map<ItemSaveRequestDto>(request);
                    var (code, msg) = await _operationObjectsService.ItemSave(data.Item, user);
                    response = new ItemSaveResponse
                    {
                        Code = code,
                        Message = msg
                    };
                }
                else
                {
                    response = new ItemSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new ItemSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }


    }
}
