using bbnApp.Common.Models;
using bbnApp.InfluxDb.Client;
using bbnApp.Share;
using Exceptionless;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.RabbitMQ.Consumer
{
    /// <summary>
    /// 
    /// </summary>
    public class RabbitMqConsumerService
    {
        private InfluxDbService _influxDbMulClient;
        /// <summary>
        /// MQ模型
        /// </summary>
        private RabbitMQModel MQModel;
        private InfluxDbModel InfluxModel;
        /// <summary>
        /// 
        /// </summary>
        private RabbitMQConsumer consumer;
        /// <summary>
        /// 
        /// </summary>
        private readonly IConfiguration configuration;

        private ExceptionlessClient exceptionlessClient;
        /// <summary>
        /// 这里需要注入influxDbService
        /// </summary>
        /// <param name="influxDbService"></param>
        public RabbitMqConsumerService(IConfiguration configuration, ExceptionlessClient exceptionlessClient)
        {
            this.configuration = configuration;
            this.exceptionlessClient = exceptionlessClient;
        }

        /// <summary>
        /// 消费者初始化
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            try
            {
                //开发阶段不加密
                //string mqttSetting = EncodeAndDecode.MixDecrypt(configuration["RabbitMQ"].ToString()); 
                //MQModel = JsonConvert.DeserializeObject<RabbitMQModel>(mqttSetting);
                MQModel= configuration.GetSection("RabbitMQ").Get<RabbitMQModel>();

                consumer = new RabbitMQConsumer(MQModel);
                await consumer.Init();
                await consumer.StartConsuming(ProcessMessageAndWriteToInfluxDb);

                _influxDbMulClient = new InfluxDbService(exceptionlessClient);
                //开发阶段不加密
                //string influxdbSetting = EncodeAndDecode.MixDecrypt(configuration["InfluxDb"].ToString()); 
                //InfluxModel = JsonConvert.DeserializeObject<InfluxDbModel>(influxdbSetting);
                InfluxModel = configuration.GetSection("InfluxDb").Get<InfluxDbModel>();

                _influxDbMulClient.StartAsync(InfluxModel);
                Console.WriteLine($"RabbitMq Consumer启动完成");
            }
            catch (Exception ex)
            {
                exceptionlessClient.CreateLog(ex.Message.ToString(), "RabbitMq Consumer启动异常").Submit();
                Console.WriteLine($"RabbitMq Consumer启动异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="payload"></param>
        private void ProcessMessageAndWriteToInfluxDb(InfluxDbReciveData Data)
        {
            ///防止阻塞
            Task.Run(async () => {
                try
                {
                    var data = JsonConvert.DeserializeObject<dynamic>(Data.Message);
                    // 获取字典
                    Dictionary<string, string> tags = data.tags.ToObject<Dictionary<string, string>>();
                    Dictionary<string, object> fields = data.fields.ToObject<Dictionary<string, object>>();

                    //写入时序数据库
                    _ = _influxDbMulClient.WriteAsync(
                            BucketName: "Bbn_" + Data.QueueName,
                           QueueName: Data.QueueName,
                           tags: tags,
                           fields: fields
                       );
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            });
        }
    }
}
