using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.RabbitMQ
{
    /// <summary>
    /// RabbitMQ配置模型
    /// </summary>
    public class RabbitMQModel
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; } = 5672;
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 虚拟主机
        /// </summary>
        public string VirtualHost { get; set; } = "/";
        /// <summary>
        /// 定义交换机
        /// </summary>
        public List<ExchangeModel> Exchanges { get; set; } = new List<ExchangeModel>();
        /// <summary>
        /// 队列名称
        /// </summary>
        public List<QueueModel> Queues { get; set; } = new List<QueueModel>();
        /// <summary>
        /// 路由键值
        /// </summary>
        public List<ExchangeAndQueueModel> RoutingKeys { get; set; } = new List<ExchangeAndQueueModel>();

    }
    /// <summary>
    /// 
    /// </summary>
    public class ExchangeModel
    {
        /// <summary>
        /// 交换机名称
        /// </summary>
        public string ExchangeName { get; set; }
        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get; set; } = false;
        /// <summary>
        /// 自动删除
        /// </summary>
        public bool AutoDelete { get; set; } = true;
        /// <summary>
        /// 连接模式
        /// </summary>
        public string ExchangeType { get; set; } = "direct"; // 默认直连模式
    }
    /// <summary>
    /// 队列模型    
    /// </summary>
    public class QueueModel { 
        public string QueueName { get; set; }
        public bool Durable { get; set; } = false;
        public bool Exclusive { get; set; } = false;
        public bool AutoDelete { get; set; } = false;
        public Dictionary<string, object?> Arguments { get; set; } = new Dictionary<string, object?>();

    }
    /// <summary>
    /// 路由关系
    /// </summary>
    public class ExchangeAndQueueModel
    {
        public string ExchangeName { get; set; }
        public string QueueName { get; set; }
        public string RoutingKey { get; set; }

    }
}
