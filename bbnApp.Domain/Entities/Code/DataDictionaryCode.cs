using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace bbnApp.Domain.Entities.Code
{
    /// <summary>
    /// 数据字典
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(DicCode))] // 配置复合主键
    public class DataDictionaryCode
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("字典代码")]
        [Comment("字典代码")]
        [Required]
        public string DicCode { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("父级字典代码")]
        [Comment("父级字典代码")]
        [Required]
        public string DicPCode { get; set; }

        [MaxLength(1)]
        [Column(TypeName = "tinyint")]
        [Description("叶子节点")]
        [Comment("叶子节点")]
        [Required]
        public byte IsLeaf { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("字典名称")]
        [Comment("字典名称")]
        public string DicName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("字典简拼")]
        [Comment("字典简拼")]
        public string DicSpell { get; set; }

        [MaxLength(3)]
        [Column(TypeName = "int")]
        [Description("序号")]
        [Comment("序号")]
        [Required]
        public int DicIndex { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("应用ID-专属应用字典")]
        [Comment("应用ID")]
        public string? AppId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [Description("应用名称-专属应用字典")]
        [Comment("应用名称")]
        public string? AppName { get; set; }

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
