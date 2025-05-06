using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 行政区划代码
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(AreaId))] // 配置复合主键
    public class AreaCode
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("行政区划ID")]
        [Comment("行政区划ID")]
        [Required]
        public string AreaId { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("行政区划父级ID")]
        [Comment("行政区划父级ID")]
        [Required]
        public string AreaPId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("行政区划名称")]
        [Comment("行政区划名称")]
        [Required]
        public string AreaName { get; set; }

        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        [Description("行政区划全称名称")]
        [Comment("行政区划全称名称")]
        [Required]
        public string AreaFullName { get; set; }

        [Column(TypeName = "tinyint")]
        [MaxLength(1)]
        [Description("行政区划级别")]
        [Comment("行政区划级别")]
        [Required]
        public byte AreaLeve { get; set; }

        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        [Description("行政区划级别名称")]
        [Comment("行政区划级别名称")]
        [Required]
        public string AreaLeveName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("行政区划所在经纬度")]
        [Comment("行政区划所在经纬度")]
        public string? AreaPoint { get; set; }

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
