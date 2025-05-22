using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 设备构成
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(StructId))] // 配置复合主键
    public class DeviceStruct
    {

        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("构成ID")]
        [Comment("构成ID")]
        [Required]
        public string StructId { get; set; }

        [Column( TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("设备ID")]
        [Comment("设备ID")]
        [Required]
        public string DeviceId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("物资ID")]
        [Comment("物资ID")]
        [Required]
        public string MaterialId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("物资名称")]
        [Comment("物资名称")]
        [Required]
        public string MaterialName { get; set; }

        [Column(TypeName = "int")]
        [MaxLength(3)]
        [Description("使用数量")]
        [Comment("使用数量")]
        [Required]
        public int UtilizeQuantity { get; set; }

        [MaxLength(40)]
        [Column(TypeName = "varchar(40)")]
        [Description("计量单位")]
        [Comment("计量单位")]
        public string QuantityUnit { get; set; }

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
