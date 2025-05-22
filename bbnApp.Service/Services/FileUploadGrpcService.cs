using AutoMapper;
using bbnApp.Application.IServices.IBusiness;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using bbnApp.DTOs.BusinessDto;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class FileUploadGrpcService : UploadFileGrpc.UploadFileGrpcBase
    {
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public FileUploadGrpcService(IFileUploadService fileUploadService, IOperatorService operatorService, IMapper mapper)
        {
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }
        /// <summary>
        /// 物资代码树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UploadFileResponse> UploadFilePost(UploadFileRequest request, ServerCallContext context)
        {
            UploadFileResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UploadFileRequestDto data = _mapper.Map<UploadFileRequestDto>(request);
                    var (code, msg, obj) = await _fileUploadService.FileUploadPost(data, user);
                    response = new UploadFileResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Item = _mapper.Map<UploadFileItem>(obj);
                }
                else
                {
                    response = new UploadFileResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UploadFileResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 文件状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UploadFileStateResponse> UploadFileState(UploadFileStateRequest request, ServerCallContext context)
        {
            UploadFileStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UploadFileStateRequestDto data = _mapper.Map<UploadFileStateRequestDto>(request);
                    var (code, msg, obj) = await _fileUploadService.FileUploadState(data.Type,data.FileId,data.LockReason, user);
                    response = new UploadFileStateResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Item = _mapper.Map<FileItems>(obj);
                }
                else
                {
                    response = new UploadFileStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UploadFileStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 文件读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<UploadFileReadResponse> UploadFileRead(UploadFileReadRequest request, ServerCallContext context)
        {
            UploadFileReadResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    UploadFileReadRequestDto data = _mapper.Map<UploadFileReadRequestDto>(request);
                    var (code, msg, obj) = await _fileUploadService.GetUploadFileList(data.LinkKey, data.LinkTable, user);
                    response = new UploadFileReadResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Item = _mapper.Map<UploadFileItem>(obj);
                }
                else
                {
                    response = new UploadFileReadResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new UploadFileReadResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
