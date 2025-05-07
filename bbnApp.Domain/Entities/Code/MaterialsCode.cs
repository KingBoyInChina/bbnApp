using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 物资代码
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(MaterialId))] // 配置复合主键
    public class MaterialsCode
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("物资ID")]
        [Comment("物资ID")]
        [Required]
        public string MaterialId { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(40)]
        [Description("物资类型")]
        [Comment("物资类型")]
        [Required]
        public string MaterialType { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("物资名称")]
        [Comment("物资名称")]
        [Required]
        public string MaterialName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("物资代码")]
        [Comment("物资代码")]
        [Required]
        public string MaterialCode { get; set; }


        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("物资条码")]
        [Comment("物资条码")]
        [Required]
        public string MaterialBarCode { get; set; }

        [Column(TypeName = "int")]
        [MaxLength(3)]
        [Description("序号")]
        [Comment("序号")]
        [Required]
        public int MaterialIndex { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("物资形态")]
        [Comment("物资形态")]
        [Required]
        public string MaterialForm { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("物资材质")]
        [Comment("物资材质")]
        [Required]
        public string MaterialSupplies { get; set; }

        [MaxLength(1)]
        [Column(TypeName = "bool")]
        [Description("危险物")]
        [Comment("危险物")]
        [Required]
        public byte IsDanger { get; set; }

        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        [Description("危险分类")]
        [Comment("危险分类")]
        [Required]
        public string DangerType { get; set; }

        [Column(TypeName = "varchar(400)")]
        [MaxLength(400)]
        [Description("规格")]
        [Comment("规格")]
        [Required]
        public string? Specifications { get; set; }

        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        [Description("计量单位")]
        [Comment("计量单位")]
        [Required]
        public string Unit { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("存储环境")]
        [Comment("存储环境")]
        [Required]
        public string StorageEnvironment { get; set; }

        [Column(TypeName = "varchar(200)")]
        [MaxLength(200)]
        [Description("其他参数")]
        [Comment("其他参数")]
        [Required]
        public string OtherParames { get; set; }

        [MaxLength(1)]
        [Column(TypeName = "tinyint")]
        [Description("停用")]
        [Comment("停用")]
        [Required]
        public byte IsLock { get; set; }

        [Column(TypeName = "datetime")]
        [Description("停用时间")]
        [Comment("停用时间")]
        public DateTime? LockTime { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("停用原因")]
        [Comment("停用原因")]
        public string? LockReason { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("备注信息")]
        [Comment("备注信息")]
        public string? ReMarks { get; set; }

        [MaxLength(1)]
        [Column(TypeName = "tinyint")]
        [Description("删除标志")]
        [Comment("删除标志")]
        [Required]
        public byte Isdelete { get; set; }

        [Column(TypeName = "datetime")]
        [Description("末次数据变更时间")]
        [Comment("末次数据变更时间")]
        [Required]
        public DateTime LastModified { get; set; }

    }
}
