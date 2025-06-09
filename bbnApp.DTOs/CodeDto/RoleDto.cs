using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    /// 角色操作权限分配
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
        /// 操作代码
        /// </summary>
        public string PermissionCode { get; set; } = string.Empty;

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
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;

        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 角色可以使用的应用分配
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
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;

        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }

}
