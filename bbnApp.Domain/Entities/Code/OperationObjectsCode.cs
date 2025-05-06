using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 操作对象代码
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(ObjCode))] // 配置复合主键
    public class OperationObjectsCode
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("操作对象代码")]
        [Comment("操作对象代码")]
        [Required]
        public string ObjCode { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("操作对象名称")]
        [Comment("操作对象名称")]
        [Required]
        public string ObjName { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Description("操作对象说明")]
        [Comment("操作对象说明")]
        public string? ObjDescription { get; set; }

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
