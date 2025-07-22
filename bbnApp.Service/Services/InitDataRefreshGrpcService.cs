using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.IServices.IINIT;
using bbnApp.Application.Services.CODE;
using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class InitDataRefreshGrpcService : InitDataRefreshGrpc.InitDataRefreshGrpcBase
    {
        private readonly IInitializationRefresh initializationRefresh;
        private readonly IMapper _mapper;

        public InitDataRefreshGrpcService(IInitializationRefresh initializationRefresh, IOperatorService operatorService, IMapper mapper)
        {
            this.initializationRefresh = initializationRefresh;
            _mapper = mapper;
        }
        /// <summary>
        /// 手动更新平台服务字典缓存
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<InitDataRefreshResponse> InitDataRefresh(InitDataRefreshRequest request, ServerCallContext context)
        {
            InitDataRefreshResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    var (code, msg) = await initializationRefresh.Refresh(request.Type,user);
                    response = new InitDataRefreshResponse
                    {
                        Code = code,
                        Message = msg
                    };
                }
                else
                {
                    response = new InitDataRefreshResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new InitDataRefreshResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
