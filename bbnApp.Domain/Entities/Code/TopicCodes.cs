using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 订阅代码
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(TopicId))] // 配置复合主键
    public class TopicCodes
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("订阅代码ID")]
        [Comment("订阅代码ID")]
        [Required]
        public string TopicId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("订阅代码")]
        [Comment("订阅代码")]
        [Required]
        public string Code { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("订阅名称")]
        [Comment("订阅名称")]
        [Required]
        public string TopicName { get; set; }

        [Column(TypeName = "varchar(100)")]
        [MaxLength(100)]
        [Description("路由地址")]
        [Comment("路由地址")]
        [Required]
        public string TopicRoter { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("订阅分类,设备/基站等")]
        [Comment("订阅分类,设备/基站等")]
        [Required]
        public string TopicType { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("设备分类代码")]
        [Comment("设备分类代码")]
        [Required]
        public string DeviceType { get; set; }

        [Column(TypeName = "varchar(200)")]
        [MaxLength(200)]
        [Description("设备ID,需要指定设备的情况")]
        [Comment("设备ID,需要指定设备的情况")]
        [Required]
        public string? DeviceIds { get; set; }

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
