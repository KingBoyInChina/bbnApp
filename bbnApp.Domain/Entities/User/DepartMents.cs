using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace bbnApp.Domain.Entities.User
{
    /// <summary>
    /// 机构部门信息表
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(DepartMentId))] // 配置复合主键
    public class DepartMents
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(10)")]
        [MaxLength(10)]
        [Description("部门ID")]
        [Comment("部门ID")]
        [Required]
        public string DepartMentId { get; set; }

        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        [Description("上级部门ID")]
        [Comment("上级部门ID")]
        [Required]
        public string PDepartMentId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("部门名称")]
        [Comment("部门名称")]
        [Required]
        public string DepartMentName { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("部门代码")]
        [Comment("部门代码")]
        [Required]
        public string DepartMentCode { get; set; }

        [Column(TypeName = "tinyint")]
        [MaxLength(1)]
        [Description("部门序号,排序显示")]
        [Comment("部门序号,排序显示")]
        [Required]
        public byte DepartMentIndex { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Description("部门位置")]
        [Comment("部门位置")]
        public string? DepartMentLocation { get; set; }


        [Column(TypeName = "varchar(100)")]
        [Description("部门介绍")]
        [Comment("部门介绍")]
        public string? DepartMentDescription { get; set; }

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

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("机构ID")]
        [Comment("机构ID")]
        [Required]
        public string CompanyId { get; set; }

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
