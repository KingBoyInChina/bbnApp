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
    /// 用户边缘盒子
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(BoxId))] // 配置复合主键
    public class UserBoxs
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("盒子ID")]
        [Comment("盒子ID")]
        [Required]
        public string BoxId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户ID")]
        [Comment("用户ID")]
        [Required]
        public string UserId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("设备ID,盒子对应的标准代码")]
        [Comment("设备ID,盒子对应的标准代码")]
        [Required]
        public string DeviceId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("盒子名称")]
        [Comment("盒子名称")]
        [Required]
        public string BoxName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("盒子编号")]
        [Comment("盒子编号")]
        [Required]
        public string BoxNumber { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("安装位置")]
        [Comment("安装位置")]
        [Required]
        public string BoxLocation { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("网络连接方式,4G/Wifi/Wlan")]
        [Comment("网络连接方式")]
        [Required]
        public string WlanType { get; set; }

        [Column(TypeName = "INT")]
        [Description("带宽,M")]
        [Comment("带宽,M")]
        [Required]
        public int TapeWidth { get; set; }

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
