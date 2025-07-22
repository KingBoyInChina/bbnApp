using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbnApp.Domain.Entities.Business
{
    /// <summary>
    /// 用户网关对应的
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(GetWayDeviceId))] // 配置复合主键
    public class UserGetWayDevices
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("网关管理的设备ID")]
        [Comment("网关管理的设备ID")]
        [Required]
        public string GetWayDeviceId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("网关ID")]
        [Comment("网关ID")]
        [Required]
        public string GetWayId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户ID")]
        [Comment("用户ID")]
        [Required]
        public string UserId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("设备ID,设备对应标准代码")]
        [Comment("设备ID,设备对应标准代码")]
        [Required]
        public string DeviceId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("设备编号")]
        [Comment("设备编号")]
        [Required]
        public string DeviceNumber { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("从站地址")]
        [Comment("从站地址")]
        [Required]
        public string SlaveId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("从站名称")]
        [Comment("从站名称")]
        [Required]
        public string SlaveName { get; set; }

        [Column(TypeName = "datetime")]
        [Description("安装时间")]
        [Comment("安装时间")]
        public DateTime InstallTime { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("安装人")]
        [Comment("安装人")]
        [Required]
        public string Installer { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("安装人ID")]
        [Comment("安装人ID")]
        [Required]
        public string InstallerId { get; set; }

        [Column(TypeName = "datetime")]
        [Description("维保到期时间")]
        [Comment("维保到期时间")]
        public DateTime Warranty { get; set; }

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

        [Column(TypeName = "tinyint")]
        [Description("删除标志")]
        [Comment("删除标志")]
        [Required]
        public byte IsDelete { get; set; }

        [Column(TypeName = "datetime")]
        [Description("末次数据变更时间")]
        [Comment("末次数据变更时间")]
        [Required]
        public DateTime LastModified { get; set; }
    }
}
