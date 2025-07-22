using AutoMapper;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Application.Services.CODE;
using bbnApp.Common.Models;
using bbnApp.DTOs.CodeDto;
using bbnApp.Protos;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace bbnApp.Service.Services
{
    public class EmployeeGrpcServcie :EmployeeGrpc.EmployeeGrpcBase
    {
        private readonly IOperatorService _operatorService;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        public EmployeeGrpcServcie(IOperatorService operatorService, IEmployeeService employeeService, IMapper mapper)
        {
            _operatorService = operatorService;
            _employeeService = employeeService;;
            _mapper = mapper;
        }

        /// <summary>
        /// 人员树
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<EmployeeTreeResponse> EmployeeTreeLoad(EmployeeTreeRequest request, ServerCallContext context)
        {
            EmployeeTreeResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    // 使用 AutoMapper 映射请求
                    var employeeRequestDto = _mapper.Map<EmployeeTreeRequestDto>(request);
                    // 调用应用程序中的服务
                    var data = await _employeeService.EmployeeTreeLoad(employeeRequestDto, user);
                    response = new EmployeeTreeResponse
                    {
                        Code = true,
                        Message = "人员信息初始化完成"
                    };
                    response.Items.AddRange(_mapper.Map<List<EmployeeTreeItem>>(data.Item3));
                }
                else
                {
                    response = new EmployeeTreeResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new EmployeeTreeResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }

            return response;
        }
        /// <summary>
        /// 人员分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<EmployeeSearchResponse> EmployeeSearchLoad(EmployeeSearchRequest request, ServerCallContext context)
        {
            EmployeeSearchResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    EmployeeSearchRequestDto data = _mapper.Map<EmployeeSearchRequestDto>(request);
                    var (code, msg, result, total) = await _employeeService.EmployeeSearch(data, user);
                    response = new EmployeeSearchResponse
                    {
                        Code = code,
                        Message = msg,
                        Total = total
                    };
                    List<EmployeeItemDto> list = result?.ToList()??new List<EmployeeItemDto>();
                    response.Items.AddRange(_mapper.Map<List<EmployeeItem>>(list));
                }
                else
                {
                    response = new EmployeeSearchResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new EmployeeSearchResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 人员清单查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<EmployeeItemsResponse> EmployeeListLoad(EmployeeItemsRequest request, ServerCallContext context)
        {
            EmployeeItemsResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    EmployeeItemsRequestDto data = _mapper.Map<EmployeeItemsRequestDto>(request);
                    var (code, msg, result) = await _employeeService.EmployeeListLoad(data, user);
                    response = new EmployeeItemsResponse
                    {
                        Code = code,
                        Message = msg
                    };
                    response.Items.AddRange(_mapper.Map<List<EmployeeItem>>(result));
                }
                else
                {
                    response = new EmployeeItemsResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new EmployeeItemsResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 人员信息读取
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<EmployeeInfoResponse> EmployeeInfoLoad(EmployeeInfoRequest request, ServerCallContext context)
        {
            EmployeeInfoResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    EmployeeInfoRequestDto data = _mapper.Map<EmployeeInfoRequestDto>(request);
                    var (code, msg, result) = await _employeeService.EmployeeInfoLoad(data.EmployeeId,data.CompanyId,data.CompanyName, user);
                    response = new EmployeeInfoResponse
                    {
                        Code = code,
                        Message = msg,
                        Item= _mapper.Map<EmployeeItem>(result)
                    };
                }
                else
                {
                    response = new EmployeeInfoResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new EmployeeInfoResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 人员信息提交
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<EmployeeSaveResponse> EmployeePost(EmployeeSaveRequest request, ServerCallContext context)
        {
            EmployeeSaveResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    EmployeeSaveRequestDto data = _mapper.Map<EmployeeSaveRequestDto>(request);
                    var (code, msg, result) = await _employeeService.EmployeePost(data.Item, user);
                    response = new EmployeeSaveResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<EmployeeItem>(result)
                    };
                }
                else
                {
                    response = new EmployeeSaveResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new EmployeeSaveResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
        /// <summary>
        /// 员工状态变更
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [Authorize]
        public override async Task<EmployeeStateResponse> EmployeeState(EmployeeStateRequest request, ServerCallContext context)
        {
            EmployeeStateResponse? response = null;
            try
            {
                if (context.UserState.TryGetValue("User", out var userObj) && userObj is UserModel user)
                {
                    EmployeeStateRequestDto data = _mapper.Map<EmployeeStateRequestDto>(request);
                    var (code, msg, result) = await _employeeService.EmployeeState(data.Type,data.EmployeeId,data.Reason, user);
                    response = new EmployeeStateResponse
                    {
                        Code = code,
                        Message = msg,
                        Item = _mapper.Map<EmployeeItem>(result)
                    };
                }
                else
                {
                    response = new EmployeeStateResponse
                    {
                        Code = false,
                        Message = "无效操作员身份信息"
                    };
                }
            }
            catch (Exception ex)
            {
                response = new EmployeeStateResponse
                {
                    Code = false,
                    Message = ex.Message
                };
            }
            return response;
        }
    }
}
