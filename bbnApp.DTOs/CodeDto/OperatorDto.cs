using bbnApp.Application.DTOs.LoginDto;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 操作员对象
    /// </summary>
    public class OperatorItemDto
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
        /// 员工ID
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;

        /// <summary>
        /// 上级管理ID
        /// </summary>
        public string PEmployeeId { get; set; } = string.Empty;

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmployeeName { get; set; } = string.Empty;

        /// <summary>
        /// 员工工号
        /// </summary>
        public string EmployeeNum { get; set; } = string.Empty;

        /// <summary>
        /// 所在部门ID
        /// </summary>
        public string DepartMentId { get; set; } = string.Empty;
        /// <summary>
        /// 职务等级ID
        /// </summary>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// 职务等级
        /// </summary>
        public byte PositionLeve { get; set; } = 0;

        /// <summary>
        /// 已分配权限
        /// </summary>
        public bool IsOperator { get; set; } =false;

        /// <summary>
        /// 操作员ID
        /// </summary>
        public string OperatorId { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord { get; set; } = string.Empty; // 解密后的密码

        /// <summary>
        /// 密码过期时间
        /// </summary>
        public string? PassWordExpTime { get; set; } = string.Empty;

        /// <summary>
        /// 停用状态
        /// </summary>
        public byte IsLock { get; set; } = 0;

        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 操作员角色
        /// </summary>
        public List<OperatorRoleDto> OperatorRoles { get; set; } = new List<OperatorRoleDto>();
    }
    /// <summary>
    /// 操作员对象
    /// </summary>
    public class WorkerItemDto
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
        /// 员工ID
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;

        /// <summary>
        /// 上级管理ID
        /// </summary>
        public string PEmployeeId { get; set; } = string.Empty;

        /// <summary>
        /// 员工姓名
        /// </summary>
        public string EmployeeName { get; set; } = string.Empty;

        /// <summary>
        /// 员工工号
        /// </summary>
        public string EmployeeNum { get; set; } = string.Empty;

        /// <summary>
        /// 所在部门ID
        /// </summary>
        public string DepartMentId { get; set; } = string.Empty;
        /// <summary>
        /// 所在部门名称
        /// </summary>
        public string DepartMentName { get; set; } = string.Empty;
        /// <summary>
        /// 职务等级ID
        /// </summary>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// 职务等级
        /// </summary>
        public byte PositionLeve { get; set; } = 0;

        /// <summary>
        /// 操作员ID
        /// </summary>
        public string OperatorId { get; set; } = string.Empty;

        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 操作员角色
    /// </summary>
    public class OperatorRoleDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;
        /// <summary>
        /// 操作员ID
        /// </summary>
        public string OperatorId { get; set; } = string.Empty;
        /// <summary>
        /// 角色ID
        /// </summary>
        public string RoleId { get; set; } = string.Empty;
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; } = string.Empty;
        /// <summary>
        /// 角色等级
        /// </summary>
        public byte RoleLeve { get; set; } = Convert.ToByte(9);
        /// <summary>
        /// 角色等级名称
        /// </summary>
        public string RoleLeveName { get; set; } = string.Empty;
        /// <summary>
        /// 角色说明
        /// </summary>
        public string RoleDescription { get; set; } = string.Empty;
        /// <summary>
        /// 勾选状态
        /// </summary>
        public bool IsChecked { get; set; } = false;
        /// <summary>
        /// 可选中
        /// </summary>
        public bool IsEnabled { get; set; } = true;
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 同事信息读取响应
    /// </summary>
    public class OperatorsLoadRequestDto
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 部门ID
        /// </summary>
        public string Yhid { get; set; } = string.Empty;
    }
    /// <summary>
    /// 同事信息读取响应
    /// </summary>
    public class OperatorsLoadResponseDto
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
        /// 员工列表
        /// </summary>
        public List<WorkerItemDto> Items { get; set; } = new List<WorkerItemDto>();
    }
    /// <summary>
    /// 操作员树查询请求对象
    /// </summary>
    public class OperatorListRequestDto
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 部门ID
        /// </summary>
        public string? DepartMentId { get; set; } = string.Empty;
        /// <summary>
        /// 员工ID
        /// </summary>
        public string? EmployeeName { get; set; } = string.Empty;
        /// <summary>
        /// 工号
        /// </summary>
        public string? EmployeeNum { get; set; } = string.Empty;
    }
    /// <summary>
    /// 操作员树响应对象
    /// </summary>
    public class OperatorListResponseDto
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
        /// 员工列表
        /// </summary>
        public List<OperatorItemDto> Items { get; set; } = new List<OperatorItemDto>();
    }
    /// <summary>
    /// 操作员信息请求对象
    /// </summary>
    public class OperatorInfoRequestDto
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;
        /// <summary>
        /// 公司ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 操作员ID
        /// </summary>
        public string OperatorId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 人员提交响应对象
    /// </summary>
    public class OperatorInfoResponseDto
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
        /// 员工列表
        /// </summary>
        public OperatorItemDto Item { get; set; } = new OperatorItemDto();
    }
    /// <summary>
    /// 操作员保存请求对象
    /// </summary>
    public class OperatorSaveRequestDto { 
        /// <summary>
        /// 角色对象
        /// </summary>
        public OperatorItemDto Item { get; set; } = new OperatorItemDto();
    }
    /// <summary>
    /// 操作员保存响应对象
    /// </summary>
    public class OperatorSaveResponseDto
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
        /// 员工列表
        /// </summary>
        public OperatorItemDto Item { get; set; } = new OperatorItemDto();
    }
    /// <summary>
    /// 操作员状态请求对象
    /// </summary>
    public class OperatorStateRequestDto
    {
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 操作员ID
        /// </summary>
        public string OperatorId { get; set; } = string.Empty;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
    /// <summary>
    /// 操作员状态响应对象
    /// </summary>
    public class OperatorStateResponseDto
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
        /// 操作员对象
        /// </summary>
        public OperatorItemDto Item { get; set; } = new OperatorItemDto();
    }
}
