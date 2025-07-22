using bbnApp.InfluxDb.Client;
using bbnApp.MQTT.DataReciveClient;
using bbnApp.Service.GlobalService;
using Exceptionless;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Microsoft.Extensions.Logging;

async Task Main()
{
    var host = Host.CreateDefaultBuilder()
        .ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile("appSettings.json", optional: false);
        })
        .ConfigureServices((context, services) =>
        {
            services.AddSingleton(Log.Logger); // 注册 ILogger

            // 注册配置
            services.AddSingleton<IConfiguration>(context.Configuration);

            #region exceptionless 配置

            services.AddSingleton<ExceptionlessClient>(sp =>
            {
                var client = new ExceptionlessClient();
                client.Startup(context.Configuration.GetValue<string>("Exceptionless:ApiKey"));
                client.Configuration.ServerUrl = context.Configuration.GetValue<string>("Exceptionless:ServerUrl");
                return client;
            });
            #endregion
            //注册influxdb服务
            services.AddSingleton<InfluxDbService>();
            // 注册MQTT
            services.AddSingleton<MqttReciveService>();
        })
        .ConfigureLogging(logging =>
        {
            logging.AddConsole();
        })
        .Build();

    // === 获取服务并调用方法 ===
    using (var scope = host.Services.CreateScope())
    {
        var mqttService = scope.ServiceProvider.GetRequiredService<MqttReciveService>();
        await mqttService.StartAsync();
    }

    await host.RunAsync();
}

await Main();