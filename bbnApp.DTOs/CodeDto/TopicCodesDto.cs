using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 便于DTO中使用
    /// </summary>
    public class TopicCodesTreeNodeDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string? Id { get; set; } = string.Empty;
        /// <summary>
        /// ID
        /// </summary>
        public string? PId { get; set; } = string.Empty;
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; } = string.Empty;
        /// <summary>
        /// Tag
        /// </summary>
        public string? Tag { get; set; } = string.Empty;
        /// <summary>
        /// 叶子节点
        /// </summary>
        public bool IsLeaf { get; set; } = false;
        /// <summary>
        /// 锁定
        /// </summary>
        public bool IsLock { get; set; } = false;
        /// <summary>
        /// Children
        /// </summary>
        public List<TopicCodesTreeNodeDto>? SubItems { get; set; } = new List<TopicCodesTreeNodeDto>();
    }
    /// <summary>
    /// 订阅代码对象
    /// </summary>
    public class TopicCodesItemDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;
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
    }
    /// <summary>
    /// 订阅代码清单请求对象
    /// </summary>
    public class TopicCodesSearchRequestDto
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string TopicType { get; set; } = string.Empty;
        public string TopicName { get; set; } = string.Empty;
        public string DeviceIds { get; set; } = string.Empty;
    }
    /// <summary>
    /// 订阅代码清单响应对象
    /// </summary>
    public class TopicCodesSearchResponseDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 数据
        /// </summary>
        public List<TopicCodesItemDto> List { get; set; } = new List<TopicCodesItemDto>();
    }
    /// <summary>
    /// 订阅代码信息请求对象
    /// </summary>
    public class TopicCodesInfoRequestDto
    {
        /// <summary>
        /// 设备ID
        /// </summary>
        public string TopicId  { get; set; } = string.Empty;
    }
    /// <summary>
    /// 订阅代码清单响应对象
    /// </summary>
    public class TopicCodesInfoResponseDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 数据
        /// </summary>
        public TopicCodesItemDto Item { get; set; } = new TopicCodesItemDto();
    }
    /// <summary>
    /// 订阅代码树请求对象
    /// </summary>
    public class TopicCodesTreeRequestDto
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string FilterValue { get; set; } = string.Empty;
    }
    /// <summary>
    /// 订阅代码树响应对象
    /// </summary>
    public class TopicCodesTreeResponseDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 数据
        /// </summary>
        public List<TopicCodesTreeNodeDto> TopicCodesItems { get; set; } = new List<TopicCodesTreeNodeDto>();
    }
    /// <summary>
    /// 订阅代码请求对象
    /// </summary>
    public class TopicCodesPostRequestDto
    {
        /// <summary>
        /// 订阅代码对象
        /// </summary>
        public TopicCodesItemDto TopicCodesItem { get; set; } = new TopicCodesItemDto();
    }
    /// <summary>
    /// 订阅代码响应对象
    /// </summary>
    public class TopicCodesPostResponseDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 订阅代码对象
        /// </summary>
        public TopicCodesItemDto Item { get; set; } = new TopicCodesItemDto();
    }
    /// <summary>
    /// 订阅代码状态请求对象
    /// </summary>
    public class TopicCodesStateRequestDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string TopicId  { get; set; } = string.Empty;
        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; } = string.Empty;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
    /// <summary>
    /// 订阅代码状态响应对象
    /// </summary>
    public class TopicCodesStateResponseDto
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 订阅代码对象
        /// </summary>
        public TopicCodesItemDto Item { get; set; } = new TopicCodesItemDto();
    }
}
