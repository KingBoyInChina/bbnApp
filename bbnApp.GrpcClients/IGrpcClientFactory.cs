using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.GrpcClients
{
    /// <summary>
    /// 单例
    /// </summary>
    public interface IGrpcClientFactory
    {
       Task<TClient> CreateClient<TClient>(string clusterName = "Basic", CancellationToken cancellationToken = default)
       where TClient : ClientBase<TClient>;
    }
}
