using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.IServices.IINIT;
using bbnApp.Common.Models;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CommDto;
using bbnApp.Protos;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class GuideGrpcService : GuideGrpc.GuideGrpcBase
    {
        private readonly IGuideService guideService;
        private readonly IMapper _mapper;

        public GuideGrpcService(IGuideService guideService, IOperatorService operatorService, IMapper mapper)
        {
            this.guideService = guideService;
            _mapper = mapper;
        }
        /// <summary>
        /// 地区名称匹配所有的地区
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<GetLoactionResponse> GetLoactionList(GetLoactionRequest request, ServerCallContext context)
        {
            GetLoactionResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    GetLoactionRequestDto data = _mapper.Map<GetLoactionRequestDto>(request);
                    var (code, msg, list) = await guideService.GetLoactionList(data.City,data.Address,data.Output);
                    response = new GetLoactionResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Items.AddRange(_mapper.Map<List<Geocode>>(list)) ;
                }
                else
                {
                    response = new GetLoactionResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new GetLoactionResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 地区名称匹配默认地址
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<GetDefaultLoactionResponse> GetDefaultLoaction(GetDefaultLoactionRequest request, ServerCallContext context)
        {
            GetDefaultLoactionResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    GetDefaultLoactionRequestDto data = _mapper.Map<GetDefaultLoactionRequestDto>(request);
                    var (code, msg, obj) = await guideService.GetLoactionList(data.City, data.Address, data.Output);
                    response = new GetDefaultLoactionResponse
                    {
                        Code = code,
                        Message = msg,
                        Item= _mapper.Map<Geocode>(obj)
                    };
                }
                else
                {
                    response = new GetDefaultLoactionResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new GetDefaultLoactionResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
