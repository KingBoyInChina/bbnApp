using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class AreaGrpcService :AreaGrpc.AreaGrpcBase
    {
        private readonly IAreaService _areaService;
        private readonly IOperatorService _operatorService;
        private readonly IMapper _mapper;
        public AreaGrpcService(IAreaService areaService, IOperatorService operatorService, IMapper mapper)
        {
            _areaService = areaService;
            _operatorService = operatorService;
            _mapper = mapper;
        }
        /// <summary>
        /// 行政区划字典Tree
        /// </summary>
        /// <param name="request"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AreaTreeNodeResponse> AreaTreeLoad(AreaTreeNodeRequest request, ServerCallContext context)
        {
            AreaTreeNodeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var arrList = await _areaService.AreaTreeLoad(string.Empty);

                    var areaitems = _mapper.Map<List<AreaTreeNode>>(arrList);
                    response = new AreaTreeNodeResponse
                    {
                        Code = true,
                        Message = "行政区划树获取完成",
                    };
                    response.Items.AddRange(areaitems);
                }
                else
                {
                    throw new Exception("无效操作员身份信息");
                }
            }
            catch (Exception ex)
            {
                response = new AreaTreeNodeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 行政区划字典list
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AreaListNodeResponse> AreaListLoad(AreaListNodeRequest request, ServerCallContext context)
        {
            AreaListNodeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var arrList = await _areaService.AreaListLoad(string.Empty);

                    var areaitems = _mapper.Map<List<AreaListNode>>(arrList);
                    response = new AreaListNodeResponse
                    {
                        Code = true,
                        Message = "行政区划清单获取完成",
                    };
                    response.Items.AddRange(areaitems);
                }
                else
                {
                    throw new Exception("无效操作员身份信息");
                }
            }
            catch (Exception ex)
            {
                response = new AreaListNodeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 从数据库中获取行政区划数据
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AreaGridResponse> GetAreaGrid(AreaGridRequest request, ServerCallContext context)
        {
            AreaGridResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    (bool, string, IEnumerable<AreaItemDto>, int) items = await _areaService.AreaSearch(user, request.AreaCode, request.AreaName, request.AreaLeve, request.PageIndex, request.PageSize);
                    if (items.Item1)
                    {
                        var areaitems = _mapper.Map<List<AreaItem>>(items.Item3);
                        response = new AreaGridResponse
                        {
                            Code = true,
                            Message = items.Item2,
                            Total = items.Item4
                        };
                        response.AreaItems.AddRange(areaitems);
                    }
                    else
                    {
                        response = new AreaGridResponse
                        {
                            Code = false,
                            Message = items.Item2,
                            Total = 0
                        };
                    }
                }
                else
                {
                    response = new AreaGridResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息",
                        Total = 0
                    };
                }
            }
            catch (Exception ex)
            {
                response = new AreaGridResponse
                {
                    Code = false,
                    Message = ex.Message,
                    Total = 0
                };
            }
            return response;
        }
        /// <summary>
        /// 行政区划数据提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AreaPostResponse> AreaPost(AreaPostRequest request,ServerCallContext context)
        {
            AreaPostResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    AreaPostDataDto postdata = _mapper.Map<AreaPostDataDto>(request.AreaData);
                    (bool, string, AreaPostDataDto) data = await _areaService.AreaSave(postdata, user);
                    if (data.Item1)
                    {
                        AreaPostData backdata = _mapper.Map<AreaPostData>(data.Item3);
                        response = new AreaPostResponse
                        {
                            Code = true,
                            Message = "数据提交成功"
                        };
                        response.AreaData.Add(backdata);
                    }
                    else
                    {

                        response = new AreaPostResponse
                        {
                            Code = false,
                            Message =data.Item2
                        };
                    }
                }
                else
                {
                    response = new AreaPostResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new AreaPostResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 行政区划删除
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AreaDeleteResponse> AreaDelete(AreaDeleteRequest request, ServerCallContext context)
        {
            AreaDeleteResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    AreaDeleteRequestDto postdata = _mapper.Map<AreaDeleteRequestDto>(request);
                    (bool,string) data = await _areaService.AreaDelete(postdata,user);
                    response = new AreaDeleteResponse
                    {
                        Code = data.Item1,
                        Message = data.Item2
                    };
                }
                else
                {
                    response = new AreaDeleteResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new AreaDeleteResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 行政地区停用
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AreaLockResponse> AreaLock(AreaLockRequest request, ServerCallContext context)
        {
            AreaLockResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    AreaLockRequestDto postdata = _mapper.Map<AreaLockRequestDto>(request);
                    (bool,string) data = await _areaService.AreaLock(postdata,user);

                    response = new AreaLockResponse { Code = data.Item1, Message = data.Item2 };
                }
                else
                {
                    response = new AreaLockResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new AreaLockResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
