using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bbnApp.Domain.Entities.Business
{
    /// <summary>
    /// 应用代码
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(AuthorId))] // 配置复合主键
    public class AuthorRegisterKeys
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("身份ID")]
        [Comment("身份ID")]
        [Required]
        public string AuthorId { get; set; }

        [Column(TypeName = "varchar(200)")]
        [MaxLength(200)]
        [Description("选择使用的应用，后期可能会用到")]
        [Comment("选择使用的应用，后期可能会用到")]
        [Required]
        public string? SelectedAppId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户注册时填写应用用途名称")]
        [Comment("用户注册时填写应用用途名称")]
        [Required]
        public string SetAppName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户注册时填写应用用途代码")]
        [Comment("用户注册时填写应用用途代码")]
        [Required]
        public string SetAppCode { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户注册时填写应用用途说明")]
        [Comment("用户注册时填写应用用途说明")]
        [Required]
        public string SetAppDescription { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("生成的APPID")]
        [Comment("生成的APPID")]
        [Required]
        public string AppId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("生成的密钥")]
        [Comment("生成的密钥")]
        [Required]
        public string SecriteKey { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("操作员ID")]
        [Comment("操作员ID")]
        public string? OperatorId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("机构ID")]
        [Comment("机构ID")]
        public string? CompanyId { get; set; }

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
