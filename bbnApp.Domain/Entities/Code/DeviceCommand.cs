using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 设备命令
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(CommandId))] // 配置复合主键
    public class DeviceCommand
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("指令ID")]
        [Comment("指令ID")]
        [Required]
        public string CommandId { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("设备ID")]
        [Comment("设备ID")]
        [Required]
        public string DeviceId { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("命令名称")]
        [Comment("命令名称")]
        [Required]
        public string CommandName { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("命令代码")]
        [Comment("命令代码")]
        [Required]
        public string CommandCode { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("硬件通信协议")]
        [Comment("硬件通信协议")]
        [Required]
        public string HardwareCP { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("应用通信协议")]
        [Comment("应用通信协议")]
        public string? ApplicationCP { get; set; }

        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        [Description("设备地址")]
        [Comment("设备地址")]
        public string? DeviceAddr { get; set; }

        [MaxLength(10)]
        [Column(TypeName = "varchar(10)")]
        [Description("功能码")]
        [Comment("功能码")]
        public string? FunctionCode { get; set; }

        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        [Description("起始地址,大小端")]
        [Comment("起始地址,大小端")]
        public string? StartAddr { get; set; }

        [MaxLength(20)]
        [Column(TypeName = "varchar(20)")]
        [Description("寄存器数量")]
        [Comment("寄存器数量")]
        public string? RegCount { get; set; }

        [Column(TypeName = "tinyint(1)")]
        [Description("校验码")]
        [Comment("校验码")]
        [Required]
        public bool CRC { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [Description("手动构造命令")]
        [Comment("手动构造命令")]
        public string? CommandInfo { get; set; }

        [MaxLength(100)]
        [Column(TypeName = "varchar(100)")]
        [Description("命令参数说明")]
        [Comment("命令参数说明")]
        public string? CommandDescription { get; set; }

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
