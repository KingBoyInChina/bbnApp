using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace bbnApp.Domain.Entities.User
{
    /// <summary>
    /// 机构信息表
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(CompanyId))] // 配置复合主键
    public class CompanyInfo
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("机构ID")]
        [Comment("机构ID")]
        [Required]
        public string CompanyId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("上级机构ID,企业/集团适用")]
        [Comment("上级机构ID,企业/集团适用")]
        public string PCompanyId { get; set; }

        [Column( TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("机构类型,个人/个体/企业/集团")]
        [Comment("机构类型,个人/个体/企业/集团")]
        [Required]
        public string CompanyType { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("机构名称")]
        [Comment("机构名称")]
        [Required]
        public string CompanyName { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("机构代码")]
        [Comment("机构代码")]
        [Required]
        public string CompanyCode { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("组织机构代码")]
        [Comment("组织机构代码")]
        public string OrganizationCode { get; set; }

        [Column(TypeName = "tinyint")]
        [MaxLength(1)]
        [Description("机构级别,1/2/3/4/5")]
        [Comment("机构级别,1/2/3/4/5")]
        [Required]
        public byte CompanyLeve { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("机构级别名称,1级/2级/3级/4级")]
        [Comment("机构级别名称,1级/2级/3级/4级")]
        [Required]
        public string CompanyLeveName { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("所在地行政区划代码")]
        [Comment("所在地行政区划代码")]
        [Required]
        public string AreaCode { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("所在地行政区划名称")]
        [Comment("所在地行政区划名称")]
        [Required]
        public string AreaName { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("所在地详细信息")]
        [Comment("所在地详细信息")]
        [Required]
        public string AreaNameExt { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("经纬度坐标")]
        [Comment("经纬度坐标")]
        public string? Location { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("联系电话")]
        [Comment("联系电话")]
        [Required]
        public string PhoneNumber { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("联系人")]
        [Comment("联系人")]
        [Required]
        public string Contact { get; set; }

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
