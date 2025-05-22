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
    public class DeviceCodeTreeNodeDto
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
        public List<DeviceCodeTreeNodeDto>? SubItems { get; set; } = new List<DeviceCodeTreeNodeDto>();
    }
    /// <summary>
    /// 设备代码对象
    /// </summary>
    public class DeviceCodeItemDto
    {
        public int IdxNum { get; set; } = 0;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// 设备代码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; } = string.Empty;

        /// <summary>
        /// 设备分类
        /// </summary>
        public string DeviceType { get; set; } = string.Empty;

        /// <summary>
        /// 规格
        /// </summary>
        public string? DeviceSpecifications { get; set; } = string.Empty;

        /// <summary>
        /// 型号
        /// </summary>
        public string? DeviceModel { get; set; } = string.Empty;

        /// <summary>
        /// 条码号
        /// </summary>
        public string? DeviceBarCode { get; set; } = string.Empty;

        /// <summary>
        /// 用途
        /// </summary>
        public string? Usage { get; set; } = string.Empty;

        /// <summary>
        /// 存储环境
        /// </summary>
        public string? StorageEnvironment { get; set; } = string.Empty;

        /// <summary>
        /// 使用环境
        /// </summary>
        public string? UsageEnvironment { get; set; } = string.Empty;

        /// <summary>
        /// 使用寿命
        /// </summary>
        public int ServiceLife { get; set; } = 0;

        /// <summary>
        /// 计量单位
        /// </summary>
        public string LifeUnit { get; set; } = string.Empty;

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
    /// 设备构成对象
    /// </summary>
    public class DeviceStructItemDto
    {
        public int IdxNum { get; set; } = 0;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 构成ID
        /// </summary>
        public string StructId { get; set; } = string.Empty;

        /// <summary>
        /// 设备ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;

        /// <summary>
        /// 物资ID
        /// </summary>
        public string MaterialId { get; set; } = string.Empty;

        /// <summary>
        /// 物资名称
        /// </summary>
        public string MaterialName { get; set; } = string.Empty;

        /// <summary>
        /// 使用数量
        /// </summary>
        public int UtilizeQuantity { get; set; } = 0;

        /// <summary>
        /// 计量单位
        /// </summary>
        public string QuantityUnit { get; set; } = string.Empty;

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
    /// 设备代码树请求对象
    /// </summary>
    public class DeviceCodeTreeRequestDto
    {
        /// <summary>
        /// 过滤条件
        /// </summary>
        public string FilterValue { get; set; } = string.Empty;
    }
    /// <summary>
    /// 设备代码树响应对象
    /// </summary>
    public class DeviceCodeTreeResponseDto
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
        public List<DeviceCodeItemDto> deviceCodeItems { get; set; } = new List<DeviceCodeItemDto>();
    }
    /// <summary>
    /// 设备代码请求对象
    /// </summary>
    public class DeviceCodePostRequestDto
    {
        /// <summary>
        /// 设备代码对象
        /// </summary>
        public DeviceCodeItem DeviceCodeItem { get; set; } = new DeviceCodeItem();
        /// <summary>
        /// 设备构成对象
        /// </summary>
        public List<DeviceStructItemDto> DeviceStructItems { get; set; } = new List<DeviceStructItemDto>();
    }
    /// <summary>
    /// 设备代码响应对象
    /// </summary>
    public class DeviceCodePostResponseDto
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
        /// 设备代码对象
        /// </summary>
        public DeviceCodeItem Item { get; set; } = new DeviceCodeItem();
        /// <summary>
        /// 设备构成对象
        /// </summary>
        public List<DeviceStructItemDto> List { get; set; } = new List<DeviceStructItemDto>();
    }
    /// <summary>
    /// 设备代码状态请求对象
    /// </summary>
    public class DeviceCodeStateRequestDto { 
        /// <summary>
        /// ID
        /// </summary>
        public string DeviceId { get; set; } = string.Empty;
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
    /// 设备代码状态响应对象
    /// </summary>
    public class DeviceCodeStateResponseDto
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
        /// 设备代码对象
        /// </summary>
        public DeviceCodeItemDto Item { get; set; } = new DeviceCodeItemDto();
        /// <summary>
        /// 设备构成对象
        /// </summary>
        public List<DeviceStructItemDto> List { get; set; } = new List<DeviceStructItemDto>();
    }
    /// <summary>
    /// 设备构成状态请求对象
    /// </summary>
    public class DeviceStructStateRquestDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string StructId { get; set; } = string.Empty;
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
    /// 设备构成状态响应对象
    /// </summary>
    public class DeviceStructStateResponseDto
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
        /// 设备构成对象
        /// </summary>
        public List<DeviceStructItemDto> List { get; set; } = new List<DeviceStructItemDto>();
    }
}
