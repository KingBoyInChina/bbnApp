using Grpc.Core.Interceptors;
using Grpc.Core;
using System.Text;
using Serilog;
using Exceptionless;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using bbnApp.Core;
using bbnApp.Domain.Entities.Safe;
using bbnApp.Domain.Entities.Code;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using bbnApp.Application.IServices.ICODE;
using bbnApp.Common.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using bbnApp.Common;
using System.Reflection;

namespace bbnApp.Service.GlobalService
{
    public class GlobalInterceptor : Interceptor
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IMemoryCache _cache;
        /// <summary>
        /// 
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// 
        /// </summary>
        private readonly IOperatorService _operatorService;
        /// <summary>
        /// 限制时间窗口
        /// </summary>
        private static readonly TimeSpan RequestLimitWindow = TimeSpan.FromMinutes(1);
        /// <summary>
        /// 限制请求次数
        /// </summary>
        private const int RequestLimitCount = 60;
        /// <summary>
        /// 黑名单
        /// </summary>
        private static readonly HashSet<string> Blacklist = new HashSet<string>();
        /// <summary>
        /// 白名单
        /// </summary>
        private static HashSet<string> AllowedIps = new HashSet<string>();
        /// <summary>
        /// 代码库上下文
        /// </summary>
        private readonly IApplicationDbContext _codeContext;
        /// <summary>
        /// 拦截器
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="cache"></param>
        /// <param name="codeContext"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="operatorService"></param>
        public GlobalInterceptor(IConfiguration configuration, IMemoryCache cache, IApplicationDbContext codeContext, IHttpContextAccessor httpContextAccessor, IOperatorService operatorService)
        {
            _cache = cache;
            AllowedIps = new HashSet<string>(configuration.GetSection("IpRateLimiting:IpWhitelist").Get<IEnumerable<string>>());
            _codeContext = codeContext;
            _httpContextAccessor = httpContextAccessor;
            _operatorService = operatorService;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                // 获取客户端 IP 地址
                var ipAddress = context.GetHttpContext().Connection.RemoteIpAddress?.ToString();
                if (string.IsNullOrEmpty(ipAddress))
                {
                    throw new RpcException(new Status(StatusCode.PermissionDenied, "无效的IP地址"));
                }
                // 检查黑名单
                if (Blacklist.Contains(ipAddress))
                {
                    Log.Warning("请求来自于黑名单: {IP}", ipAddress);
                    throw new RpcException(new Status(StatusCode.PermissionDenied, $"您的IP【{ipAddress}】已经被纳入黑名单"));
                }

                // 检查请求频率
                var cacheKey = $"rate_limit_{ipAddress}";
                var requestCount = _cache.GetOrCreate(cacheKey, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = RequestLimitWindow;
                    return 0;
                });

                if (requestCount >= RequestLimitCount)
                {
                    // 超过请求频率，拉入黑名单
                    Blacklist.Add(ipAddress);
                    Log.Warning("IP {IP} added to blacklist due to excessive requests", ipAddress);
                    #region 写入数据库
                    LimiteRecord _record = new LimiteRecord
                    {
                        Yhid = "000000",
                        LimiteId = Guid.NewGuid().ToString("N"),
                        LimiteIP = ipAddress,
                        LimiteTime = DateTime.Now,
                        LimiteExpireTime = DateTime.Now.AddDays(1),
                        LimiteReason = "异常请求频次",
                        Isdelete = 0,
                        LastModified = DateTime.Now,
                    };
                    // 获取 DbSet<T>
                    var dbSet = _codeContext.Set<LimiteRecord>();
                    // 添加实体
                    await dbSet.AddAsync(_record);
                    // 保存更改
                    await _codeContext.SaveChangesAsync();
                    #endregion
                    throw new RpcException(new Status(StatusCode.PermissionDenied, $"【{ipAddress}】请求被限制"));
                }

                // 增加请求计数
                _cache.Set(cacheKey, requestCount + 1);

                // IP 限制逻辑-白名单校验
                //if (!AllowedIps.Contains(ipAddress))
                //{
                //throw new RpcException(new Status(StatusCode.PermissionDenied, $"【{ipAddress}】请求被拒绝(未在白名单中)"));
                //}
                // JWT 认证
                //需要认证的
                // 获取当前方法的 Endpoint
                var endpoint = context.GetHttpContext().GetEndpoint();

