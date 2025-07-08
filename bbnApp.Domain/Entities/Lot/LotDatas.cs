using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Domain.Entities.Lot
{
    /// <summary>
    /// 物联网数据采集表
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(DataId))] // 配置复合主键
    public class LotDatas
    {

        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(Order = 1, TypeName = "int")]
        [Description("数据ID")]
        [Comment("数据ID")]
        [Required]
        public int DataId { get; set; }

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
        [Description("传感器地址")]
        [Comment("传感器地址")]
        [Required]
        public string SensorAddr { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("编号,用于区分是否同一组数据")]
        [Comment("编号,用于区分是否同一组数据")]
        [Required]
        public string GroupNumber { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("数据类型")]
        [Comment("数据类型")]
        [Required]
        public string DataType { get; set; }

        [Column(TypeName = "DECIMAL(8,2)")]
        [Description("接收到的数据")]
        [Comment("接收到的数据")]
        [Required]
        public decimal ReciveValue { get; set; }

        [Column(TypeName = "datetime")]
        [Description("接收时间")]
        [Comment("接收时间")]
        public DateTime ReciveDate { get; set; }

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
