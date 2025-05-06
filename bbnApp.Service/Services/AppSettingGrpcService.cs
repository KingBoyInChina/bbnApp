using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using BbnApp.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class AppSettingGrpcService: AppSettingGrpc.AppSettingGrpcBase
    {
        private readonly IAppSettingService _appSettingService;
        private readonly IOperatorService _operatorService;
        private readonly IMapper _mapper;
        public AppSettingGrpcService(IAppSettingService appSettingService, IOperatorService operatorService, IMapper mapper)
        {
            _appSettingService = appSettingService;
            _operatorService = operatorService;
            _mapper = mapper;
        }
        /// <summary>
        /// 配置清单查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AppSettingSearchResponse> AppSettingGridLoad(AppSettingSearchRequest request, ServerCallContext context)
        {
            AppSettingSearchResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    AppSettingSearchRequestDto data=_mapper.Map<AppSettingSearchRequestDto>(request);
                    var (code,msg,result,total) = await _appSettingService.AppSettingSearch(data, user);
                    response =new AppSettingSearchResponse { 
                        Code= code,
                        Message= msg,
                        Total=total
                    };
                    List<AppSettingDto> list = result.ToList();
                    response.Items.AddRange(_mapper.Map<List<AppSetting>>(list));
                }
                else
                {
                    response = new AppSettingSearchResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new AppSettingSearchResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 配置信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<AppSettingPostResponse> AppSettingPostSave(AppSettingPostRequest request, ServerCallContext context)
        {
            AppSettingPostResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    AppSettingPostRequestDto data = _mapper.Map<AppSettingPostRequestDto>(request);
                    var (code,msg,result) = await _appSettingService.AppSettingSave(data, user);
                    AppSetting item=_mapper.Map<AppSetting>(result);
                    response = new AppSettingPostResponse { 
                        Code = code,
                        Message = msg,
                        Item= item
                    };
                }
                else
                {
                    response = new AppSettingPostResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new AppSettingPostResponse
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
        public override async Task<AppSettingStateResponse> AppSettingStateSave(AppSettingStateRequest request, ServerCallContext context)
        {
            AppSettingStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    AppSettingStateRequestDto data = _mapper.Map<AppSettingStateRequestDto>(request);
                    var (code,msg,result) = await _appSettingService.AppSettingStateSave(data, user);
                    AppSetting item = _mapper.Map<AppSetting>(result);
                    response = new AppSettingStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = item
                    };
                }
                else
                {
                    response = new AppSettingStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new AppSettingStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
