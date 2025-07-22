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
    public class DataDictionaryGrpcService:DataDictionaryGrpc.DataDictionaryGrpcBase
    {
        private readonly IDataDictionaryService _dataDictionaryService;
        private readonly IMapper _mapper;

        public DataDictionaryGrpcService(IDataDictionaryService dataDictionaryService,IMapper mapper)
        {
            _dataDictionaryService = dataDictionaryService;
            _mapper = mapper;
        }
        /// <summary>
        /// 字典下载
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DataDictionaryDownloadResponse> DicDownload(DataDictionaryDownloadRequest request, ServerCallContext context)
        {
            DataDictionaryDownloadResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list) = await _dataDictionaryService.DicLoad();
                    response = new DataDictionaryDownloadResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Item.AddRange(_mapper.Map<List<DicTreeItem>>(list));
                }
                else
                {
                    response = new DataDictionaryDownloadResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DataDictionaryDownloadResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 字典类别树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DataDictionaryTreeResponse> DicTree(DataDictionaryTreeRequest request, ServerCallContext context)
        {
            DataDictionaryTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    DataDictionaryTreeRequestDto data = _mapper.Map<DataDictionaryTreeRequestDto>(request);
                    var (code, msg, list) =await _dataDictionaryService.DicTree(data.FilterKey);
                    response = new DataDictionaryTreeResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Item.AddRange(_mapper.Map<List<DicTreeItem>>(list));
                }
                else
                {
                    response = new DataDictionaryTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DataDictionaryTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 字典明细
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DataDictionaryInfoResponse> DicInfo(DataDictionaryInfoRequest request, ServerCallContext context)
        {
            DataDictionaryInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    DataDictionaryInfoRequestDto data = _mapper.Map<DataDictionaryInfoRequestDto>(request);
                    var (code, msg,obj, list) =await _dataDictionaryService.DicRead(data.DicCode);
                    response = new DataDictionaryInfoResponse
                    {
                        Code = code,
                        Message = msg,
                        DicObj= _mapper.Map<DataDictionaryCode>(obj)
                    };
                    response.Items.AddRange(_mapper.Map<List<DataDictionaryItem>>(list));
                }
                else
                {
                    response = new DataDictionaryInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DataDictionaryInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 字典类别提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DataDictionarySaveResponse> DicPost(DataDictionarySaveRequest request, ServerCallContext context)
        {
            DataDictionarySaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    DataDictionarySaveRequestDto data = _mapper.Map<DataDictionarySaveRequestDto>(request);
                    var (code, msg, obj) = await _dataDictionaryService.DicSave(data.Item,user);
                    response = new DataDictionarySaveResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<DataDictionaryCode>(obj)
                    };
                }
                else
                {
                    response = new DataDictionarySaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DataDictionarySaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 字典类别状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DataDictionaryStateResponse> DicStateSave(DataDictionaryStateRequest request, ServerCallContext context)
        {
            DataDictionaryStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    DataDictionaryStateRequestDto data = _mapper.Map<DataDictionaryStateRequestDto>(request);
                    var (code, msg, obj) = await _dataDictionaryService.DicState(data.Type, data.Item, user);
                    response = new DataDictionaryStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<DataDictionaryCode>(obj)
                    };
                }
                else
                {
                    response = new DataDictionaryStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DataDictionaryStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 字典项目列表
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DataDictionaryItemSearchResponse> ItemsSearch(DataDictionaryItemSearchRequest request, ServerCallContext context)
        {
            DataDictionaryItemSearchResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    DataDictionaryItemSearchRequestDto data = _mapper.Map<DataDictionaryItemSearchRequestDto>(request);
                    var (code, msg, list) = await _dataDictionaryService.DicItemLoad(data.DicCode, user);
                    response = new DataDictionaryItemSearchResponse
                    {
                        Code = code,
                        Message = msg,
                    };
                    response.Items.AddRange(_mapper.Map<List<DataDictionaryItem>>(list));
                }
                else
                {
                    response = new DataDictionaryItemSearchResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DataDictionaryItemSearchResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 字典项目提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DataDictionaryItemSaveResponse> ItemPost(DataDictionaryItemSaveRequest request, ServerCallContext context)
        {
            DataDictionaryItemSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    DataDictionaryItemSaveRequestDto data = _mapper.Map<DataDictionaryItemSaveRequestDto>(request);
                    var (code, msg, model) = await _dataDictionaryService.DicItemSave(data.Item, user);
                    response = new DataDictionaryItemSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        Item= _mapper.Map<DataDictionaryItem>(model)
                    };
                }
                else
                {
                    response = new DataDictionaryItemSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DataDictionaryItemSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 字典项目变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<DataDictionaryItemStateResponse> ItemStateSave(DataDictionaryItemStateRequest request, ServerCallContext context)
        {
            DataDictionaryItemStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    DataDictionaryItemStateRequestDto data = _mapper.Map<DataDictionaryItemStateRequestDto>(request);
                    var (code, msg, model) = await _dataDictionaryService.DicItemState(data.Type,data.ItemId, user);
                    response = new DataDictionaryItemStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<DataDictionaryItem>(model)
                    };
                }
                else
                {
                    response = new DataDictionaryItemStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new DataDictionaryItemStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
