using bbnApp.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.BusinessDto
{
    /// <summary>
    /// 树形对象
    /// </summary>
    public class UserTreeItemDto
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
        public List<UserTreeItemDto>? SubItems { get; set; }
    }
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInformationDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } = 1;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 用户类型
        /// </summary>
        public string UserType { get; set; } = string.Empty;

        /// <summary>
        /// 用户等级
        /// </summary>
        public string UserLeve { get; set; } = string.Empty;

        /// <summary>
        /// 种养规模
        /// </summary>
        public string Scale { get; set; } = string.Empty;

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserNumber { get; set; } = string.Empty;

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; } = string.Empty;

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// 所在地名称
        /// </summary>
        public string AreaName { get; set; } = string.Empty;

        /// <summary>
        /// 所在地ID
        /// </summary>
        public string AreaId { get; set; } = string.Empty;

        /// <summary>
        /// 通信地址
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 位置坐标
        /// </summary>
        public string? Point { get; set; } = string.Empty;

        /// <summary>
        /// 停用
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
    /// 联系人
    /// </summary>
    public class UserContactDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } =1;

        /// <summary>
        /// 联系人ID
        /// </summary>
        public string ContactId { get; set; } = string.Empty;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; } = string.Empty;

        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// 主要联系人
        /// </summary>
        public bool IsFirst { get; set; } = false;

        /// <summary>
        /// 职务
        /// </summary>
        public string? Jobs { get; set; } = string.Empty;

        /// <summary>
        /// 停用
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
    /// 种养信息
    /// </summary>
    public class UserAabInformationDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; } =1;

        /// <summary>
        /// 种养信息ID
        /// </summary>
        public string AabId { get; set; } = string.Empty;

        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// 种养类型
        /// </summary>
        public string AABType { get; set; } = string.Empty;

        /// <summary>
        /// 种养分类
        /// </summary>
        public string Categori { get; set; } = string.Empty;

        /// <summary>
        /// 种养名称
        /// </summary>
        public string ObjName { get; set; } = string.Empty;

        /// <summary>
        /// 种养名称代码
        /// </summary>
        public string ObjCode { get; set; } = string.Empty;

        /// <summary>
        /// 面积
        /// </summary>
        public decimal AreaNumber { get; set; } = 0;

        /// <summary>
        /// 面积单位
        /// </summary>
        public string AreaNumberUnit { get; set; } = string.Empty;

        /// <summary>
        /// 分布情况
        /// </summary>
        public string Distribution { get; set; } = string.Empty;

        /// <summary>
        /// 主要位置
        /// </summary>
        public string? Point { get; set; } = string.Empty;

        /// <summary>
        /// 停用
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
    /// 用户信息树查询请求
    /// </summary>
    public class UserInformationTreeRequestDto
    {
        /// <summary>
        /// 所在地区
        /// </summary>
        public string AreaId { get; set; } = string.Empty;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
    }
    /// <summary>
    /// 用户信息树查询响应
    /// </summary>
    public class UserInformationTreeResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 响应数据集
        /// </summary>
        public List<UserTreeItemDto> Items = new List<UserTreeItemDto>();
    }
    /// <summary>
    /// 用户信息树查询请求
    /// </summary>
    public class UserInformationListRequestDto
    {
        /// <summary>
        /// 所在地区
        /// </summary>
        public string AreaId { get; set; } = string.Empty;
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 联系电话
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
    }
    /// <summary>
    /// 用户信息树查询响应
    /// </summary>
    public class UserInformationListResponseDto
    { 
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get;set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 响应数据集
        /// </summary>
        public List<UserInformationDto> Items = new List<UserInformationDto>();
    }
    /// <summary>
    /// 用户信息读取请求
    /// </summary>
    public class UserInformationLoadRequestDto
    { 
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; }=string.Empty;
    }
    /// <summary>
    /// 用户信息读取响应
    /// </summary>
    public class UserInformationLoadResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 基础信息
        /// </summary>
        public UserInformationDto User { get; set; } = new UserInformationDto();
        /// <summary>
        /// 联系人信息
        /// </summary>
        public List<UserContactDto> Contacts = new List<UserContactDto>();
        /// <summary>
        /// 种养信息
        /// </summary>
        public List<UserAabInformationDto> Aabs = new List<UserAabInformationDto>();
    }
    /// <summary>
    /// 用户信息提交请求
    /// </summary>
    public class UserInformationSaveRequestDto {
        /// <summary>
        /// 基础信息
        /// </summary>
        public UserInformationDto User { get; set; } = new UserInformationDto();
        /// <summary>
        /// 联系人信息
        /// </summary>
        public List<UserContactDto> Contacts = new List<UserContactDto>();
        /// <summary>
        /// 种养信息
        /// </summary>
        public List<UserAabInformationDto> Aabs = new List<UserAabInformationDto>();
    }
    /// <summary>
    /// 用户信息提交响应
    /// </summary>
    public class UserInformationSaveResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 基础信息
        /// </summary>
        public UserInformationDto User { get; set; } = new UserInformationDto();
        /// <summary>
        /// 联系人信息
        /// </summary>
        public List<UserContactDto> Contacts = new List<UserContactDto>();
        /// <summary>
        /// 种养信息
        /// </summary>
        public List<UserAabInformationDto> Aabs = new List<UserAabInformationDto>();
    }
    /// <summary>
    /// 用户信息状态变更请求
    /// </summary>
    public class UserInformationStateRequestDto
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 用户ID
        /// </summary>
        public string UserId { get; set; } = string.Empty;
        /// <summary>
        /// 联系人id-针对联系人操作使用
        /// </summary>
        public string ContactId { get; set; } = string.Empty;
        /// <summary>
        /// 种养ID-Aabid
        /// </summary>
        public string AabId { get; set; } = string.Empty;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
    }
    /// <summary>
    /// 用户信息状态变更响应
    /// </summary>
    public class UserInformationStateResponseDto
    {
        /// <summary>
        ///请求状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// 基础信息
        /// </summary>
        public UserInformationDto User { get; set; } = new UserInformationDto();
        /// <summary>
        /// 联系人信息
        /// </summary>
        public List<UserContactDto> Contacts = new List<UserContactDto>();
        /// <summary>
        /// 种养信息
        /// </summary>
        public List<UserAabInformationDto> Aabs = new List<UserAabInformationDto>();
    }
}
