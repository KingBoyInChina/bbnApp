using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Domain.Entities.Business
{
    /// <summary>
    /// 用户网关
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(GetWayId))] // 配置复合主键
    public class UserGetWays
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("网关ID")]
        [Comment("网关ID")]
        [Required]
        public string GetWayId { get; set; }

        [Column( TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户ID")]
        [Comment("用户ID")]
        [Required]
        public string UserId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("网关设备ID,网关设备对应标准代码")]
        [Comment("网关设备ID,网关设备对应标准代码")]
        [Required]
        public string DeviceId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("网关名称，自定义名称")]
        [Comment("网关名称,网关名称")]
        [Required]
        public string GetWayName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("网关编号")]
        [Comment("网关编号")]
        [Required]
        public string GetWayNumber { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("安装位置")]
        [Comment("安装位置")]
        [Required]
        public string GetWayLocation { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("网络连接方式,4G/Wifi/Wlan")]
        [Comment("网络连接方式")]
        [Required]
        public string WlanType { get; set; }

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
