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

}
