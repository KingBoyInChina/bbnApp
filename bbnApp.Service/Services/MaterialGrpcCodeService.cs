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
    public class MaterialGrpcCodeService : MaterialsCodeGrpc.MaterialsCodeGrpcBase
    {
        private readonly IMaterialsCodeService _materialsService;
        private readonly IOperatorService _operatorService;
        private readonly IMapper _mapper;

        public MaterialGrpcCodeService(IMaterialsCodeService materialsService, IOperatorService operatorService, IMapper mapper)
        {
            _materialsService = materialsService;
            _operatorService = operatorService;
            _mapper = mapper;
        }
        /// <summary>
        /// 物资代码树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<MaterialsCodeTreeResponse> GetMaterailTree(MaterialsCodeTreeRequest request, ServerCallContext context)
        {
            MaterialsCodeTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    OperationObjectTreeRequestDto data = _mapper.Map<OperationObjectTreeRequestDto>(request);
                    var (code, msg, list) = await _materialsService.GetMaterailTree(user);
                    response = new MaterialsCodeTreeResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Items.AddRange(_mapper.Map<List<MaterialTreeItem>>(list));
                }
                else
                {
                    response = new MaterialsCodeTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new MaterialsCodeTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 物资信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<MaterialsCodeInfoResponse> GetMaterialInfo(MaterialsCodeInfoRequest request, ServerCallContext context)
        {
            MaterialsCodeInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, data) = await _materialsService.GetMaterialInfo(request.MaterialId, user);
                    response = new MaterialsCodeInfoResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<MaterialsCode>(data)
                    };
                }
                else
                {
                    response = new MaterialsCodeInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new MaterialsCodeInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<MaterialsCodeListResponse> GetMaterialList(MaterialsCodeListRequest request, ServerCallContext context)
        {
            MaterialsCodeListResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list) = await _materialsService.GetMaterialList(request.MaterialType, user);
                    response = new MaterialsCodeListResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Items.AddRange(_mapper.Map<List<MaterialsCode>>(list));
                }
                else
                {
                    response = new MaterialsCodeListResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new MaterialsCodeListResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 物资信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<MaterialsCodeSaveResponse> MaterialSave(MaterialsCodeSaveRequest request, ServerCallContext context)
        {
            MaterialsCodeSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var data = _mapper.Map<MaterialsCodeDto>(request.Item);
                    var (code, msg, result) = await _materialsService.MaterialSave(data, user);
                    response = new MaterialsCodeSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        Item =  _mapper.Map<MaterialsCode>(result) 
                    };
                }
                else
                {
                    response = new MaterialsCodeSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new MaterialsCodeSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 物资状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<MaterialsCodeStateResponse> MaterialState(MaterialsCodeStateRequest request, ServerCallContext context)
        {
            MaterialsCodeStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, result) = await _materialsService.MaterialState(request.Type,request.MaterialId, user);
                    response = new MaterialsCodeStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<MaterialsCode>(result)
                    };
                }
                else
                {
                    response = new MaterialsCodeStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new MaterialsCodeStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
