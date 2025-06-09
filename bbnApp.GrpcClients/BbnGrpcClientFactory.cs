using Consul;
using Exceptionless;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace bbnApp.GrpcClients
{
    #region GRPC Client Factory Interface 静态连接
    //public class BbnGrpcClientFactory : IGrpcClientFactory
    //{
    //    private readonly IConfiguration _configuration;

    //    public BbnGrpcClientFactory(IConfiguration configuration)
    //    {
    //        _configuration = configuration;
    //    }

    //    public TClient CreateClient<TClient>() where TClient : ClientBase<TClient>
    //    {
    //        var channel = CreateGrpcChannel(_configuration);
    //        return (TClient)Activator.CreateInstance(typeof(TClient), channel);
    //    }

    //    private GrpcChannel CreateGrpcChannel(IConfiguration configuration)
    //    {
    //        var grpcUrl = configuration.GetSection("Grpc:Url").Value;

    //        if (string.IsNullOrEmpty(grpcUrl))
    //        {
    //            throw new InvalidOperationException("gRPC URL is not configured.");
    //        }

    //        var httpClientHandler = new HttpClientHandler
    //        {
    //            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    //        };

    //        return GrpcChannel.ForAddress(
    //            grpcUrl,
    //            new GrpcChannelOptions { HttpHandler = httpClientHandler }
    //        );
    //    }
    //}
    #endregion

    public class BbnGrpcClientFactory : IGrpcClientFactory, IDisposable
    {
        private readonly IConsulClient _consulClient;
        private List<ServiceEntry> _serviceCache = new();
        private DateTime _lastRefreshTime = DateTime.MinValue;
        private readonly object _cacheLock = new();
        private readonly ConcurrentDictionary<string, int> _serviceConnections = new();
        private readonly object _loadBalancerLock = new(); private readonly ExceptionlessClient _exceptionlessClient;

        public BbnGrpcClientFactory(IConsulClient consulClient,  ExceptionlessClient exceptionlessClient)
        {
            _consulClient = consulClient;
            _exceptionlessClient = exceptionlessClient;
        }


        public async Task<TClient> CreateClient<TClient>() where TClient : ClientBase<TClient>
        {
            await RefreshServiceCacheIfNeeded();
            var service = GetLeastLoadedService();
            var channel = GrpcChannel.ForAddress($"http://{service.Address}:{service.Port}");
            return (TClient)Activator.CreateInstance(typeof(TClient), channel);
        }

        private async Task RefreshServiceCacheIfNeeded()
        {
            if ((DateTime.UtcNow - _lastRefreshTime).TotalSeconds <= 30)
            {
                return;
            }

            lock (_cacheLock)
            {
                if ((DateTime.UtcNow - _lastRefreshTime).TotalSeconds <= 30)
                {
                    return;
                }

                try
                {
                    var queryResult = _consulClient.Health.Service("bbn-grpc-service", "", true).Result;
                    lock (_cacheLock)
                    {
                        _serviceCache = queryResult.Response?.ToList() ?? new List<ServiceEntry>();
                        _lastRefreshTime = DateTime.UtcNow;
                    }
                }
                catch (Exception ex)
                {
                    _exceptionlessClient.SubmitException(new Exception($"RefreshServiceCacheIfNeeded 异常:{ex.Message.ToString()}"));
                }
            }
        }

        private AgentService GetLeastLoadedService()
        {
            lock (_loadBalancerLock)
            {
                // 如果缓存为空，尝试同步刷新一次
                
                if (!_serviceCache.Any())
                {
                    _exceptionlessClient.SubmitException(new Exception($"GetLeastLoadedService 异常:No available gRPC services"));
                    throw new Exception("No available gRPC services");
                }

                var healthyServices = _serviceCache
                    .Where(entry => entry.Checks.All(check => check.Status == HealthStatus.Passing))
                    .Select(entry => entry.Service)
                    .ToList();

                if (!healthyServices.Any())
                {
                    _exceptionlessClient.SubmitException(new Exception($"GetLeastLoadedService 异常:No healthy gRPC services available"));
                    throw new Exception("No healthy gRPC services available");
                }

                var selectedService = healthyServices
                    .OrderBy(service => _serviceConnections.GetOrAdd(service.ID, 0))
                    .First();

                _serviceConnections.AddOrUpdate(selectedService.ID, 1, (_, count) => count + 1);
                return selectedService;
            }
        }

        public void ReleaseService(string serviceId)
        {
            _serviceConnections.AddOrUpdate(serviceId, 0, (_, count) => Math.Max(0, count - 1));
        }

        public void Dispose()
        {
            _serviceConnections.Clear();
        }
    }
}
