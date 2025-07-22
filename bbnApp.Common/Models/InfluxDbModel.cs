using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Common.Models
{
    public class InfluxDbModel
    {
        public string Url { get; set; } = string.Empty;// InfluxDB 服务器地址
        public string Token { get; set; } = string.Empty;// 访问令牌
        public string Org { get; set; } = string.Empty;// 组织名称
        public List<string> Bucket { get; set; } =new List<string>(); // 存储桶名称
        public int MaxBatchSize { get; set; } = 1000; // 最大批量大小
        public int MaxBatchInterval { get; set; } = 5; // 最大批量间隔（秒）
        public InfluxDbModel() { }
    }
    /// <summary>
    /// 接收到的数据
    /// </summary>
    public class InfluxDbReciveData
    {
        public string Message { get; set; } = string.Empty; // 接收到的消息内容
        public string Exchange { get; set; } = string.Empty; // 消息所属的 Exchange
        public string RoutingKey { get; set; } = string.Empty; // 消息的路由键
        public string QueueName { get; set; } = string.Empty; // 消息所属的队列名称`
        public DateTime Timestamp { get; set; } = DateTime.UtcNow; // 消息接收时间戳
    }
}
