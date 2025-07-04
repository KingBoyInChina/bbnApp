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
    /// 边缘盒子对应的摄像头
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(CameraId))] // 配置复合主键
    public class UserCameras
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("摄像头ID")]
        [Comment("摄像头ID")]
        [Required]
        public string CameraId { get; set; }

        [Column(TypeName = "varchar(40)")]
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
        [Description("设备ID,摄像头对应标准代码")]
        [Comment("设备ID,摄像头对应标准代码")]
        [Required]
        public string DeviceId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("摄像头编号")]
        [Comment("摄像头编号")]
        [Required]
        public string CameraNumber { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("摄像头IP")]
        [Comment("摄像头IP")]
        [Required]
        public string CameraIp { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("摄像头通道号")]
        [Comment("摄像头通道号")]
        [Required]
        public string CameraChannel { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("摄像头自定义名称")]
        [Comment("摄像头自定义名称")]
        [Required]
        public string CameraName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("摄像头登录名")]
        [Comment("摄像头登录名")]
        [Required]
        public string? CameraAdmin { get; set; }

        [Column(TypeName = "varchar(200)")]
        [MaxLength(200)]
        [Description("摄像头登录密码")]
        [Comment("摄像头登录密码")]
        [Required]
        public string? CameraPassword { get; set; }

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
