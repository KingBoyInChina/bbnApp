using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 设备代码
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(DeviceId))] // 配置复合主键
    public class DeviceCode
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("设备ID")]
        [Comment("设备ID")]
        [Required]
        public string DeviceId { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("设备名称")]
        [Comment("设备名称")]
        [Required]
        public string DeviceName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("设备代码")]
        [Comment("设备代码")]
        [Required]
        public string Code { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("设备分类")]
        [Comment("设备分类")]
        [Required]
        public string DeviceType { get; set; }

        [MaxLength(80)]
        [Column(TypeName = "varchar(80)")]
        [Description("规格")]
        [Comment("规格")]
        public string? DeviceSpecifications { get; set; }

        [MaxLength(80)]
        [Column(TypeName = "varchar(80)")]
        [Description("型号")]
        [Comment("型号")]
        public string? DeviceModel { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [Description("条码号")]
        [Comment("条码号")]
        public string? DeviceBarCode { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [Description("用途")]
        [Comment("用途")]
        public string? Usage { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [Description("存储环境")]
        [Comment("存储环境")]
        public string? StorageEnvironment { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [Description("使用环境")]
        [Comment("使用环境")]
        public string? UsageEnvironment { get; set; }

        [MaxLength(3)]
        [Column(TypeName = "int")]
        [Description("使用寿命")]
        [Comment("使用寿命")]
        public int ServiceLife { get; set; }

        [MaxLength(40)]
        [Column(TypeName = "varchar(40)")]
        [Description("计量单位")]
        [Comment("计量单位")]
        public string LifeUnit { get; set; }

        [MaxLength(1)]
        [Column(TypeName = "tinyint")]
        [Description("停用,0/1")]
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
