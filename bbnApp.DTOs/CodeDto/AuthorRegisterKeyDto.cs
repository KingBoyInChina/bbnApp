
namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 注册密钥对象
    /// </summary>
    public class AuthorRegisterKeyItemDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public Int32 IdxNum { get; set; } = 1;

        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 身份ID
        /// </summary>
        public string AuthorId { get; set; } = string.Empty;

        /// <summary>
        /// 选择使用的应用，后期可能会用到
        /// </summary>
        public string? SelectedAppId { get; set; } = string.Empty;

        /// <summary>
        /// 用户注册时填写应用用途名称
        /// </summary>
        public string SetAppName { get; set; } = string.Empty;

        /// <summary>
        /// 用户注册时填写应用用途代码
        /// </summary>
        public string SetAppCode { get; set; } = string.Empty;

        /// <summary>
        /// 用户注册时填写应用用途说明
        /// </summary>
        public string SetAppDescription { get; set; } = string.Empty;

        /// <summary>
        /// 生成的APPID
        /// </summary>
        public string AppId { get; set; } = string.Empty;

        /// <summary>
        /// 生成的密钥
        /// </summary>
        public string SecriteKey { get; set; } = string.Empty;

        /// <summary>
        /// 操作员ID
        /// </summary>
        public string? OperatorId { get; set; } = string.Empty;

        /// <summary>
        /// 机构ID
        /// </summary>
        public string? CompanyId { get; set; } = string.Empty;

        /// <summary>
        /// 停用
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
    /// 注册信息关联客户端对象
    /// </summary>
    public class AuthorReginsterKeyClientDto
    {
        /// <summary>
        /// 客户端ID
        /// </summary>
        public string ClientId { get; set; } = string.Empty;
        /// <summary>
        /// 客户端名称
        /// </summary>
        public string ClientName { get; set; } = string.Empty;
        /// <summary>
        /// 设置应用名称
        /// </summary>
        public string SetAppName { get; set; } = string.Empty;
        /// <summary>
        /// 注册的身份ID
        /// </summary>
        public string AuthorId { get; set; } = string.Empty;
        /// <summary>
        /// ID
        /// </summary>
        public string AppId { get; set; } = string.Empty;
        /// <summary>
        /// 密钥
        /// </summary>
        public string SecriteKey { get; set; } = string.Empty;
        /// <summary>
        /// 用户Id
        /// </summary>
        public string Yhid { get; set; } = string.Empty;
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 注册人ID
        /// </summary>
        public string OperatorId { get; set; } = string.Empty;
    }

    /// <summary>
    /// 注册机构信息
    /// </summary>
    public class CompanyAuthorRegistrKeyItemDto
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 机构名称
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;
        /// <summary>
        /// 注册密钥数量
        /// </summary>
        public int RegisterKeyCount { get; set; } = 0;
        /// <summary>
        /// 密钥清单
        /// </summary>
        public List<AuthorRegisterKeyItemDto>? RegisterKeys { get; set; } = new List<AuthorRegisterKeyItemDto>();

    }
    /// <summary>
    /// 注册密钥查询请求对象-公司机构
    /// </summary>
    public class CompanyAuthorRegistrKeySearchRequestDto
    {
        public string? AreaId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 注册密钥查询响应对象-公司机构
    /// </summary>
    public class CompanyAuthorRegistrKeySearchResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 对象
        /// </summary>
        public List<CompanyAuthorRegistrKeyItemDto>? Items { get; set; } = new List<CompanyAuthorRegistrKeyItemDto>();
    }

    /// <summary>
    /// 注册密钥查询请求对象
    /// </summary>
    public class AuthorRegisterKeySearchRequestDto
    {
        /// <summary>
        /// 注册时设置的应用名称
        /// </summary>
        public string? SetAppName { get; set; } = string.Empty;
        /// <summary>
        /// 注册时设置的应用代码
        /// </summary>
        public string? SetAppCode { get; set; } = string.Empty;
        /// <summary>
        /// 申请机构
        /// </summary>
        public string? CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 生成的应用ID
        /// </summary>
        public string? AppId { get; set; } = string.Empty;

        /// <summary>
        /// 单页条数
        /// </summary>
        public int PageSize { get; set; } = 1;
        /// <summary>
        /// 分页序号
        /// </summary>
        public int PageIndex { get; set; } = 15;
    }
    /// <summary>
    /// t: 注册密钥查询响应对象
    /// </summary>
    public class AuthorRegisterKeySearchResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 总数
        /// </summary>
        public int Total { get; set; } = 0;
        /// <summary>
        /// 对象
        /// </summary>
        public List<AuthorRegisterKeyItemDto>? Items { get; set; } = new List<AuthorRegisterKeyItemDto>();
    }
    /// <summary>
    /// 注册密钥清单请求对象-不分页
    /// </summary>
    public class AuthorRegisterKeyListRequestDto
    {
        /// <summary>
        /// 注册时设置的应用名称
        /// </summary>
        public string? SetAppName { get; set; } = string.Empty;
        /// <summary>
        /// 注册时设置的应用代码
        /// </summary>
        public string? SetAppCode { get; set; } = string.Empty;
        /// <summary>
        /// 申请机构
        /// </summary>
        public string? CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 生成的应用ID
        /// </summary>
        public string? AppId { get; set; } = string.Empty;
        /// <summary>
        /// AuthorId
        /// </summary>
        public string? AuthorId { get; set; } = string.Empty;
    }
    /// <summary>
    /// t: 注册密钥清单响应对象-不分页
    /// </summary>
    public class AuthorRegisterKeyListResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 对象
        /// </summary>
        public List<AuthorRegisterKeyItemDto>? Items { get; set; } = new List<AuthorRegisterKeyItemDto>();
    }
    /// <summary>
    /// 注册密钥添加请求对象
    /// </summary>
    public class AuthorRegisterKeyAddRequestDto
    {
        public AuthorRegisterKeyItemDto? Item { get; set; } = new AuthorRegisterKeyItemDto();
    }
    /// <summary>
    /// 注册密钥添加响应对象
    /// </summary>
    public class AuthorRegisterKeyAddResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 对象
        /// </summary>
        public AuthorRegisterKeyItemDto Item { get; set; } = new AuthorRegisterKeyItemDto();
    }
    /// <summary>
    /// 注册密钥添加请求对象
    /// </summary>
    public class AuthorRegisterKeyStateRequestDto
    {
        /// <summary>
        /// 操作方式
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 注册的身份ID
        /// </summary>
        public string AuthorId { get; set; } = string.Empty;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
    /// <summary>
    /// 注册密钥添加响应对象
    /// </summary>
    public class AuthorRegisterKeyStateResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 对象
        /// </summary>
        public AuthorRegisterKeyItemDto Item { get; set; } = new AuthorRegisterKeyItemDto();
    }
    /// <summary>
    /// 密钥关联设备清单-请求对象
    /// </summary>
    public class AuthorReginsterKeyClientListRequestDto
    {
        /// <summary>
        /// 冗余参数
        /// </summary>
        public string Type { get; set; } = string.Empty;
    }
    /// <summary>
    /// 密钥关联设备清单-响应
    /// </summary>
    public class AuthorReginsterKeyClientListResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 禽蛋
        /// </summary>
        public List<AuthorReginsterKeyClientDto> Items { get; set; } = new List<AuthorReginsterKeyClientDto>();
    }
    /// <summary>
    /// 密钥对象获取-请求对象(一般用于平台,因为设备端在初始化的时候固定下来了的)
    /// </summary>
    public class AuthorReginsterKeyInfoRequestDto
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;
        public string OperatorId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 密钥关联设备清单-响应
    /// </summary>
    public class AuthorReginsterKeyInfoResponseDto
    {
        /// <summary>
        /// 状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 禽蛋
        /// </summary>
        public List<AuthorReginsterKeyClientDto> Items { get; set; } = new List<AuthorReginsterKeyClientDto>();
    }
}
