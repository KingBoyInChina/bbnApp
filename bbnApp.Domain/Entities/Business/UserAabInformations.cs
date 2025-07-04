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
    /// 用户种植养殖信息
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(AabId))] // 配置复合主键
    public class UserAabInformations
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("种养信息ID")]
        [Comment("种养信息ID")]
        [Required]
        public string AabId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户ID")]
        [Comment("用户ID")]
        [Required]
        public string UserId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("种养类型")]
        [Comment("种养类型")]
        [Required]
        public string AABType { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("种养分类")]
        [Comment("种养分类")]
        [Required]
        public string Categori { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("种养名称")]
        [Comment("种养名称")]
        [Required]
        public string ObjName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("种养名称代码")]
        [Comment("种养名称代码")]
        [Required]
        public string ObjCode { get; set; }

        [Column(TypeName = "DECIMAL(8,2)")]
        [Description("面积")]
        [Comment("面积")]
        [Required]
        public decimal AreaNumber { get; set; }

        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        [Description("面积单位")]
        [Comment("面积单位")]
        [Required]
        public string AreaNumberUnit { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("分布情况")]
        [Comment("分布情况")]
        [Required]
        public string Distribution { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("主要位置")]
        [Comment("主要位置")]
        [Required]
        public string? Point { get; set; }

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
        public byte Isdelete { get; set; }

        [Column(TypeName = "datetime")]
        [Description("末次数据变更时间")]
        [Comment("末次数据变更时间")]
        [Required]
        public DateTime LastModified { get; set; }
    }
}
