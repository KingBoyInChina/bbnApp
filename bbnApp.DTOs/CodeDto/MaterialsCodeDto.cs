using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.DTOs.CodeDto
{
    /// <summary>
    /// 便于DTO中使用
    /// </summary>
    public class MaterialTreeItemDto
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
        public List<MaterialTreeItemDto>? SubItems { get; set; }
    }
    /// <summary>
    /// 物资代码对象
    /// </summary>
    public class MaterialsCodeDto
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
        /// 物资ID
        /// </summary>
        public string MaterialId { get; set; }

        /// <summary>
        /// 物资类型
        /// </summary>
        public string MaterialType { get; set; }

        /// <summary>
        /// 物资名称
        /// </summary>
        public string MaterialName { get; set; }

        /// <summary>
        /// 物资代码
        /// </summary>
        public string MaterialCode { get; set; }

        /// <summary>
        /// 物资条码
        /// </summary>
        public string MaterialBarCode { get; set; }

        /// <summary>
        /// 物资形态
        /// </summary>
        public string MaterialForm { get; set; }

        /// <summary>
        /// 物资材质
        /// </summary>
        public string MaterialSupplies { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int MaterialIndex { get; set; }

        /// <summary>
        /// 危险物
        /// </summary>
        public byte IsDanger { get; set; }

        /// <summary>
        /// 危险分类
        /// </summary>
        public string DangerType { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string? Specifications { get; set; }

        /// <summary>
        /// 计量单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 存储环境
        /// </summary>
        public string StorageEnvironment { get; set; }

        /// <summary>
        /// 其他参数
        /// </summary>
        public string OtherParames { get; set; }

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
    /// 物资树请求对象
    /// </summary>
    public class MaterialsCodeTreeRequestDto
    {
        public string FilterKey { get; set; }
    }
    /// <summary>
    /// 物资树响应
    /// </summary>
    public class MaterialsCodeTreeResponseDto
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
        /// 树
        /// </summary>
        public List<MaterialTreeItemDto>? Items { get; set; }
    }
    /// <summary>
    /// 物资树请求对象
    /// </summary>
    public class MaterialsCodeInfoRequestDto
    {
        public string MaterialId { get; set; }
    }
    /// <summary>
    /// 物资信息读取响应
    /// </summary>
    public class MaterialsCodeInfoResponseDto
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
        /// 树
        /// </summary>
        public MaterialsCodeDto? Item { get; set; }
    }
    /// <summary>
    /// 物资清单求对象
    /// </summary>
    public class MaterialsCodeListRequestDto
    {
        public string MaterialType { get; set; }
    }
    /// <summary>
    /// 物资清单读取响应
    /// </summary>
    public class MaterialsCodeListResponseDto
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
        /// 树
        /// </summary>
        public List<MaterialsCodeDto>? Item { get; set; }
    }
    /// <summary>
    /// 物资信息提交请求对象
    /// </summary>
    public class MaterialsCodeSaveRequestDto
    {
        public MaterialsCodeDto? Item { get; set; }
    }
    /// <summary>
    /// 物资信息提交响应
    /// </summary>
    public class MaterialsCodeSaveResponseDto
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
        /// 树
        /// </summary>
        public MaterialsCodeDto? Item { get; set; }
    }

    /// <summary>
    /// 物资信息提交请求对象
    /// </summary>
    public class MaterialsCodeStateRequestDto
    {
        public string Type { get; set; }
        public string MaterialId { get; set; }
    }
    /// <summary>
    /// 物资信息提交响应
    /// </summary>
    public class MaterialsCodeStateResponseDto
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
        /// 树
        /// </summary>
        public MaterialsCodeDto? Item { get; set; }
    }
}
