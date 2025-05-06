using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace bbnApp.Domain.Entities.UserLogin
{
    /// <summary>
    /// 本次登录有效信息(登录成功写入该表)
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(EmployeeId))] // 配置复合主键
    public class LoginInfo
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
        [Description("用户身份ID")]
        [Comment("用户身份ID")]
        [Required]
        public string EmployeeId { get; set; }

        [MaxLength(10)] // 设置字段长度为 50
        [Description("登录来源，PC/APP/WX/ZFB")]
        [Comment("登录来源，PC/APP/WX/ZFB")]
        [Required]
        public string LoginFrom { get; set; }

        [MaxLength(500)] // 设置字段长度为 300
        [Description("token信息")]
        [Comment("token信息")]
        [Required]
        public string? Token { get; set; }

        [Column(TypeName = "datetime")] // 设置字段类型为 datetime
        [Description("token过期时间")]
        [Comment("token过期时间")]
        [Required]
        public DateTime? Exptime { get; set; }

        [MaxLength(40)] // 设置字段长度为 50
        [Description("备注信息")]
        [Comment("备注信息")]
        public string? Remarks { get; set; }

        [MaxLength(40)] // 设置字段长度为 50
        [Description("机构ID,登录成功时记录")]
        [Comment("机构ID,登录成功时记录")]
        [Required]
        public string CompanyId { get; set; }

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
