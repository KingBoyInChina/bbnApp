using RabbitMQ.Client;
using System.Text;

namespace bbnApp.RabbitMQ
{
    /// <summary>
    /// 生产者
    /// </summary>
    public class RabbitMQProducer
    {
        private IConnection _connection;
        private IChannel _channel;

        public RabbitMQProducer()
        {

        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task StartAsync(RabbitMQModel model)
        {
            var factory = new ConnectionFactory
            {
                HostName = model.HostName,//  
                Port = model.Port,               // 默认 AMQP 端口
                UserName = model.UserName,       // RabbitMQ 用户名（默认 "guest"）
                Password = model.Password,       // RabbitMQ 密码（默认 "guest"）
                VirtualHost = model.VirtualHost         // 虚拟主机（默认 "/"）
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();
            foreach(var Exchange in model.Exchanges)
            {
                // 声明 Exchange（如果不存在则创建）
                await _channel.ExchangeDeclareAsync(
                    exchange: Exchange.ExchangeName,
                    type: Exchange.ExchangeType, // 直连模式
                    durable: Exchange.Durable,             // 持久化
                    autoDelete: Exchange.AutoDelete);
            }
            foreach (var Queue in model.Queues)
            {
                // 声明 Queue（如果不存在则创建）
                await _channel.QueueDeclareAsync(
                    queue: Queue.QueueName,
                    durable: Queue.Durable,             // 持久化
                    exclusive: Queue.Exclusive,
                    autoDelete: Queue.AutoDelete,
                    arguments: Queue.Arguments);
            }
            foreach (var routingkey in model.RoutingKeys)
            {
                // 绑定 Queue 到 Exchange
                await _channel.QueueBindAsync(
                    queue: routingkey.QueueName,
                    exchange: routingkey.ExchangeName,
                    routingKey: routingkey.RoutingKey);
            }
        }

        public async Task Publish(string Exchange="", string QueueName="/", string message="")
        {
            var body = Encoding.UTF8.GetBytes(message);

            // 创建消息属性
            var properties = new BasicProperties();
            properties.Persistent = false;
            properties.ContentType = "text/plain";
            properties.DeliveryMode = DeliveryModes.Transient;

            await _channel.BasicPublishAsync(
                exchange: Exchange,
                routingKey: $"bbn_{QueueName}_routing",
                mandatory: true,
                basicProperties: properties,
                body: body);

            Console.WriteLine($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} Exchange：{Exchange}，QueueName：{QueueName}，routingKey：{$"bbn_{QueueName}_routing"} 发送 {message}");
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
            _channel?.Dispose();
            _connection?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
