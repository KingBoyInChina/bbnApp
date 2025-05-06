using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 节点树读取-请求对象
    /// </summary>
    public class OperationObjectTreeRequestDto
    {

    }
    /// <summary>
    /// 节点树读取-响应对象
    /// </summary>
    public class OperationObjectTreeResponseDto
    {
        public bool Code { get; set; }
        public string Message { get; set; }
        public List<OperationObjectNodeDto> Item { get; set; }
    }
    /// <summary>
    /// 清单读取-请求对象
    /// </summary>
    public class OperationObjectCodeListRequestDto
    {

    }
    /// <summary>
    /// 清单读取-响应对象
    /// </summary>
    public class OperationObjectCodeListResponseDto
    {
        public bool Code { get; set; }
        public string Message { get; set; }
        public List<OperationObjectCodeDto> Item { get; set; }
    }
    /// <summary>
    /// 详细信息读取-请求对象
    /// </summary>
    public class GetOperationInfoRequestDto
    {
        public string ObjCode { get; set; }
    }
    /// <summary>
    /// 详细信息读取-响应对象
    /// </summary>
    public class GetOperationInfoResponseDto
    {
        public bool Code { get; set; }
        public string Message { get; set; }
        public OperationObjectCodeDto Obj { get; set; }
        public List<ObjectOperationTypeDto> Item { get; set; }
    }
    /// <summary>
    /// 标准对象维护-请求对象
    /// </summary>
    public class SaveOperationInfoRequestDto
    {
        public OperationObjectCodeDto Data { get; set; }
    }
    /// <summary>
    /// 标准对象维护-响应对象
    /// </summary>
    public class SaveOperationInfoResponseDto
    {
        public bool Code { get; set; }
        public string Message { get; set; }
        public OperationObjectCodeDto Obj { get; set; }
    }
    /// <summary>
    /// 标准对象状态变更-请求对象
    /// </summary>
    public class OperationStateRequestDto
    {
        public string Type { get; set; }
        public string ObjCode { get; set; }
    }
    /// <summary>
    /// 标准对象状态变更-响应对象
    /// </summary>
    public class OperationStateResponseDto
    {
        public bool Code { get; set; }
        public string Message { get; set; }
        public OperationObjectCodeDto Obj { get; set; }
    }
    /// <summary>
    /// 标准对象操作代码设置-请求对象
    /// </summary>
    public class ItemSaveRequestDto
    {
        public ObjectOperationTypeDto Item { get; set; }
    }
    /// <summary>
    /// 标准对象操作代码设置-响应对象
    /// </summary>
    public class ItemSaveResponseDto
    {
        public bool Code { get; set; }
        public string Message { get; set; }
    }
    /// <summary>
    /// 操作对象树节点
    /// </summary>
    public class OperationObjectNodeDto
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
        public bool IsLeaf { get; set; }
        /// <summary>
        /// 锁定
        /// </summary>
        public bool IsLock { get; set; }
        /// <summary>
        /// Children
        /// </summary>
        public List<OperationObjectNodeDto>? SubItems { get; set; }
    }
    /// <summary>
    /// 标准操作对象
    /// </summary>
    public class OperationObjectCodeDto
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
        /// 操作对象代码
        /// </summary>
        public string ObjCode { get; set; }

        /// <summary>
        /// 操作对象名称
        /// </summary>
        public string ObjName { get; set; }

        /// <summary>
        /// 操作对象说明
        /// </summary>
        public string? ObjDescription { get; set; }

        /// <summary>
        /// 锁定
        /// </summary>
        public byte IsLock { get; set; }

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
    /// 标准操作对象对应的对象代码
    /// </summary>
    public class ObjectOperationTypeDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; }
        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; }

        /// <summary>
        /// 对象代码
        /// </summary>
        public string ObjCode { get; set; }

        /// <summary>
        /// 权限代码
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// 权限代码名称
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// 权限代码说明
        /// </summary>
        public string? PermissionDescription { get; set; }

        /// <summary>
        /// 删除状态
        /// </summary>
        public byte IsDelete { get; set; }
        /// <summary>
        /// 已选择
        /// </summary>
        public bool IsChecked { get; set; }
    }
    /// <summary>
    /// 标准权限代码类型
    /// </summary>
    public class PermissionsCodeDto
    {
        /// <summary>
        /// 序号
        /// </summary>
        public int IdxNum { get; set; }
        /// <summary>
        /// 用户组ID
        /// </summary>
        public string Yhid { get; set; }

        /// <summary>
        /// 权限代码ID
        /// </summary>
        public string PermissionId { get; set; }

        /// <summary>
        /// 权限代码
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// 权限代码名称
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// 权限代码说明
        /// </summary>
        public string? PermissionDescription { get; set; }

        /// <summary>
        /// 锁定
        /// </summary>
        public byte IsLock { get; set; }

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

}
