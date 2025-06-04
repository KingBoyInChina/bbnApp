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
    public class DepartMentTreeItemDto
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
        public List<DepartMentTreeItemDto>? SubItems { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class DepartMentInfoDto
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
        /// 部门ID
        /// </summary>
        public string DepartMentId { get; set; } = string.Empty;

        /// <summary>
        /// 上级部门ID
        /// </summary>
        public string PDepartMentId { get; set; } = string.Empty;

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartMentName { get; set; } = string.Empty;

        /// <summary>
        /// 部门代码
        /// </summary>
        public string DepartMentCode { get; set; } = string.Empty;

        /// <summary>
        /// 部门序号
        /// </summary>
        public byte DepartMentIndex { get; set; } = 0;

        /// <summary>
        /// 部门位置
        /// </summary>
        public string? DepartMentLocation { get; set; } = string.Empty;

        /// <summary>
        /// 部门介绍
        /// </summary>
        public string? DepartMentDescription { get; set; } = string.Empty;

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
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 部门树请求对象
    /// </summary>
    public class DepartMentTreeRequestDto
    {
        /// 公司代码
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 部门树返回对象
    /// </summary>
    public class DepartMentTreeResponseDto
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Code { get; set; } = false;
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; } = string.Empty;

        public List<DepartMentTreeItemDto> Items { get; set; }
    }
    /// <summary>
    /// 部门清单请求对象
    /// </summary>
    public class DepartMentSearchRequestDto
    {
        /// <summary>
        /// 公司名称
        /// </summary>
        public string DepartMentName { get; set; } = string.Empty;
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 部门清单返回对象
    /// </summary>
    public class DepartMentSearchResponseDto
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
        /// 部门清单
        /// </summary>
        public List<DepartMentInfoDto> Items { get; set; }
    }
    /// <summary>
    /// 部门信息请求
    /// </summary>
    public class DepartMentInfoRequestDto
    {
        /// <summary>
        /// 部门id
        /// </summary>
        public string DepartMentId { get; set; } = string.Empty;
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 部门信息返回对象
    /// </summary>
    public class DepartMentInfoResponseDto
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
        public DepartMentInfoDto Item { get; set; } = new DepartMentInfoDto();
    }
    /// <summary>
    /// 部门提交请求对象
    /// </summary>
    public class DepartMentSaveRequestDto
    {
        public DepartMentInfoDto Item { get; set; } = new DepartMentInfoDto();
    }
    /// <summary>
    /// 部门提交返回对象
    /// </summary>
    public class DepartMentSaveResponseDto
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
        public DepartMentInfoDto Item { get; set; } = new DepartMentInfoDto();
    }
    /// <summary>
    /// 部门状态请求对象
    /// </summary>
    public class DepartMentStateRequestDto
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; } = string.Empty;
        /// <summary>
        /// 部门ID
        /// </summary>
        public string DepartMentId { get; set; } = string.Empty;
        /// <summary>
        /// 原因
        /// </summary>
        public string Reason { get; set; } = string.Empty;
        /// <summary>
        /// 机构ID
        /// </summary>
        public string CompanyId { get; set; } = string.Empty;
    }
    /// <summary>
    /// 部门状态返回对象
    /// </summary>
    public class DepartMentStateResponseDto
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
        public DepartMentInfoDto Item { get; set; } = new DepartMentInfoDto();
    }
}
