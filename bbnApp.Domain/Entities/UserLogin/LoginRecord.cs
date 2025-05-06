using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace bbnApp.Domain.Entities.UserLogin
{
    /// <summary>
    /// 登录状态状态
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(LoginId))] // 配置复合主键
    public class LoginRecord
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
        [Description("登录ID")]
        [Comment("登录ID")]
        [Required]
        public string LoginId { get; set; }

        [MaxLength(40)] // 设置字段长度为 50
        [Description("登录用户身份id")]
        [Comment("登录用户身份id")]
        [Required]
        public string EmployeeId { get; set; }

        [Column(TypeName = "datetime")] // 设置字段类型为 datetime
        [Description("登录时间")]
        [Comment("登录时间")]
        [Required]
        public DateTime LoginTime { get; set; }

        [MaxLength(20)] // 设置字段长度为 20
        [Description("登录来源，PC/APP/WX/ZFB")]
        [Comment("登录来源，PC/APP/WX/ZFB")]
        [Required]
        public string LoginFrom { get; set; }

        [MaxLength(15)] // 设置字段长度为 15
        [Description("登录IP")]
        [Comment("登录IP")]
        public string IpAddress { get; set; }

        [MaxLength(200)] // 设置字段长度为 15
        [Description("登录地区")]
        [Comment("登录地区")]
        public string AreaInfo { get; set; }

        [MaxLength(10)] // 设置字段长度为 10
        [Description("登录状态,成功/失败")]
        [Comment("登录状态,成功/失败")]
        [Required]
        public string LoginState { get; set; }

        [MaxLength(500)] // 设置字段长度为 500
        [Description("token信息,成功登录才需要")]
        [Comment("token信息,成功登录才需要")]
        public string? Token { get; set; }

        [Column(TypeName = "datetime")] // 设置字段类型为 datetime
        [Description("token过期时间")]
        [Comment("token过期时间")]
        public DateTime? Exptime { get; set; }

        [MaxLength(40)] // 设置字段长度为 50
        [Description("机构ID,登录成功时记录")]
        [Comment("机构ID,登录成功时记录")]
        public string? CompanyId { get; set; }

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
