using Consul;

namespace bbnApp.Service.GlobalService
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly IConfiguration _configuration;
        private readonly IHostApplicationLifetime _appLifetime;
        private string _serviceId;
        private readonly ILogger<ConsulHostedService> _logger;

        public ConsulHostedService(
            IConsulClient consulClient,
            IConfiguration configuration,
            IHostApplicationLifetime appLifetime,
            ILogger<ConsulHostedService> logger
            )
        {
            _consulClient = consulClient;
            _configuration = configuration;
            _appLifetime = appLifetime;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

            try
            {
                // 从配置获取服务信息
                var grpcConfig = _configuration.GetSection("Kestrel:Endpoints:Http");
                var serviceAddress = grpcConfig["Url"].Replace("http://", "").Replace("https://", "");
                var uri = new Uri($"http://{serviceAddress}");

                _serviceId = $"bbn-grpc-service-{Guid.NewGuid()}";

                var registration = new AgentServiceRegistration
                {
                    ID = _serviceId,
                    Name = "bbn-grpc-service",
                    Address = uri.Host,
                    Port = uri.Port,
                    Check = new AgentServiceCheck
                    {
                        GRPC = $"{uri.Host}:{uri.Port}",
                        GRPCUseTLS = false,
                        Interval = TimeSpan.FromSeconds(20),
                        Timeout = TimeSpan.FromSeconds(5),
                        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)
                    }
                };

                // 注册服务
                var result = await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
                _logger.LogInformation("Service registered successfully with Consul. Service ID: {ServiceId}", _serviceId);

                // 应用停止时注销服务
                _appLifetime.ApplicationStopping.Register(async () =>
                {
                    try
                    {
                        await _consulClient.Agent.ServiceDeregister(_serviceId);
                        _logger.LogInformation("Service deregistered successfully");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to deregister service");
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register service with Consul");
                throw; // 重新抛出异常，让宿主知道启动失败
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _consulClient.Agent.ServiceDeregister(_serviceId);
        }
    }

}
