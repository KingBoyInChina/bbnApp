using bbnApp.Core;
using bbnApp.MQTT.Service;
using bbnApp.Service.GlobalService;
using Exceptionless;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MQTTnet.Server;
using Serilog;

async Task Main()
{
    var host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile("appSetting.json", optional: false);
        })
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton(Log.Logger); // 注册 ILogger

            // 注册配置
            services.AddSingleton<IConfiguration>(context.Configuration);

            // 注册 AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            #region exceptionless 配置

            services.AddSingleton<ExceptionlessClient>(sp =>
            {
                var client = new ExceptionlessClient();
                client.Startup(context.Configuration.GetValue<string>("Exceptionless:ApiKey"));
                client.Configuration.ServerUrl = context.Configuration.GetValue<string>("Exceptionless:ServerUrl");
                return client;
            });
            #endregion

            //注册Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = context.Configuration.GetSection("Redis:ConnectionString").Value; // Redis 连接字符串
            });

            // 注册 Redis 服务
            services.AddScoped<IRedisService, RedisService>();

            // 注册MQTT服务器
            services.AddSingleton<MqttServer>(provider =>
            {
                var factory = new MqttServerFactory(new ConsoleLogger());
                var options = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                    .WithDefaultEndpointPort(context.Configuration.GetValue<int>("MQTT:Port"))
                    .Build();
                return factory.CreateMqttServer(options);
            });

            // 注册托管服务
            services.AddHostedService<HealthCheckHostedService>();

            // 注册健康检查
            services.AddHealthChecks()
                .AddCheck<MqttHealthCheck>("mqtt_server");

            services.AddHostedService<MqttServerHostedService>();

        })
        .ConfigureLogging(logging =>
        {
            logging.AddConsole();
        })
        .Build();
    await host.RunAsync();
}

await Main();