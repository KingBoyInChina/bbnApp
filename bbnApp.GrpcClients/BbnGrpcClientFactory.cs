using Consul;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace bbnApp.GrpcClients
{
    public class GrpcClientFactory : IGrpcClientFactory,IDisposable
    {
        private readonly IConsulClient _consul;
        private readonly IReadOnlyDictionary<string, GrpcClusterConfig> _clusters;
        private readonly ConcurrentDictionary<string, GrpcChannel> _channelCache = new();
        private readonly ILogger<GrpcClientFactory> _logger;

        public GrpcClientFactory(IConsulClient consul, IOptions<ConsulConfig> config, ILogger<GrpcClientFactory> logger)
        {
            _consul = consul;
            _clusters = config.Value.GrpcClusters.ToDictionary(c => c.Name);
            _logger = logger;
        }

        public async Task<TClient> CreateClient<TClient>(
        string clusterName = "Basic",
        CancellationToken cancellationToken = default)
        where TClient : ClientBase<TClient>
        {
            if (!_clusters.TryGetValue(clusterName, out var clusterConfig))
            {
                _logger.LogError("Cluster {ClusterName} not configured", clusterName);
                throw new ArgumentException($"Cluster {clusterName} not configured");
            }

            try
            {
                var serviceAddress = clusterConfig.FallbackAddress;//开发环境下，使用GetServiceAddressAsync 一致卡着，因此先跳过
                //var serviceAddress = await GetServiceAddressAsync(clusterConfig, cancellationToken);
                var channel = _channelCache.GetOrAdd(serviceAddress, addr => CreateChannel(addr));

                _logger.LogDebug("Created gRPC client for {Type} via {Address}",
                    typeof(TClient).Name, serviceAddress);

                return (TClient)Activator.CreateInstance(typeof(TClient), channel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create gRPC client {Type} for cluster {Cluster}",
                    typeof(TClient).Name, clusterName);
                throw;
            }
        }

        private GrpcChannel CreateChannel(string address)
        {
            return GrpcChannel.ForAddress(address, new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                }
            });
        }

        private async Task<string> GetServiceAddressAsync(GrpcClusterConfig config,CancellationToken cts=default)
        {
            try
            {
                var services = await _consul.Health.Service(config.ServiceName, "", true, cts);

                var healthyServices = services.Response?
                    .Where(x => x.Checks.All(c => c.Status == HealthStatus.Passing))
                    .Select(x => $"http://{x.Service.Address}:{x.Service.Port}")
                    .ToList();

                return healthyServices?.Count > 0
                    ? healthyServices[Random.Shared.Next(healthyServices.Count)]
                    : config.FallbackAddress;
            }
            catch
            {
                return config.FallbackAddress;
            }
        }
        
        public void Dispose()
        {
            foreach (var channel in _channelCache.Values)
            {
                channel.Dispose();
            }
            _channelCache.Clear();
        }
    }
}
