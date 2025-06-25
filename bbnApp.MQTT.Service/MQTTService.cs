using AutoMapper;
using bbnApp.Core;
using bbnApp.Domain.Entities.Business;
using bbnApp.DTOs.CodeDto;
using bbnApp.GrpcClients;
using bbnApp.Protos;
using Exceptionless;
using Grpc.Core;
using Grpc.Net.ClientFactory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Diagnostics.Logger;
using MQTTnet.Protocol;
using MQTTnet.Server;
using Newtonsoft.Json;
using Serilog.Core;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.MQTT.Service;


/// <summary>
/// MQTT服务器托管服务
/// </summary>
public class MqttServerHostedService : IHostedService
{
    /// <summary>
    /// 
    /// </summary>
    private readonly ExceptionlessClient exceptionlessClient;
    /// <summary>
    /// 服务
    /// </summary>
    private readonly MqttServer _mqttServer;
    /// <summary>
    /// 日志
    /// </summary>
    private readonly ILogger<MqttServerHostedService> _logger;
    /// <summary>
    /// 有效身份的客户端ID集合，用于快速校验身份有效性
    /// </summary>
    private ImmutableList<AuthorReginsterKeyClientDto> _validClients =ImmutableList<AuthorReginsterKeyClientDto>.Empty;
    /// <summary>
    /// 当前连接的MQTT客户端集合，使用ConcurrentDictionary以支持多线程访问
    /// </summary>
    private readonly ConcurrentDictionary<string, ClientConnectedEventArgs> _connectionClients = new();
    /// <summary>
    /// 配置参数
    /// </summary>
    private readonly IConfiguration _configuration;
    /// <summary>
    /// 
    /// </summary>
    private readonly IRedisService redisService;
    
    public MqttServerHostedService(MqttServer mqttServer, ILogger<MqttServerHostedService> logger, IConfiguration configuration , IRedisService redisService, ExceptionlessClient exceptionlessClient)
    {
        _mqttServer = mqttServer;
        _logger = logger;
        _configuration = configuration;
        this.exceptionlessClient = exceptionlessClient;
        this.redisService = redisService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 设置事件处理器
        _mqttServer.ClientConnectedAsync += OnClientConnected;
        _mqttServer.ClientDisconnectedAsync += OnClientDisconnected;
        _mqttServer.InterceptingPublishAsync += OnMessageReceived;
        _mqttServer.StartedAsync += OnServerStarted;
        _mqttServer.ValidatingConnectionAsync += OnValidatingConnection;
        // 启动服务器
        await _mqttServer.StartAsync();
        _logger.LogInformation("MQTT server started on port 1883");
        await RegisterClientsInit();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _mqttServer.StopAsync();
        _logger.LogInformation("MQTT server stopped");
    }

    private Task OnServerStarted(EventArgs args)
    {
        _logger.LogInformation("MQTT server is ready");
        return Task.CompletedTask;
    }

