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
    public class TopicCodesGrpcService :TopicCodesGrpc.TopicCodesGrpcBase
    {
        private readonly ITopicCodesService deviceCodeService;
        private readonly IMapper _mapper;

        public TopicCodesGrpcService(ITopicCodesService deviceCodeService, IMapper mapper)
        {
            this.deviceCodeService = deviceCodeService;
            _mapper = mapper;
        }
        /// <summary>
        /// 订阅代码树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<TopicCodesTreeResponse> TopicCodesTreeLoad(TopicCodesTreeRequest request, ServerCallContext context)
        {
           TopicCodesTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                   TopicCodesTreeRequestDto data = _mapper.Map<TopicCodesTreeRequestDto>(request);
                    var (code, msg, list) = await deviceCodeService.GetTopicTree(user);
                    response = new TopicCodesTreeResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.TopicCodesItems.AddRange(_mapper.Map<List<TopicCodesTreeNode>>(list));
                }
                else
                {
                    response = new TopicCodesTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new TopicCodesTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 订阅信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<TopicCodesInfoResponse> TopicCodesInfoLoad(TopicCodesInfoRequest request, ServerCallContext context)
        {
           TopicCodesInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, data) = await deviceCodeService.GetTopicInfo(request.TopicId, user);
                    response = new TopicCodesInfoResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<TopicCodesItem>(data)
                    };
                }
                else
                {
                    response = new TopicCodesInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new TopicCodesInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 订阅清单读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<TopicCodesSearchResponse> TopicCodesListLoad(TopicCodesSearchRequest request, ServerCallContext context)
        {
           TopicCodesSearchResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, list) = await deviceCodeService.TopicSearch(_mapper.Map<TopicCodesSearchRequestDto>(request), user);
                    response = new TopicCodesSearchResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.List.AddRange(_mapper.Map<List<TopicCodesItem>>(list));
                }
                else
                {
                    response = new TopicCodesSearchResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new TopicCodesSearchResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 订阅信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<TopicCodesPostResponse>TopicCodesPost(TopicCodesPostRequest request, ServerCallContext context)
        {
           TopicCodesPostResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, result) = await deviceCodeService.TopicInfoPost(_mapper.Map<TopicCodesItemDto>(request.TopicCodesItem),user);
                    response = new TopicCodesPostResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<TopicCodesItem>(result)
                    };
                }
                else
                {
                    response = new TopicCodesPostResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new TopicCodesPostResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 订阅状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<TopicCodesStateResponse>TopicCodesState(TopicCodesStateRequest request, ServerCallContext context)
        {
           TopicCodesStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg, result) = await deviceCodeService.TopicStateChange(request.State, request.TopicId, request.Reason, user);
                    response = new TopicCodesStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<TopicCodesItem>(result)
                    };
                }
                else
                {
                    response = new TopicCodesStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new TopicCodesStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
