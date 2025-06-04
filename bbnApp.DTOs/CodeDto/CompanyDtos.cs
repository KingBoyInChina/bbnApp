
namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 树形对象
    /// </summary>
    public class CompanyTreeItemDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string? PId { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public string? Tag { get; set; }
        /// <summary>
        /// 叶子节点
        /// </summary>
        public bool? IsLeaf { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        public bool? IsLock { get; set; }
        /// <summary>
        /// Children
        /// </summary>
        public List<CompanyTreeItemDto>? SubItems { get; set; }
    }
    /// <summary>
    /// 机构对象
    /// </summary>
    public class CompanyItemDto
    {
        /// <summary>
        /// ID
        /// </summary>
        public string? Yhid { get; set; }
        /// <summary>
        /// ID
        /// </summary>
        public string? Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Tag
        /// </summary>
        public string? Tag { get; set; }
    }
    /// <summary>
    /// 机构信息数据传输对象
    /// </summary>
    public class CompanyInfoDto
    {
        /// <summary>
        /// 
        /// </summary>
        public int IdxNum { get; set; } = 1;
        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;

        /// <summary>
        /// 上级机构ID
        /// </summary>
        public string PCompanyId { get; set; } = string.Empty;

        /// <summary>
        /// 机构类型
        /// </summary>
        public string CompanyType { get; set; } = string.Empty;

        /// <summary>
        /// 机构名称
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;

        /// <summary>
        /// 机构代码
        /// </summary>
        public string CompanyCode { get; set; } = string.Empty;

        /// <summary>
        /// 组织机构代码
        /// </summary>
        public string OrganizationCode { get; set; } = string.Empty;

        /// <summary>
        /// 机构级别
        /// </summary>
        public byte CompanyLeve { get; set; } = byte.MinValue;

        /// <summary>
        /// 机构级别名称
        /// </summary>
        public string CompanyLeveName { get; set; } = string.Empty;

        /// <summary>
        /// 所在地行政区划代码
        /// </summary>
        public string AreaCode { get; set; } = string.Empty;

        /// <summary>
        /// 所在地行政区划名称
        /// </summary>
        public string AreaName { get; set; } = string.Empty;

        /// <summary>
        /// 所在地详细信息
        /// </summary>
        public string AreaNameExt { get; set; } = string.Empty;

        /// <summary>
        /// 经纬度坐标
        /// </summary>
        public string? Location { get; set; } = string.Empty;

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; } = string.Empty;

        /// <summary>
        /// 停用状态
        /// </summary>
        public byte IsLock { get; set; } = 0;

        /// <summary>
        /// 停用时间
        /// </summary>
        public string? LockTime { get; set; } = string.Empty;

        /// <summary>
        /// 停用原因
        /// </summary>
        public string? LockReason { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;
    }
    /// <summary>
    /// 请求对象
    /// </summary>
    public class CompanyRequestDto
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string? Version { get; set; } = string.Empty;
        /// <summary>
        /// 默认机构id
        /// </summary>
        public string? CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 用户组ID
        /// </summary>
        public string? Yhid { get; set; } = string.Empty;
    }
    /// <summary>
    /// 机构返回信息
    /// </summary>
    public class CompanyResponseDto
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
        /// 机构清单
        /// </summary>
        public List<CompanyItemDto> CompanyItems { get; set; }
    }
    /// <summary>
    /// 机构树请求对象
    /// </summary>
    public class CompanyTreeRequestDto
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string? CompanyName { get; set; } = string.Empty;
        /// <summary>
        /// 所在地区
        /// </summary>
        public string? AreaCode { get; set; } = string.Empty;
        /// <summary>
        /// 公司代码
        /// </summary>
        public string? CompanyCode { get; set; } = string.Empty;
    }
    /// <summary>
    /// 机构树返回对象
    /// </summary>
    public class CompanyTreeResponseDto {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        public List<CompanyTreeItemDto> Items { get; set; }
    }
    /// <summary>
    /// 机构清单请求对象
    /// </summary>
    public class CompanySearchRequestDto
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;
        /// <summary>
        /// 所在地区
        /// </summary>
        public string AreaCode { get; set; } = string.Empty;
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; } = string.Empty;
    }
    /// <summary>
    /// 机构清单返回对象
    /// </summary>
    public class CompanySearchResponseDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 机构清单
        /// </summary>
        public List<CompanyInfoDto> Items { get; set; }
    }
    /// <summary>
    /// 机构信息请求
    /// </summary>
    public class CompanyInfoRequestDto {
        /// <summary>
        /// 机构id
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 机构信息返回对象
    /// </summary>
    public class CompanyInfoResponseDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public CompanyInfoDto Item { get; set; } = new CompanyInfoDto();
    }
    /// <summary>
    /// 机构提交请求对象
    /// </summary>
    public class CompanySaveRequestDto { 
        public CompanyInfoDto Item { get; set; } = new CompanyInfoDto();
    }
    /// <summary>
    /// 机构提交返回对象
    /// </summary>
    public class CompanySaveResponseDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public CompanyInfoDto Item { get; set; } = new CompanyInfoDto();
    }
    /// <summary>
    /// 机构状态请求对象
    /// </summary>
    public class CompanyStateRequestDto
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
    /// <summary>
    /// 机构状态返回对象
    /// </summary>
    public class CompanyStateResponseDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public CompanyInfoDto Item { get; set; } = new CompanyInfoDto();
    }
}
