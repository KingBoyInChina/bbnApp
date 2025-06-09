using Grpc.Core;
using Grpc.Health.V1;

namespace bbnApp.Service.GlobalService
{
    public class HealthCheckService : Health.HealthBase
    {
        public override Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HealthCheckResponse
            {
                Status = HealthCheckResponse.Types.ServingStatus.Serving
            });
        }
    }
}
