using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Common.Models
{
    /// <summary>
    /// MQTT对象
    /// </summary>
    public class MqttClientModel
    {
        /// <summary>
        /// ClientId,对应的不同类型的连接对象：OperatorId/UserId/DeviceId
        /// </summary>
        public string ClientId { get; set; } = string.Empty;
        /// <summary>
        /// 连接对象类型,Operator/User/Device
        /// </summary>
        public string ClientType { get; set; } = string.Empty;
        /// <summary>
        /// 连接对象名称
        /// </summary>
        public string ClientName { get; set; }=string.Empty;
        /// <summary>
        /// 是否是平台运维人员
        /// </summary>
        public bool IsAdmin { get; set; } = false;
        /// <summary>
        /// 客户名称,Operator对应company,User对应用户名称,Device对应用户名称
        /// </summary>
        public string CustomeName { get; set; } = string.Empty;
        /// <summary>
        /// 客户等级
        /// </summary>
        public string CustomeLeveName { get; set; } = string.Empty;
        /// <summary>
        /// 编号
        /// </summary>
        public string Number { get; set; } = string.Empty;
        /// <summary>
        /// 是否在线
        /// </summary>
        public bool Online { get; set; } = false;
    }
}
