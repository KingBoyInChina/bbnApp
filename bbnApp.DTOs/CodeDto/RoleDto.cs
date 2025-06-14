
namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 角色对象
    /// </summary>
    public class RoleItemDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;
        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// 角色代码
        /// </summary>
        public string RoleCode { get; set; } = string.Empty;

        /// <summary>
        /// 角色级别
        /// </summary>
        public byte RoleLeve { get; set; } = 0;

        /// <summary>
        /// 角色介绍
        /// </summary>
        public string? RoleDescription { get; set; } = string.Empty;

        /// <summary>
        /// 停用状态
        /// </summary>
        public byte IsLock { get; set; } = 0;

        /// <summary>
        /// 停用时间
        /// </summary>
        public string? LockTime { get; set; } = string.Empty; // 使用字符串格式化日期

        /// <summary>
        /// 停用原因
        /// </summary>
        public string? LockReason { get; set; } = string.Empty;

        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;
    }
    /// <summary>
    /// 角色应用对象
    /// </summary>
    public class RoleAppsDto
    {

        /// <summary>
        /// 序号
        /// </summary>
        public byte IdxNum { get; set; } = 0;
        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;

        /// <summary>
        /// 应用ID
        /// </summary>
        public string AppId { get; set; } = string.Empty;

        /// <summary>
        /// 应用名称
        /// </summary>
        public string AppName { get; set; } = string.Empty;

        /// <summary>
        /// 应用代码
        /// </summary>
        public string AppCode { get; set; } = string.Empty;

        /// <summary>
        /// 选中状态
        /// </summary>
        public bool IsChecked { get; set; } = false;

        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 应用关联的操作对象
        /// </summary>
        public List<RolePermissionItemDto> Items { get; set; } = new List<RolePermissionItemDto>();
    }
    /// <summary>
    /// 角色操作权限对象
    /// </summary>
    public class RolePermissionItemDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;
        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;

        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;
        /// <summary>
        /// 对象代码
        /// </summary>
        public string ObjCode { get; set; } = string.Empty;
        /// <summary>
        /// 对象名称
        /// </summary>
        public string ObjName { get; set; } = string.Empty;
        /// <summary>
        /// 说明
        /// </summary>
        public string ObjDescription { get; set; } = string.Empty;
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;

        /// <summary>
        /// 勾选
        /// </summary>
        public bool IsChecked { get; set; } = false;
        /// <summary>
        /// 操作对象对应的操作权限
        /// </summary>
        public List<PermissionCodeItemDto> Codes { get; set; } = new List<PermissionCodeItemDto>();
    }
    /// <summary>
    /// 角色操作代码对象
    /// </summary>
    public class PermissionCodeItemDto
    {

        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;
        /// <summary>
        /// 对象代码
        /// </summary>
        public string ObjCode { get; set; } = string.Empty;
        /// <summary>
        /// 对象代码名称
        /// </summary>
        public string ObjName { get; set; } = string.Empty;
        /// <summary>
        /// 操作代码
        /// </summary>
        public string PermissionCode { get; set; } = string.Empty;
        /// <summary>
        /// 操作名称
        /// </summary>
        public string PermissionName { get; set; } = string.Empty;

        /// <summary>
        /// 勾选
        /// </summary>
        public bool IsChecked { get; set; } = false;
    }
    /// <summary>
    /// 角色列表请求对象
    /// </summary>
    public class RoleListRequestDto { 
        /// <summary>
        /// 
        /// </summary>
        public int RoleLeve { get; set; } = 0; // 角色级别，0表示所有角色
    }
    /// <summary>
    /// 角色列表响应对象
    /// </summary>
    public class RoleListResponseDto {
        /// <summary>
        /// 请求状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 角色列表数据
        /// </summary>
        public List<RoleItemDto> Data { get; set; } = new List<RoleItemDto>();
    }
    /// <summary>
    /// 角色应用请求对象   
    /// </summary>
    public class RoleAppListRequestDto
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 选中的角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 角色应用响应对象
    /// </summary>
    public class RoleAppListResponseDto
    {
        /// <summary>
        /// 请求状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 角色操作权限列表数据
        /// </summary>
        public List<RoleAppsDto>? Data { get; set; } = new List<RoleAppsDto>();
    }
    /// <summary>
    /// 角色信息请求对象
    /// </summary>
    public class RoleInfoRequestDto { 
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 角色信息响应对象
    /// </summary>
    public class RoleInfoResponseDto
    {
        /// <summary>
        /// 请求状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 角色信息
        /// </summary>
        public RoleItemDto Role { get; set; } = new RoleItemDto();
        /// <summary>
        /// 角色对应的应用
        /// </summary>
        public List<RoleAppsDto> RoleApps { get; set; } = new List<RoleAppsDto>();
    }
    /// <summary>
    /// 角色保存请求对象
    /// </summary>
    public class RoleSaveRequestDto
    {
        /// <summary>
        /// 角色信息
        /// </summary>
        public RoleItemDto Role { get; set; } = new RoleItemDto();
        /// <summary>
        /// 角色对应的应用
        /// </summary>
        public List<RoleAppsDto> RoleApps { get; set; } = new List<RoleAppsDto>();
    }
    /// <summary>
    /// 角色保存响应对象
    /// </summary>
    public class RoleSaveResponseDto
    {
        /// <summary>
        /// 请求状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 角色信息
        /// </summary>
        public RoleItemDto Role { get; set; } = new RoleItemDto();
    }
    /// <summary>
    /// 角色列表请求对象
    /// </summary>
    public class RoleStateRequestDto
    {
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
    /// <summary>
    /// 角色列表响应对象
    /// </summary>
    public class RoleStateResponseDto
    {
        /// <summary>
        /// 请求状态码
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 内容
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 角色列表数据
        /// </summary>
        public List<RoleItemDto> Data { get; set; } = new List<RoleItemDto>();
    }

}
