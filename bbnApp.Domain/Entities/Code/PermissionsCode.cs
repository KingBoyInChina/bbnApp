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
    /// 操作权限代码
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(PermissionId))] // 配置复合主键
    public class PermissionsCode
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("权限代码ID")]
        [Comment("权限代码ID")]
        [Required]
        public string PermissionId { get; set; }

        [Column( TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("权限代码代码")]
        [Comment("权限代码代码")]
        [Required]
        public string PermissionCode { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("权限代码名称")]
        [Comment("权限代码名称")]
        [Required]
        public string PermissionName { get; set; }

        [Column(TypeName = "varchar(100)")]
        [Description("权限代码说明")]
        [Comment("权限代码说明")]
        public string? PermissionDescription { get; set; }

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
