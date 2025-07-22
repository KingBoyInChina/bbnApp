using bbnApp.Common.Models;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.RabbitMQ
{
    public class RabbitMQConsumer : IDisposable
    {
        private IConnection _connection;
        private IChannel _channel;
        private RabbitMQModel rabbitMQModel;

        public RabbitMQConsumer(RabbitMQModel model)
        {
            rabbitMQModel = model;
        }

        public async Task Init()
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQModel.HostName,
                Port = rabbitMQModel.Port,               // 默认 AMQP 端口
                UserName = rabbitMQModel.UserName,       // RabbitMQ 用户名（默认 "guest"）
                Password = rabbitMQModel.Password,      // RabbitMQ 密码（默认 "guest"）
                VirtualHost = rabbitMQModel.VirtualHost,       // 虚拟主机（默认 "/"）
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            foreach (var Exchange in rabbitMQModel.Exchanges)
            {
                // 声明 Exchange（如果不存在则创建）
                await _channel.ExchangeDeclareAsync(
                    exchange: Exchange.ExchangeName,
                    type: Exchange.ExchangeType, // 直连模式
                    durable: Exchange.Durable,             // 持久化
                    autoDelete: Exchange.AutoDelete);
            }
            foreach (var Queue in rabbitMQModel.Queues)
            {
                // 声明 Queue（如果不存在则创建）
                await _channel.QueueDeclareAsync(
                    queue: Queue.QueueName,
                    durable: Queue.Durable,             // 持久化
                    exclusive: Queue.Exclusive,
                    autoDelete: Queue.AutoDelete,
                    arguments: Queue.Arguments);
            }
            foreach (var routingkey in rabbitMQModel.RoutingKeys)
            {
                // 绑定 Queue 到 Exchange
                await _channel.QueueBindAsync(
                    queue: routingkey.QueueName,
                    exchange: routingkey.ExchangeName,
                    routingKey: routingkey.RoutingKey);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="reciveDataParse"></param>
        /// <returns></returns>
        public async Task StartConsuming(Action<InfluxDbReciveData> reciveDataParse=null)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);


            consumer.ReceivedAsync += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var tag = ea.ConsumerTag;
                var routingKey = ea.RoutingKey;
                var exchange = ea.Exchange;
                var _data = new InfluxDbReciveData { 
                    Message=message,
                    Exchange=exchange,
                    QueueName=tag,
                    RoutingKey=routingKey
                };
                Debug.WriteLine($"{DateTime.Now.ToString("HH:mm:ss:fff")} Received message: {message} from Exchange: {exchange}, RoutingKey: {routingKey}, QueueName: {tag}");
                if (reciveDataParse != null)
                {
                    reciveDataParse(_data);
                }
                return Task.CompletedTask;
            };
            foreach (var routingkey in rabbitMQModel.RoutingKeys)
            {
                await _channel.BasicConsumeAsync(
                               queue: routingkey.QueueName,
                               autoAck: true,
                               consumerTag: routingkey.QueueName,
                               consumer: consumer);
            }
               

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
