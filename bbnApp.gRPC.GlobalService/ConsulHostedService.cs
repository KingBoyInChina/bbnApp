using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

namespace bbnApp.gRPC.GlobalService
{
    public class ConsulHostedService : IHostedService
    {
        private readonly IConsulClient _consulClient;
        private readonly IConfiguration _configuration;
        private readonly IHostApplicationLifetime _appLifetime;
        //private string _serviceId;
        private List<string> _serviceIds=new List<string>();
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
                // 从配置获取服务信息（localhost）
                //var grpcConfig = _configuration.GetSection("Kestrel:Endpoints:Http");
                //var serviceAddress = grpcConfig["Url"].Replace("http://", "").Replace("https://", "");

                //var serviceAddress = _configuration.GetSection("Consul:Endpoints:Http");
                //var uri = new Uri($"http://{serviceAddress}");

                //_serviceId = $"bbn-grpc-service-{Guid.NewGuid()}";

                //var registration = new AgentServiceRegistration
                //{
                //    ID = _serviceId,
                //    Name = "bbn-grpc-service",
                //    Address = uri.Host,
                //    Port = uri.Port,
                //    Check = new AgentServiceCheck
                //    {
                //        GRPC = $"{uri.Host}:{uri.Port}",
                //        GRPCUseTLS = false,
                //        Interval = TimeSpan.FromSeconds(20),
                //        Timeout = TimeSpan.FromSeconds(5),
                //        DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(30)
                //    }
                //};
                // 注册服务
                //var result = await _consulClient.Agent.ServiceRegister(registration, cancellationToken);
                //_logger.LogInformation("Service registered successfully with Consul. Service ID: {ServiceId}", _serviceId);
                //多服务循环注册
                var consulConfig = _configuration.GetSection("Consul");
                var consulClient = new ConsulClient(c => c.Address = new Uri(consulConfig["Url"]));

                foreach (var serviceConfig in consulConfig.GetSection("Services").GetChildren())
                {
                    var uri = new Uri(serviceConfig["Address"]);
                    string id = $"{serviceConfig["Name"]}-{Guid.NewGuid()}";
                    var registration = new AgentServiceRegistration
                    {
                        ID = id,
                        Name = serviceConfig["Name"],
                        Address = serviceConfig["IP"]?.ToString()?? Dns.GetHostName(), // 
                        Port = uri.Port,
                        Check = new AgentServiceCheck
                        {
                            GRPC = $"{uri.Host}:{uri.Port}", // 确保是 gRPC 实际监听地址
                            GRPCUseTLS = false,              // 如果未启用 TLS
                            Interval = TimeSpan.FromSeconds(10),
                            Timeout = TimeSpan.FromSeconds(5),
                            DeregisterCriticalServiceAfter = TimeSpan.FromMinutes(1)
                        }
                    };
                    _serviceIds.Add(id);
                    await consulClient.Agent.ServiceRegister(registration);
                }


                // 应用停止时注销服务
                _appLifetime.ApplicationStopping.Register(async () =>
                {
                    try
                    {
                        //await _consulClient.Agent.ServiceDeregister(_serviceId);
                        foreach(string id in _serviceIds)
                        {
                            await _consulClient.Agent.ServiceDeregister(id);
                        }
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
            foreach (string id in _serviceIds)
            {
                await _consulClient.Agent.ServiceDeregister(id);
            }
        }
    }

}
