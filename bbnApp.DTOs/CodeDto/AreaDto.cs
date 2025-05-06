

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 应用请求对象
    /// </summary>
    public class AreaRequestDto
    {
        /// <summary>
        /// 区划代码
        /// </summary>
        public string? AreaCode { get; set; }
        /// <summary>
        /// 地区名称
        /// </summary>
        public string? AreaName { get; set; }
        /// <summary>
        /// 行政级别
        /// </summary>
        public string? AreaLeve { get; set; }
    }
    /// <summary>
    /// 返回信息
    /// </summary>
    public class AreaResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public List<AreaItemDto>? Items { get; set; }
    }
    /// <summary>
    /// 地区信息
    /// </summary>
    public class AreaItemDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int? IdxNum { get; set; }
        /// <summary>
        /// 区划代码
        /// </summary>
        public string? AreaId { get; set; }
        /// <summary>
        /// 上级区划代码
        /// </summary>
        public string? AreaPId { get; set; }
        /// <summary>
        /// 区划名称
        /// </summary>
        public string? AreaName { get; set; }
        /// <summary>
        /// 区划全称呼
        /// </summary>
        public string? AreaFullName { get; set; }
        /// <summary>
        /// 行政级别
        /// </summary>
        public string? AreaLeve { get; set; }
        /// <summary>
        /// 行政级别名称
        /// </summary>
        public string? AreaLeveName { get; set; }
        /// <summary>
        /// 经纬度
        /// </summary>
        public string? AreaPoint { get; set; }
        /// <summary>
        /// 锁定状态
        /// </summary>
        public bool? IsLock { get; set; }
        /// <summary>
        /// 锁定时间
        /// </summary>
        public string? LockTime { get; set; }
        /// <summary>
        /// 锁定原因
        /// </summary>
        public string? LockReason { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? ReMarks { get; set; }
    }

    /// <summary>
    /// 地区字典Tree请求对象
    /// </summary>
    public class AreaTreeNodeRequestDto
    {
        /// <summary>
        /// 区划代码
        /// </summary>
        public string? AreaCode { get; set; }
        /// <summary>
        /// 区划级别
        /// </summary>
        public int? AreaLeve { get; set; }
    }
    /// <summary>
    /// 地区字典Tee返回信息
    /// </summary>
    public class AreaTreeNodeResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public List<AreaTreeNodeDto>? Items { get; set; }
    }
    /// <summary>
    /// 地区字典Tree节点信息
    /// </summary>
    public class AreaTreeNodeDto
    {
        /// <summary>
        /// 区划代码
        /// </summary>
        public string? AreaId { get; set; }
        /// <summary>
        /// 区划名称
        /// </summary>
        public string? AreaName { get; set; }
        /// <summary>
        /// 区划全称
        /// </summary>
        public string? AreaFullName { get; set; }
        /// <summary>
        /// 行政级别
        /// </summary>
        public string? AreaLeve { get; set; }
        /// <summary>
        /// 行政级别名称
        /// </summary>
        public string? AreaLeveName { get; set; }
        /// <summary>
        /// 经纬度
        /// </summary>
        public string? AreaPoint { get; set; }
        /// <summary>
        /// 锁定状态
        /// </summary>
        public bool? IsLock { get; set; }
        /// <summary>
        /// 叶子节点
        /// </summary>
        public bool? IsLeaf { get; set; }
        /// <summary>
        /// 子节点
        /// </summary>
        public IEnumerable<AreaTreeNodeDto>? Children { get; set; }
    }

    /// <summary>
    /// 地区字典List请求对象
    /// </summary>
    public class AreaListNodeRequestDto
    {
        /// <summary>
        /// 区划代码
        /// </summary>
        public string? AreaCode { get; set; }
        /// <summary>
        /// 区划级别
        /// </summary>
        public int? AreaLeve { get; set; }
    }
    /// <summary>
    /// 地区字典Tree返回信息
    /// </summary>
    public class AreaListNodeResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public List<AreaListNodeDto>? Items { get; set; }
    }
    /// <summary>
    /// 地区字典Tree节点信息
    /// </summary>
    public class AreaListNodeDto
    {
        /// <summary>
        /// 区划代码
        /// </summary>
        public string? AreaId { get; set; }
        /// <summary>
        /// 上级区划代码
        /// </summary>
        public string? AreaPId { get; set; }
        /// <summary>
        /// 区划名称
        /// </summary>
        public string? AreaName { get; set; }
        /// <summary>
        /// 区划全称
        /// </summary>
        public string? AreaFullName { get; set; }
        /// <summary>
        /// 行政级别
        /// </summary>
        public string? AreaLeve { get; set; }
        /// <summary>
        /// 行政级别名称
        /// </summary>
        public string? AreaLeveName { get; set; }
        /// <summary>
        /// 经纬度
        /// </summary>
        public string? AreaPoint { get; set; }
        /// <summary>
        /// 锁定状态
        /// </summary>
        public bool? IsLock { get; set; }
    }

    /// <summary>
    /// 地区字典List请求对象
    /// </summary>
    public class AreaPostRequestDto
    {
        /// <summary>
        /// 区划提交代码
        /// </summary>
        public AreaPostDataDto? AreaData { get; set; }
    }
    /// <summary>
    /// 地区字典Tree返回信息
    /// </summary>
    public class AreaPostResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        public AreaPostDataDto? AreaData { get; set; }
    }
    /// <summary>
    /// 行政区划提交数据
    /// </summary>
    public class AreaPostDataDto
    {
        /// <summary>
        /// 区划代码
        /// </summary>
        public string? AreaId { get; set; }
        /// <summary>
        /// 上级区划代码
        /// </summary>
        public string? AreaPId { get; set; }
        /// <summary>
        /// 区划名称
        /// </summary>
        public string? AreaName { get; set; }
        /// <summary>
        /// 区划全称
        /// </summary>
        public string? AreaFullName { get; set; }
        /// <summary>
        /// 行政级别
        /// </summary>
        public string? AreaLeve { get; set; }
        /// <summary>
        /// 行政级别名称
        /// </summary>
        public string? AreaLeveName { get; set; }
        /// <summary>
        /// 经纬度
        /// </summary>
        public string? AreaPoint { get; set; }
        /// <summary>
        /// 锁定状态
        /// </summary>
        public bool? IsLock { get; set; }
        /// <summary>
        /// 锁定时间
        /// </summary>
        public string? LockTime { get; set; }
        /// <summary>
        /// 锁定原因
        /// </summary>
        public string? LockReason { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? ReMarks { get; set; }
    }

    /// <summary>
    /// 地区删除请求
    /// </summary>
    public class AreaDeleteRequestDto
    {
        /// <summary>
        /// 地区代码
        /// </summary>
        public string? AreaId { get; set; }
    }
    /// <summary>
    /// 地区删除返回信息
    /// </summary>
    public class AreaDeleteResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
    }


    /// <summary>
    /// 地区删除请求
    /// </summary>
    public class AreaLockRequestDto
    {
        /// <summary>
        /// 地区代码
        /// </summary>
        public string? AreaId { get; set; }
        /// <summary>
        /// 地区停用原因
        /// </summary>
        public string? LockReason { get; set; }
    }
    /// <summary>
    /// 地区删除返回信息
    /// </summary>
    public class AreaLockResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; }
    }
}
