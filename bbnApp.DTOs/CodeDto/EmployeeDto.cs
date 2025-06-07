using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 树形对象
    /// </summary>
    public class EmployeeTreeItemDto
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
        /// 部门最高管理
        /// </summary>
        public bool DepartMentMaster { get; set; }
        /// <summary>
        /// Children
        /// </summary>
        public List<EmployeeTreeItemDto>? SubItems { get; set; }
    }
    /// <summary>
    /// 员工对象
    /// </summary>
    public class EmployeeItemDto
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
        /// 员工代码
        /// </summary>
        public string EmployeeCode { get; set; } = string.Empty;

        /// <summary>
        /// 员工工号
        /// </summary>
        public string EmployeeNum { get; set; } = string.Empty;

        /// <summary>
        /// 所在部门ID
        /// </summary>
        public string DepartMentId { get; set; } = string.Empty;
        /// <summary>
        /// 部门最高管理人员
        /// </summary>
        public bool DepartMentMaster { get; set; } = false;

        /// <summary>
        /// 所在部门名称
        /// </summary>
        public string DepartMentName { get; set; } = string.Empty;

        /// <summary>
        /// 职务等级
        /// </summary>
        public byte PositionLeve { get; set; } = 0;

        /// <summary>
        /// 职务名称
        /// </summary>
        public string Position { get; set; } = string.Empty;

        /// <summary>
        /// 员工性别
        /// </summary>
        public string Gender { get; set; } = string.Empty;

        /// <summary>
        /// 出生日期
        /// </summary>
        public string BirthDate { get; set; } = string.Empty; // 使用字符串格式化日期

        /// <summary>
        /// 员工证件号码
        /// </summary>
        public string? IDCardNumber { get; set; } = string.Empty;

        /// <summary>
        /// 员工联系电话
        /// </summary>
        public string PhoneNum { get; set; } = string.Empty;

        /// <summary>
        /// 员工邮箱
        /// </summary>
        public string EmailNum { get; set; } = string.Empty;

        /// <summary>
        /// 员工通信地址
        /// </summary>
        public string? CommunicationAddress { get; set; } = string.Empty;

        /// <summary>
        /// 入职时间
        /// </summary>
        public string DateOfEmployment { get; set; } = string.Empty; // 使用字符串格式化日期

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
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? ReMarks { get; set; } = string.Empty;

        /// <summary>
        /// 机构名称
        /// </summary>
        public string? CompanyName { get; set; } = string.Empty;
    }
    /// <summary>
    /// 人员树查询请求对象
    /// </summary>
    public class EmployeeTreeRequestDto
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
    }
    /// <summary>
    /// 人员树响应对象
    /// </summary>
    public class EmployeeTreeResponseDto
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
        public List<EmployeeTreeItemDto> Items { get; set; } = new List<EmployeeTreeItemDto>();
    }
    /// <summary>
    /// 人员清单分页查询请求对象
    /// </summary>
    public class EmployeeSearchRequestDto
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;
        /// <summary>
        /// 部门ID
        /// </summary>
        public string? DepartMentId { get; set; } = string.Empty;
        /// <summary>
        /// 员工ID
        /// </summary>
        public string? EmployeeName { get; set; } = string.Empty;
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
    /// 人员清单分页查询响应对象
    /// </summary>
    public class EmployeeSearchResponseDto
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
        /// 员工列表
        /// </summary>
        public List<EmployeeItemDto> Items { get; set; } = new List<EmployeeItemDto>();
    }
    /// <summary>
    /// 人员清单请求对象
    /// </summary>
    public class EmployeeItemsRequestDto
    {
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyName { get; set; } = string.Empty;
        /// <summary>
        /// 部门ID
        /// </summary>
        public string? DepartMentId { get; set; } = string.Empty;
        /// <summary>
        /// 员工ID
        /// </summary>
        public string? EmployeeName { get; set; } = string.Empty;
    }
    /// <summary>
    /// 人员清单响应对象
    /// </summary>
    public class EmployeeItemsResponseDto
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
        public List<EmployeeItemDto> Items { get; set; } = new List<EmployeeItemDto>();
    }
    /// <summary>
    /// 人员提交请求对象
    /// </summary>
    public class EmployeeSaveRequestDto
    {
        /// <summary>
        /// 人员对象
        /// </summary>
        public EmployeeItemDto Item { get; set; } = new EmployeeItemDto();
    }
    /// <summary>
    /// 人员提交响应对象
    /// </summary>
    public class EmployeeSaveResponseDto
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
        public EmployeeItemDto Item { get; set; } = new EmployeeItemDto();
    }
    /// <summary>
    /// 人员状态请求对象
    /// </summary>
    public class EmployeeStateRequestDto
    {
        /// <summary>
        ///类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 员工ID
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;
        /// <summary>
        /// 原因说明
        /// </summary>
        public string Reason  { get; set; } = string.Empty;
    }
    /// <summary>
    /// 人员提交响应对象
    /// </summary>
    public class EmployeeStateResponseDto
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
        public EmployeeItemDto Item { get; set; } = new EmployeeItemDto();
    }
    /// <summary>
    /// 人员状态请求对象
    /// </summary>
    public class EmployeeInfoRequestDto
    {
        /// <summary>
        /// 员工ID
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;
        public string CompanyId { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }
    /// <summary>
    /// 人员提交响应对象
    /// </summary>
    public class EmployeeInfoResponseDto
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
        public EmployeeItemDto Item { get; set; } = new EmployeeItemDto();
    }

}
