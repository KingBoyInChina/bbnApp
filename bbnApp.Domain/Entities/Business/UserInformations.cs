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
    /// 用户信息
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(UserId))] // 配置复合主键
    public class UserInformations
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户ID")]
        [Comment("用户ID")]
        [Required]
        public string UserId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户类型")]
        [Comment("用户类型")]
        [Required]
        public string UserType { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户等级")]
        [Comment("用户等级")]
        [Required]
        public string UserLeve { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("种养规模")]
        [Comment("种养规模")]
        [Required]
        public string Scale { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户名称")]
        [Comment("用户名称")]
        [Required]
        public string UserName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户编号")]
        [Comment("用户编号")]
        [Required]
        public string UserNumber { get; set; }

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

        [Column(TypeName = "varchar(140)")]
        [MaxLength(140)]
        [Description("所在地名称")]
        [Comment("所在地名称")]
        public string AreaName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("所在地ID")]
        [Comment("所在地ID")]
        public string AreaId { get; set; }

        [Column(TypeName = "varchar(200)")]
        [MaxLength(200)]
        [Description("通信地址")]
        [Comment("通信地址")]
        public string Address { get; set; }

        [Column(TypeName = "varchar(50)")]
        [MaxLength(50)]
        [Description("位置坐标")]
        [Comment("通信地址")]
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
        public byte IsDelete { get; set; }

        [Column(TypeName = "datetime")]
        [Description("末次数据变更时间")]
        [Comment("末次数据变更时间")]
        [Required]
        public DateTime LastModified { get; set; }
    }
}
