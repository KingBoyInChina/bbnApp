using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbnApp.Domain.Entities.Lot
{
    /// <summary>
    /// 物联网设备动作记录
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(ActionId))] // 配置复合主键
    public class LotDeviceAction
    {

        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1, TypeName = "int")]
        [Description("动作ID")]
        [Comment("动作ID")]
        [Required]
        public int ActionId { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("编号,区分是否手动触发的同组操作")]
        [Comment("编号,区分是否手动触发的同组操作")]
        [Required]
        public string? GroupNumber { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户ID")]
        [Comment("用户ID")]
        [Required]
        public string UserId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("网关ID")]
        [Comment("网关ID")]
        [Required]
        public string GetWayId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("设备ID")]
        [Comment("设备ID")]
        [Required]
        public string DeviceId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("动作类型")]
        [Comment("动作类型")]
        [Required]
        public string ActionType { get; set; }


        [Column(TypeName = "datetime")]
        [Description("记录时间")]
        [Comment("记录时间")]
        public DateTime ActionDate { get; set; }

        [Column(TypeName = "varchar(80)")]
        [Description("原始数据")]
        [Comment("原始数据")]
        public string OriginalData { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Description("特殊数据")]
        [Comment("特殊数据")]
        public string? MeatData { get; set; }

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
