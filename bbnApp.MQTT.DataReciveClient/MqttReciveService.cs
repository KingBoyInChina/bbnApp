using bbnApp.DTOs.BusinessDto;
using bbnApp.RabbitMQ;
using Exceptionless;
using Microsoft.Extensions.Configuration;
using MQTTnet;
using MQTTnet.Extensions.TopicTemplate;
using MQTTnet.Protocol;
using Newtonsoft.Json;
using NodaTime.Extensions;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Text;

namespace bbnApp.MQTT.DataReciveClient
{
    public class MqttReciveService
    {
        // 在类级别添加缓存字典
        private static readonly ConcurrentDictionary<string, (DateTime timestamp, Dictionary<string, object> tempHumData, Dictionary<string, object> lightData)>
            sensorDataCache = new ConcurrentDictionary<string, (DateTime, Dictionary<string, object>, Dictionary<string, object>)>();
        List<MqttTopicTemplate> Topics;
        /// <summary>
        /// 
        /// </summary>
        MqttClientFactory mqttFactory = new MqttClientFactory();
        /// <summary>
        /// 
        /// </summary>
        IMqttClient mqttClient;
        /// <summary>
        /// 
        /// </summary>
        private RabbitMQProducer producer;
        /// <summary>
        /// 
        /// </summary>
        private readonly IConfiguration configuration;
        /// <summary>
        /// MQ模型
        /// </summary>
        private RabbitMQModel MQModel;
        /// <summary>
        /// 设备清单
        /// </summary>
        private BlockingCollection<UserDeviceListItemDto> _devices = new BlockingCollection<UserDeviceListItemDto>();
        
        private ExceptionlessClient exceptionlessClient;
        /// <summary>
        /// 环境数据设备代码
        /// </summary>
        private string _envDataCodes = string.Empty;
        /// <summary>
        /// 气体数据设备代码
        /// </summary>
        private string _gasDataCodes = string.Empty;
        /// <summary>
        /// 这里需要注入influxDbService
        /// </summary>
        /// <param name="influxDbService"></param>
        public MqttReciveService(IConfiguration configuration, ExceptionlessClient exceptionlessClient)
        {
            this.configuration = configuration;
            this.exceptionlessClient = exceptionlessClient;
        }
        /// <summary>
        /// 采集者初始化
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            try
            {
                mqttFactory = new MqttClientFactory();
                producer = new RabbitMQProducer();
                mqttClient = mqttFactory.CreateMqttClient();

                //string mqttSetting = 开发阶段不加密 EncodeAndDecode.MixDecrypt(configuration["RabbitMQ"].ToString());
                //MQModel = JsonConvert.DeserializeObject<RabbitMQModel>(mqttSetting);
                MQModel= configuration.GetSection("RabbitMQ").Get<RabbitMQModel>();
                _envDataCodes = configuration.GetSection("DataTypeCode:EnvData").Get<string>();
                _gasDataCodes = configuration.GetSection("DataTypeCode:GasData").Get<string>();
                //初始化事件列
                var topics = configuration.GetSection("MQTT:Topics").Get<string[]>();
                Topics = new List<MqttTopicTemplate>();
                foreach (string topic in topics)
                {
                    Topics.Add(new MqttTopicTemplate(topic));
                }
                await SubscribeTopics();
                await PublishInit();
                StartCacheCleanupTask();
            }
            catch(Exception ex)
            {
                exceptionlessClient.CreateLog(ex.Message.ToString(), "MQTTClient启动异常").Submit();
                Console.WriteLine($"MQTTClient启动异常：{ex.Message.ToString()}");
            }
        }

        /// <summary>
        /// 主题订阅
        /// </summary>
        /// <returns></returns>
        public async Task SubscribeTopics()
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(configuration["MQTT:IP"],Convert.ToInt32(configuration["MQTT:Port"]))
                .WithClientId(configuration["MQTT:ClientId"])
                .WithCredentials(configuration["MQTT:AppId"], configuration["MQTT:SecretKey"])
                .Build();

            mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                var info = e.ApplicationMessage;
                string payload = Encoding.UTF8.GetString(info.Payload.ToArray());
                string topic = info.Topic.TrimStart('/');
                string exchange = topic.Split('/')[0];
                string queue = topic.Split('/')[1];

