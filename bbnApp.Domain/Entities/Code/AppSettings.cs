using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 系统配置
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(SettingId))] // 配置复合主键
    public class AppSettings
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("配置ID")]
        [Comment("配置ID")]
        [Required]
        public string SettingId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("配置代码")]
        [Comment("配置代码")]
        [Required]
        public string SettingCode { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("配置名称")]
        [Comment("配置名称")]
        [Required]
        public string SettingName { get; set; }

        [Column(TypeName = "varchar(200)")]
        [MaxLength(200)]
        [Description("配置说明")]
        [Comment("配置说明")]
        [Required]
        public string SettingDesc { get; set; }

        [Column(TypeName = "int")]
        [MaxLength(3)]
        [Description("序号")]
        [Comment("序号")]
        [Required]
        public int SettingIndex { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("配置参数类型")]
        [Comment("配置参数类型")]
        [Required]
        public string SettingType { get; set; }

        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        [Description("当前值")]
        [Comment("当前值")]
        [Required]
        public string NowValue { get; set; }

        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        [Description("默认值")]
        [Comment("默认值")]
        [Required]
        public string DefaultValue { get; set; }

        [Column(TypeName = "varchar(400)")]
        [MaxLength(400)]
        [Description("取值范围")]
        [Comment("取值范围")]
        [Required]
        public string? ValueRange { get; set; }

        [MaxLength(1)]
        [Column(TypeName = "tinyint")]
        [Description("固定")]
        [Comment("固定")]
        [Required]
        public byte IsFiexed { get; set; }

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
