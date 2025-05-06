using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.GrpcClients
{
    public interface IGrpcClientFactory
    {
        TClient CreateClient<TClient>() where TClient : ClientBase<TClient>;
    }
}
