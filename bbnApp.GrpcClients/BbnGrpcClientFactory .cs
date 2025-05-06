using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;

namespace bbnApp.GrpcClients
{
    public class BbnGrpcClientFactory : IGrpcClientFactory
    {
        private readonly IConfiguration _configuration;

        public BbnGrpcClientFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TClient CreateClient<TClient>() where TClient : ClientBase<TClient>
        {
            var channel = CreateGrpcChannel(_configuration);
            return (TClient)Activator.CreateInstance(typeof(TClient), channel);
        }

        private GrpcChannel CreateGrpcChannel(IConfiguration configuration)
        {
            var grpcUrl = configuration.GetSection("Grpc:Url").Value;

            if (string.IsNullOrEmpty(grpcUrl))
            {
                throw new InvalidOperationException("gRPC URL is not configured.");
            }

            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };

            return GrpcChannel.ForAddress(
                grpcUrl,
                new GrpcChannelOptions { HttpHandler = httpClientHandler }
            );
        }
    }

}
