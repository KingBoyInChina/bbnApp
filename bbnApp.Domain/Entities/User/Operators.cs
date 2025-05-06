using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bbnApp.Domain.Entities.User
{
    /// <summary>
    /// 操作员信息
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(OperatorId))] // 配置复合主键
    public class Operators
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("分配记录ID")]
        [Comment("分配记录ID")]
        [Required]
        public string OperatorId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("员工ID")]
        [Comment("员工ID")]
        [Required]
        public string EmployeeId { get; set; }

        [Column(TypeName = "varchar(500)")]
        [MaxLength(500)]
        [Description("密码,需加密")]
        [Comment("密码,需加密")]
        [Required]
        public string PassWord { get; set; }

        [Column(TypeName = "datetime")]
        [Description("密码重置时间")]
        [Comment("密码重置时间")]
        [Required]
        public DateTime? PassWordExpTime { get; set; }

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
