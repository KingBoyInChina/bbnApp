namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 系统配置类型
    /// </summary>
    public class AppSettingDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Yhid { get; set; }
        /// <summary>
        /// 取值范围
        /// </summary>
        public string ValueRange { get; set; }
        /// <summary>
        /// 配置参数类型
        /// </summary>
        public string SettingType { get; set; }
        /// <summary>
        /// 配置名称
        /// </summary>
        public string SettingName { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int SettingIndex { get; set; }
        /// <summary>
        /// 配置id
        /// </summary>
        public string SettingId { get; set; }
        /// <summary>
        /// 配置说明
        /// </summary>
        public string SettingDesc { get; set; }
        /// <summary>
        /// 配置代码
        /// </summary>
        public string SettingCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string? ReMarks { get; set; }
        /// <summary>
        /// 当前值
        /// </summary>
        public string NowValue { get; set; }
        /// <summary>
        /// 锁定时间
        /// </summary>
        public string? LockTime { get; set; }
        /// <summary>
        /// 锁定原因
        /// </summary>
        public string? LockReason { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        public byte IsLock { get; set; }
        /// <summary>
        /// 固定值
        /// </summary>
        public byte IsFiexed { get; set; }
        /// <summary>
        /// 删除状态
        /// </summary>
        public byte Isdelete { get; set; }
        /// <summary>
        /// 默认值
        /// </summary>
        public string DefaultValue { get; set; }
    }
    #region 系统配置查询
    /// <summary>
    /// 系统配置查询请求
    /// </summary>
    public class AppSettingSearchRequestDto
    {
        /// <summary>
        /// 配置参数名称
        /// </summary>
        public string? SettingName { get; set; }
        /// <summary>
        /// 配置代码
        /// </summary>
        public string? SettingCode { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        public byte IsFiexed { get; set; }
        /// <summary>
        /// 固定
        /// </summary>
        public byte IsLock { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string? SettingDesc { get; set; }
        /// <summary>
        /// 单页条数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 分页序号
        /// </summary>
        public int PageIndex { get; set; }
    }
    /// <summary>
    /// 系统配置查询响应
    /// </summary>
    public class AppSettingSearchResponseDto
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
        /// 总数
        /// </summary>
        public int Total { get; set; }
        /// <summary>
        /// 配置对象
        /// </summary>
        public List<AppSettingDto>? Items { get; set; }
    }
    #endregion
    #region 系统配置提交
    /// <summary>
    /// 系统配置提交请求
    /// </summary>
    public class AppSettingPostRequestDto
    { 
        public AppSettingDto Item { get; set; }
    }
    /// <summary>
    /// 系统配置提交响应
    /// </summary>
    public class AppSettingPostResponseDto 
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
        /// 配置对象
        /// </summary>
        public AppSettingDto? Item { get; set; }
    }
    #endregion
    #region 系统配置状态变更
    /// <summary>
    /// 系统配置状态变更请求
    /// </summary>
    public class AppSettingStateRequestDto
    {
        /// <summary>
        /// 动作类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 配置对象
        /// </summary>
        public AppSettingDto Item { get; set; }
    }
    /// <summary>
    /// 系统配置状态变更响应
    /// </summary>
    public class AppSettingStateResponseDto
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
        /// 配置对象
        /// </summary>
        public AppSettingDto? Item { get; set; }
    }
    #endregion
}