    private Task OnClientConnected(ClientConnectedEventArgs args)
    {
        // 通过发布消息事件捕获连接上下文（可选）
        _connectionClients.AddOrUpdate(
            args.ClientId,
            args,
            (_, __) => args); // 始终更新为最新上下文
        _logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}Client connected: {args.ClientId}");
        return Task.CompletedTask;
    }

    private Task OnClientDisconnected(ClientDisconnectedEventArgs args)
    {
        // 从连接客户端集合中移除
        _connectionClients.TryRemove(args.ClientId, out _);
        _logger.LogInformation($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")}Client disconnected: {args.ClientId}");
        return Task.CompletedTask;
    }
    /// <summary>
    /// 接收到消息时的处理方法
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private Task OnMessageReceived(InterceptingPublishEventArgs args)
    {
        var payload = args.ApplicationMessage?.Payload == null
            ? null
            : Encoding.UTF8.GetString(args.ApplicationMessage.Payload);
        string topic = args.ApplicationMessage?.Topic ?? string.Empty;
        #region 平台对服务端的一些特殊操作,这里后面应该需要加上是否是平台运维人员的身份验证
        if (topic.Contains("/MqttService/Restart"))
        {
            #region 服务重启
            try
            {
                // 获取当前程序路径
                string exePath = Environment.ProcessPath ?? string.Empty;//  Process.GetCurrentProcess().MainModule.FileName;
                if (!string.IsNullOrEmpty(exePath))
                {
                    // 启动新实例
                    Process.Start(exePath);
                    // 退出当前程序
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {

            }
            #endregion
        }
        else if (topic.Contains("/MqttService/Disconnected"))
        {
            #region 断开连接
            if (payload == "all")
            {
                _connectionClients.Clear();
            }
            else if (!string.IsNullOrEmpty(payload))
            {
                _connectionClients.TryRemove(payload, out _);
            }

            var message = new MqttApplicationMessageBuilder().WithTopic($"/private/{args.ClientId}/Notice").WithPayload($"服务端连接用户【{payload}】断开完成").Build();

            _ = _mqttServer.InjectApplicationMessage(
                new InjectedMqttApplicationMessage(message)
                {
                    SenderClientId = "root"
                });
            #endregion
        }
        else if (topic.Contains("/MqttService/RegisterClients"))
        {
            #region 重载注册用户
            _ = RegisterClientsInit();
            var message = new MqttApplicationMessageBuilder().WithTopic($"/private/{args.ClientId}/Notice").WithPayload("服务端注册用户重载完成").Build();

            _ = _mqttServer.InjectApplicationMessage(
                new InjectedMqttApplicationMessage(message)
                {
                    SenderClientId = "root"
                });
            #endregion
        }
        else if (topic.Contains("/MqttService/GetClients"))
        {
            #region 得到实时连接信息(一般由平台触发,数据也只需返回触发对象获取)
            string data = JsonConvert.SerializeObject(_validClients);
            var message = new MqttApplicationMessageBuilder().WithTopic($"/Private/Operator/{args.ClientId}/GetClients").WithPayload(data).Build();

            _ = _mqttServer.InjectApplicationMessage(
                new InjectedMqttApplicationMessage(message)
                {
                    SenderClientId = "root"
                });
            #endregion
        }
        #endregion
        _logger.LogInformation("Message received on topic {Topic}: {Payload}", args.ApplicationMessage?.Topic, payload);

        return Task.CompletedTask;
    }
    /// <summary>
    /// 身份验证
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    private Task OnValidatingConnection(ValidatingConnectionEventArgs e)
    {
        //设备与密钥绑定后再判断
        if (!_validClients.Any(x => x.AppId == e.UserName && x.SecriteKey == e.Password))//&&x.ClientId==e.ClientId
        {
            e.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
        }
        return Task.CompletedTask;
    }
    /// <summary>
    /// 初始化系统注册的用户(改从redis中读取，避免访问接口模式还必须得有先后顺序)
    /// </summary>
    /// <returns></returns>
    private async Task RegisterClientsInit()
    {
        try
        {
            var registerAuthors =await redisService.GetAsync("RegisterKeys");
            if (!string.IsNullOrEmpty(registerAuthors))
            {
                List<AuthorReginsterKeyClientDto> list = JsonConvert.DeserializeObject<List<AuthorReginsterKeyClientDto>>(registerAuthors);
                _validClients = list.ToImmutableList();
            }
        }
        catch(Exception ex)
        {
            exceptionlessClient.SubmitException(ex);
        }
    }
}

/// <summary>
/// 健康检查托管服务
/// </summary>
public class HealthCheckHostedService : IHostedService
{
    private readonly HealthCheckService _healthCheckService;
    private readonly ILogger<HealthCheckHostedService> _logger;
    private Timer? _timer;

    public HealthCheckHostedService(HealthCheckService healthCheckService, ILogger<HealthCheckHostedService> logger)
    {
        _healthCheckService = healthCheckService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Health check service started");

        // 每10秒执行一次健康检查
        _timer = new Timer(DoHealthCheck, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    private async void DoHealthCheck(object? state)
    {
        try
        {
            var result = await _healthCheckService.CheckHealthAsync();
            _logger.LogInformation("Health check status: {Status}", result.Status);

            foreach (var entry in result.Entries)
            {
                _logger.LogInformation("{Key}: {Status}", entry.Key, entry.Value.Status);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
        }
    }
}

/// <summary>
/// 健康检查实现
/// </summary>
public class MqttHealthCheck : IHealthCheck
{
    private readonly MqttServer _mqttServer;

    public MqttHealthCheck(MqttServer mqttServer)
    {
        _mqttServer = mqttServer;
    }

    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 检查MQTT服务器是否正在运行
            if (_mqttServer.IsStarted)
            {
                var activeConnections = _mqttServer.GetClientsAsync().GetAwaiter().GetResult().Count;

                var data = new Dictionary<string, object>
                {
                    { "active_connections", activeConnections },
                    { "server_status", "running" }
                };

                return Task.FromResult(
                    HealthCheckResult.Healthy("MQTT server is running", data));
            }

            return Task.FromResult(
                HealthCheckResult.Unhealthy("MQTT server is not running"));
        }
        catch (Exception ex)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy("MQTT server health check failed", ex));
        }
    }
}

class ConsoleLogger : IMqttNetLogger
{
    readonly object _consoleSyncRoot = new();

    public bool IsEnabled => true;

    public void Publish(MqttNetLogLevel logLevel, string source, string message, object[]? parameters, Exception? exception)
    {
        var foregroundColor = ConsoleColor.White;
        switch (logLevel)
        {
            case MqttNetLogLevel.Verbose:
                foregroundColor = ConsoleColor.White;
                break;

            case MqttNetLogLevel.Info:
                foregroundColor = ConsoleColor.Green;
                break;

            case MqttNetLogLevel.Warning:
                foregroundColor = ConsoleColor.DarkYellow;
                break;

            case MqttNetLogLevel.Error:
                foregroundColor = ConsoleColor.Red;
                break;
        }

        if (parameters?.Length > 0)
        {
            message = string.Format(message, parameters);
        }

        lock (_consoleSyncRoot)
        {
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message);

            if (exception != null)
            {
                Console.WriteLine(exception);
            }
        }
    }
}