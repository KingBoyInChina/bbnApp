using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 订阅代码对象
    /// </summary>
    public class TopicCodesItemDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 订阅代码ID
        /// </summary>
        public string TopicId { get; set; } = string.Empty;

        /// <summary>
        /// 订阅代码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 订阅名称
        /// </summary>
        public string TopicName { get; set; } = string.Empty;

        /// <summary>
        /// 路由地址
        /// </summary>
        public string TopicRoter { get; set; } = string.Empty;

        /// <summary>
        /// 订阅分类,设备/基站等
        /// </summary>
        public string TopicType { get; set; } = string.Empty;

        /// <summary>
        /// 设备分类代码
        /// </summary>
        public string DeviceType { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID,需要指定设备的情况
        /// </summary>
        public string? DeviceIds { get; set; } = string.Empty;

        /// <summary>
        /// 锁定状态
        /// </summary>
        public byte IsLock { get; set; } = 0;

        /// <summary>
        /// 锁定时间
        /// </summary>
        public string? LockTime { get; set; } = string.Empty;

        /// <summary>
        /// 锁定原因
        /// </summary>
        public string? LockReason { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;

        /// <summary>
        /// 删除状态
        /// </summary>
        public byte Isdelete { get; set; } = 0;

        /// <summary>
        /// 末次数据变更时间
        /// </summary>
        public DateTime LastModified { get; set; } = DateTime.Now;
    }

}
