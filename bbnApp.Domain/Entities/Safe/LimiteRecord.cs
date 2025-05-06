using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace bbnApp.Domain.Entities.Safe
{
    /// <summary>
    /// 黑名单限制记录
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(LimiteId))] // 配置复合主键
    public class LimiteRecord
    {
        // 主键：yhid 和 loginid
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)] // 设置字段长度为 50
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)] // 设置字段长度为 50
        [Description("限制记录ID")]
        [Comment("限制记录ID")]
        [Required]
        public string LimiteId { get; set; }

        [Column(TypeName = "varchar(128)")]
        [MaxLength(128)] // 设置字段长度为 128
        [Description("限制IP地址")]
        [Comment("限制IP地址")]
        [Required]
        public string LimiteIP { get; set; }

        [Column(TypeName = "datetime")] // 设置字段类型为 datetime
        [Description("限制记录时间")]
        [Comment("限制记录时间")]
        [Required]
        public DateTime LimiteTime { get; set; }

        [Column(TypeName = "varchar(200)")]
        [MaxLength(200)] // 设置字段长度为 50
        [Description("限制原因")]
        [Comment("限制原因")]
        [Required]
        public string LimiteReason { get; set; }

        [Column(TypeName = "datetime")] // 设置字段类型为 datetime
        [Description("限制有效期,到期自动解除")]
        [Comment("限制有效期,到期自动解除")]
        [Required]
        public DateTime LimiteExpireTime { get; set; }

        [MaxLength(1)] // 设置字段长度为 1
        [Column(TypeName = "tinyint")] // 设置字段类型为 tinyint
        [Description("删除标志")]
        [Comment("删除标志")]
        [Required]
        public byte Isdelete { get; set; }

        [Column(TypeName = "datetime")] // 设置字段类型为 datetime
        [Description("末次数据变更时间")]
        [Comment("末次数据变更时间")]
        [Required]
        public DateTime LastModified { get; set; }
    }
}