                // 检查是否允许匿名访问
                if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() == null)
                {
                    // 使用已注入的 JWT 认证服务进行验证
                    var token = GetJwtTokenFromHeaders(context.RequestHeaders);
                    if (string.IsNullOrEmpty(token))
                    {
                        throw new RpcException(new Status(StatusCode.Unauthenticated, "未提供认证令牌"));
                    }
                    // 使用已注入的 JWT 认证服务进行验证
                    var httpContext = _httpContextAccessor.HttpContext;
                    var authenticateResult = await httpContext.AuthenticateAsync();

                    if (!authenticateResult.Succeeded)
                    {
                        Log.Warning("JWT 认证失败: {Message}", authenticateResult.Failure?.Message);
                        throw new RpcException(new Status(StatusCode.Unauthenticated, "认证令牌无效"));
                    }
                }
                // 记录请求开始时间
                var watch = Stopwatch.StartNew();
                try
                {
                    //获取User信息
                    UserModel? User = await GetUser(context.RequestHeaders);
                    if (User != null)
                    {
                        context.UserState["User"] = User;
                    }
                    // 继续处理请求
                    var response = await continuation(request, context);

                    // 记录响应日志
                    Log.Information("请求完成: {Method} from {IP}", context.Method, ipAddress);

                    return response;
                }
                catch (Exception ex)
                {
                    // 异常监听逻辑
                    LogException(ex, context);
                    throw new RpcException(new Status(StatusCode.Internal, $"[GetUser]拦截器中请求产生了异常{ex.StackTrace.ToString()}"));
                }
                finally
                {
                    // 结束计时
                    watch.Stop();
                    // 记录响应时间（单位：毫秒）
                    var responseTime = watch.ElapsedMilliseconds;

                    //将响应时间长的接口日志信息存入数据库，用于分析和排查(比如响应时间超过5s的)
                    if (responseTime >= 5000)
                    {
                        #region 写入数据库日志记录
                        await Task.Run(async () =>
                        {
                            #region 写入数据库
                            ResponseAnalysis _rsp = new ResponseAnalysis
                            {
                                RequestTime = DateTime.Now,
                                RequestIp = ipAddress,
                                RequestHost = context.Host,
                                RequestMethod = context.Method,
                                RequestPeer = context.Peer,
                                ResponseTime = responseTime,
                                Isdelete = 0,
                                LastModified = DateTime.Now,
                            };
                            // 获取 DbSet<T>
                            var dbSet = _codeContext.Set<ResponseAnalysis>();
                            // 添加实体
                            await dbSet.AddAsync(_rsp);
                            // 保存更改
                            await _codeContext.SaveChangesAsync();
                            #endregion
                        });
                        #endregion
                        LogException(new Exception($"接口响应时间【{responseTime.ToString()}毫秒】,超过限制时常，需优化"), context);
                    }

                }
            }
            catch(Exception ex)
            {
                LogException(ex, context);
                throw new RpcException(new Status(StatusCode.Internal, $"拦截器中发生了一个未知异常{ex.StackTrace.ToString()}"));
            }
        }

        private void LogException(Exception ex, ServerCallContext context)
        {

            // 构建日志内容
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Exception Message: {ex.Message}");
            stringBuilder.AppendLine($"StackTrace: {ex.StackTrace}");
            stringBuilder.AppendLine($"Method: {context.Method}");
            stringBuilder.AppendLine($"Peer: {context.Peer}");
            stringBuilder.AppendLine($"Host: {context.Host}");

            // 记录异常日志
            Log.Information(stringBuilder.ToString());
            // 上报异常到 Exceptionless
            ex.ToExceptionless()
              .SetProperty("Method", context.Method)
              .SetProperty("Peer", context.Peer)
              .SetProperty("Host", context.Host)
              .Submit();
        }
        /// <summary>
        /// 获取jwt令牌
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        private string GetJwtTokenFromHeaders(Metadata headers)
        {
            // 从 gRPC 请求头中提取 JWT 令牌
            var authorizationHeader = headers.FirstOrDefault(h => h.Key == "authorization")?.Value;
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return null;
            }

            // 去掉 "Bearer " 前缀
            if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authorizationHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }
        /// <summary>
        /// 如果是需要JWT验证的，还需要获取用户信息
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        private async Task<UserModel?> GetUser(Metadata headers)
        {
            var yhid = headers.FirstOrDefault(h => h.Key == "yhid")?.Value;
            var operatorid= headers.FirstOrDefault(h => h.Key == "operatorid")?.Value;
            if (yhid != null && operatorid != null)
            {
                UserModel model = await _operatorService.GetOperator(yhid, operatorid);
                return model;
            }
            return null;
        }
    }

}
