using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbnApp.Domain.Entities.Business
{
    /// <summary>
    /// 用户联系人信息
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(ContactId))] // 配置复合主键
    public class UserContacts
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("联系人ID")]
        [Comment("联系人ID")]
        [Required]
        public string ContactId { get; set; }

        [Column( TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户ID")]
        [Comment("用户ID")]
        [Required]
        public string UserId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("联系人")]
        [Comment("联系人")]
        [Required]
        public string Contact { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("联系电话")]
        [Comment("联系电话")]
        [Required]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "TINYINT(1)")]
        [Description("主要联系人")]
        [Comment("主要联系人")]
        [Required]
        public bool IsFirst { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("职务")]
        [Comment("职务")]
        [Required]
        public string? Jobs { get; set; }

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
