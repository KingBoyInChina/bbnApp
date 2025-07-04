using Microsoft.Extensions.Logging;
using MQTTnet;
using System.Text;

namespace bbnApp.MQTT.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class MqttClientService
    {
        private readonly MqttClientFactory mqttFactory = new MqttClientFactory();
        private readonly ILogger _logger;
        private readonly IMqttClient _mqttClient;
        private readonly Dictionary<string, List<Action<string,string>>> _messageHandlers = new();

        private MqttClientOptions mqttClientOptions;
        private bool _isRunning = false;
        public MqttClientService() {
            _mqttClient = mqttFactory.CreateMqttClient();
            _mqttClient.ApplicationMessageReceivedAsync += OnMessageReceived;
        }
        /// <summary>
        /// MQTT连接
        /// </summary>
        /// <param name="TcpIp"></param>
        /// <param name="port"></param>
        /// <param name="ClientId"></param>
        /// <param name="AppId"></param>
        /// <param name="SecretKey"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task Connect(string TcpIp, int port, string ClientId, string AppId, string SecretKey, List<string>? defaultTopics = null)
        {
            try
            {
                mqttClientOptions = new MqttClientOptionsBuilder()
                            .WithTcpServer(TcpIp, port)
                            .WithClientId(ClientId)
                            .WithCredentials(AppId, SecretKey).Build();
                await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                if (defaultTopics != null)
                {
                    var mqttSubscribeOptions = mqttFactory.CreateSubscribeOptionsBuilder();
                    foreach (var topic in defaultTopics)
                    {
                        mqttSubscribeOptions.WithTopicFilter(topic);
                    }
                    var subscribeOptions = mqttSubscribeOptions.Build();
                    await _mqttClient.SubscribeAsync(subscribeOptions, CancellationToken.None);
                }
                _isRunning = true;
                _mqttClient.DisconnectedAsync += async e =>
                {
                    if (_isRunning)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(20)); // 等待 20s
                        await TryConnectAsync(); // 自动重连
                    }
                };
            }
            catch(Exception ex)
            {
                _logger.LogError($"MQTT 连接异常：{ex.Message.ToString()}");
            }
        }
        /// <summary>
        /// 尝试重连
        /// </summary>
        /// <returns></returns>
        private async Task TryConnectAsync()
        {
            if (_mqttClient != null)
            {
                try
                {
                    if (!_mqttClient.IsConnected && _isRunning)
                    {
                        await _mqttClient.ConnectAsync(mqttClientOptions);
                    }
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        await Task.Delay(TimeSpan.FromSeconds(20)); // 等待 20s 后重试
                        await TryConnectAsync();
                    }
                }
            }
        }
        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        public async Task Disconnect()
        {
            if (_mqttClient != null)
            {
                if (_mqttClient.IsConnected)
                {
                    await _mqttClient.DisconnectAsync();
                }
            }
        }
        /// <summary>
        /// 添加主题订阅
        /// </summary>
        /// <param name="topic"></param>
        /// <returns></returns>
        public async Task Subscribe(string topic)
        {
            if (_mqttClient == null)
            {
                return;
            }
            if (_mqttClient.IsConnected)
            {
                await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).Build());
            }
        }
        /// <summary>
        /// 注册handler
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="handler"></param>
        public void RegisterHandler(string topic, Action<string,string> handler)
        {
            if (_messageHandlers != null)
            {
                if (!_messageHandlers.ContainsKey(topic))
                {
                    _messageHandlers[topic] = new List<Action<string, string>>();
                }
                _messageHandlers[topic].Add(handler);
            }
        }
        /// <summary>
        /// 移除handler
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="handler"></param>
        public void UnregisterHandler(string topic, Action<string,string> handler)
        {
            if (_messageHandlers != null)
            {
                if (_messageHandlers.ContainsKey(topic))
                {
                    _messageHandlers[topic].Remove(handler);
                }
            }
        }
        /// <summary>
        /// 消息接收
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private Task OnMessageReceived(MqttApplicationMessageReceivedEventArgs args)
        {
            var topic = args.ApplicationMessage.Topic;
            var payload = Encoding.UTF8.GetString(args.ApplicationMessage.Payload);

            if (_messageHandlers.ContainsKey(topic))
            {
                //消息分发
                foreach (var handler in _messageHandlers[topic])
                {
                    handler(topic,payload);
                }
            }

            return Task.CompletedTask;
        }
        /// <summary>
        /// 清除Handler
        /// </summary>
        public void ClearHandlers()
        {
            if (_messageHandlers != null)
            {
                _messageHandlers.Clear();
            }
        }
        /// <summary>
        /// 消息发布
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishAsync(string topic,string message)
        {
            var applicationMessage = new MqttApplicationMessageBuilder()
                .WithTopic(topic)
                .WithPayload(message)
                .Build();

            await _mqttClient.PublishAsync(applicationMessage, CancellationToken.None);
        }
    }
}