                Console.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} Received application message.{info.Topic},{payload}");
                if (info.Topic == "/Private/Reciver/DataReciver/DeviceData")
                {
                    #region 接收到设备数据,写入内存
                    DeviceDataInit(payload);
                    #endregion
                }
                else if (info.Topic == "/Private/Reciver/DataReciver/Restart")
                {
                    #region 重启
                    Share.CommMethod.applicationRestart();
                    #endregion
                }
                else
                {
                    if (payload == "heart")
                    {
                        //写在线状态
                        _ = ReciveDataParse(exchange, queue, info.Topic, info.Payload.ToArray(), true);
                    }
                    if (info.Topic.StartsWith("/r/hex"))
                    {
                        _ = ReciveDataParse(exchange, queue, info.Topic, info.Payload.ToArray());
                    }
                    else
                    {
                        //写入队列
                        _ = producer.Publish(exchange, queue, payload);
                    }
                }
                return Task.CompletedTask;
            };

            var mqttconnectingresult= await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);


            var builder = mqttFactory.CreateSubscribeOptionsBuilder();
            foreach (var topic in Topics)
            {
                builder.WithTopicTemplate(
                    topic,
                    noLocal: true,
                    retainHandling: MqttRetainHandling.SendAtSubscribe
                );
            }

            var response = await mqttClient.SubscribeAsync(builder.Build(), CancellationToken.None);

            //初始化生产者
            await producer.StartAsync(MQModel);

            Console.WriteLine("主题订阅完成...");

        }
        /// <summary>
        /// 初始化主题发布
        /// </summary>
        /// <returns></returns>
        private async Task PublishInit()
        {
            try
            {
               await mqttClient.PublishStringAsync("/MqttService/GetDevicesData", mqttClient.Options.ClientId);
            }
            catch(Exception ex)
            {
                exceptionlessClient.CreateLog(ex.Message.ToString(), "PublishInit 异常").Submit();
            }
        }
        #region 内存数据
        /// <summary>
        /// 写设备清单
        /// </summary>
        /// <param name="payload"></param>
        private void DeviceDataInit(string payload)
        {
            try
            {
                var list = JsonConvert.DeserializeObject<List<UserDeviceListItemDto>>(payload);
                _devices = new BlockingCollection<UserDeviceListItemDto>(new ConcurrentQueue<UserDeviceListItemDto>(list));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")} DeviceDataInit 异常:{ex.Message.ToString()}");
            }
        }
        #endregion
        #region RS485串口服务器采集的数据格式处理
        /// <summary>
        /// tas-lan-869串口服务器上传数据格式处理
        /// </summary>
        private async Task ReciveDataParse(string exchange, string queue, string topic,byte[] Payload,bool isHeart=false)
        {
            try
            {
                string Number = topic.Split('/')[3];//这里约定第3位为ClientId（即设备编号）
                #region 写数据
                string ParseExchange ="Center";
                string ParseQueue ="";
                var tags = new Dictionary<string, string>();
                var fields = new Dictionary<string, object>();
                if (isHeart)
                {
                    var model = _devices.FirstOrDefault(x => x.Number == Number);
                    if (model != null)
                    {
                        tags = new Dictionary<string, string>
                        {
                            { "UserId", model.UserId},
                            { "Id", model.Id },
                            { "Name", model.Name },
                            { "DeviceId", model.DeviceId },
                            { "Number", model.Number },
                            { "ReciveTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                        };
                        fields = new Dictionary<string, object> {
                            { "Online",true},
                            { "Power",0}//点亮,都是写入MechanicalData
                        };
                        ParseQueue = "MechanicalData";//设备健康状态
                    }
                }
                else
                {
                    // 增加缓存,合并提交
                    if (topic.StartsWith("/r/hex"))
                    {
                        string payload = BitConverter.ToString(Payload.ToArray()).Replace("-", " ");
                        var data = Share.CommMethod.ParseTASSensor(Payload.ToArray(), true);
                        Console.WriteLine($"{DateTime.Now:HH:mm:ss:fff},状态：{data.Item1}，反馈：{data.Item2}，地址：{data.Item3}，数值1：{data.Item4},数值2:{data.Item5}");
                        if (data.Item1)
                        {
                            string qz = "11" + Number.Substring(2);
                            var model = _devices.FirstOrDefault(x => x.Number.StartsWith(qz) && x.SlaveId == data.Item3.ToString());
                            if (model != null)
                            {
                                tags = new Dictionary<string, string>
                                {
                                    { "UserId", model.UserId},
                                    { "Id", model.Id },
                                    { "Name",model.Name },
                                    { "DeviceId", model.DeviceId },
                                    { "SlaveId", data.Item3.ToString() },
                                    { "Number", model.Number },
                                    { "ReciveTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                                };
                                //判断设备类型,如果是 温湿度、光照、土壤湿度、雨滴 数据，写入 EvaData
                                //如果是是气体类数据，写入GasData
                                bool isEnv = _envDataCodes.Contains(model.DeviceId);
                                bool isGas = _gasDataCodes.Contains(model.DeviceId);

                                if (isEnv)
                                {
                                    #region 环境
                                    // 判断设备类型
                                    bool isCombinedSensor = model.DeviceId == "1101006001"; // 温湿度光照一体传感器

                                    if (isCombinedSensor)
                                    {
                                        // 处理温湿度光照一体设备
                                        string cacheKey = $"{model.Id}_{data.Item3}";

                                        ParseQueue = "EnvData";
                                        if (data.Item5 == double.MinValue) // 这是光照数据
                                        {
                                            var lightData = new Dictionary<string, object> { { "Light", data.Item4 }, { "Soil", 0.0 }, { "Rain", 0.0 } };

                                            // 检查是否有缓存的温湿度数据
                                            if (sensorDataCache.TryGetValue(cacheKey, out var cachedData))
                                            {
                                                // 合并数据并写入
                                                fields = new Dictionary<string, object>(cachedData.tempHumData);
                                                fields.Add("Light", data.Item4);

                                                // 写入后移除缓存
                                                sensorDataCache.TryRemove(cacheKey, out _);
                                            }
                                            else
                                            {
                                                // 只收到光照数据，先缓存
                                                sensorDataCache[cacheKey] = (DateTime.Now, null, lightData);
                                            }
                                        }
                                        else // 这是温湿度数据
                                        {
                                            var tempHumData = new Dictionary<string, object>
                                        {
                                            { "Temperature", data.Item4 },
                                            { "Humidity", data.Item5 },
                                            { "Soil", 0.0 },
                                            { "Rain", 0.0 }
                                        };

                                            // 检查是否有缓存的光照数据
                                            if (sensorDataCache.TryGetValue(cacheKey, out var cachedData))
                                            {
                                                // 合并数据并写入
                                                fields = tempHumData;
                                                if (cachedData.lightData != null)
                                                {
                                                    fields.Add("Light", cachedData.lightData["Light"]);
                                                }

                                                // 写入后移除缓存
                                                sensorDataCache.TryRemove(cacheKey, out _);
                                            }
                                            else
                                            {
                                                // 只收到温湿度数据，先缓存
                                                sensorDataCache[cacheKey] = (DateTime.Now, tempHumData, null);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        // 处理普通温湿度传感器
                                        fields = new Dictionary<string, object>
                                    {
                                        { "Temperature", data.Item4 },
                                        { "Humidity", data.Item5 },
                                        { "Light", 0.0 }, // 普通温湿度传感器，光照设为0
                                        { "Soil", 0.0 }, // 普通温湿度传感器，土壤湿度设为0
                                        { "Rain", 0.0 }//雨滴
                                    };
                                        ParseQueue = "EnvData";
                                    }
                                    #endregion
                                }
                                else if(isGas)
                                {
                                    #region 气体

                                    #endregion
                                }
                            }
                        }
                    }


                }
                if (tags.Count > 0&& fields.Count > 0)
                {
                    var postdata = new
                    {
                        tags,
                        fields
                    };

                    _ = producer.Publish(ParseExchange, ParseQueue, JsonConvert.SerializeObject(postdata));
                }
                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff")} ReciveDataParse 异常:{topic}");
            }

            
        }
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();
        
        /// <summary>
        /// 
        /// </summary>
        private void CleanExpiredCache()
        {
            var now = DateTime.Now;
            var expiredKeys = sensorDataCache
                .Where(x => (now - x.Value.timestamp) > TimeSpan.FromMinutes(1))
                .Select(x => x.Key)
                .ToList();

            foreach (var key in expiredKeys)
            {
                sensorDataCache.TryRemove(key, out _);
            }
        }
        private void StartCacheCleanupTask()
        {
            Task.Run(async () =>
            {
                while (!_cts.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), _cts.Token);
                    CleanExpiredCache();
                }
            }, _cts.Token);
        }
        #endregion

    }
}
