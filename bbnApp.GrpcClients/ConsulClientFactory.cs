using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

namespace bbnApp.GrpcClients
{
    public class ConsulClientFactory : IDisposable
    {
        private readonly ConcurrentDictionary<string, IConsulClient> _clients = new();
        private readonly IReadOnlyDictionary<string, GrpcClusterConfig> _clusterConfigs;
        private readonly ILogger<ConsulClientFactory> _logger;
        private readonly IOptions<ConsulConfig> consulconfig;
        public ConsulClientFactory(
            IOptions<ConsulConfig> consulconfig,
            ILogger<ConsulClientFactory> logger)
        {
            this.consulconfig = consulconfig;
            _clusterConfigs = consulconfig.Value.GrpcClusters.ToDictionary(c => c.Name);
            _logger = logger;
        }

        public IConsulClient GetClient(string clusterName)
        {
            if (string.IsNullOrEmpty(clusterName))
                throw new ArgumentNullException(nameof(clusterName));

            return _clients.GetOrAdd(clusterName, name =>
            {
                if (!_clusterConfigs.TryGetValue(name, out var config))
                {
                    _logger.LogError("Consul cluster '{ClusterName}' not configured", name);
                    throw new ArgumentException($"Consul cluster '{name}' not found in config");
                }

                _logger.LogDebug("Creating Consul client for cluster '{ClusterName}' with address {Address}",
                    name, consulconfig.Value.Address);

                return new ConsulClient(cfg =>
                {
                    cfg.Address = new Uri(consulconfig.Value.Address);
                    cfg.Datacenter = consulconfig.Value.Datacenter;
                    // 可添加其他Consul客户端配置
                    cfg.WaitTime = TimeSpan.FromSeconds(5);
                });
            });
        }

        public void Dispose()
        {
            foreach (var (name, client) in _clients)
            {
                try
                {
                    client.Dispose();
                    _logger.LogDebug("Disposed Consul client for cluster '{ClusterName}'", name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disposing Consul client for cluster '{ClusterName}'", name);
                }
            }
            _clients.Clear();
        }
    }

    public class GrpcClusterConfig
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string ServiceName { get; set; }

        [Url]
        public string FallbackAddress { get; set; }
    }

    public class ConsulConfig
    {
        [Required]
        [Url]
        public string Address { get; set; }
        [Required]
        public string Datacenter { get; set; }

        [ValidateEnumerable]
        public List<GrpcClusterConfig> GrpcClusters { get; set; } = new();
    }

    // 自定义验证属性
    public class ValidateEnumerableAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            if (value is IEnumerable enumerable && !enumerable.GetEnumerator().MoveNext())
            {
                return new ValidationResult($"{context.DisplayName} must contain at least one cluster config");
            }
            return ValidationResult.Success;
        }
    }


}
