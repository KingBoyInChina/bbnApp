using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace bbnApp.Domain.Entities.User
{

    /// <summary>
    /// 员工信息表
    /// </summary>
    [PrimaryKey(nameof(Yhid), nameof(EmployeeId))] // 配置复合主键
    public class Employees
    {
        [Column(Order = 0, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("用户组ID")]
        [Comment("用户组ID")]
        [Required]
        public string Yhid { get; set; }

        [Column(Order = 1, TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("员工ID")]
        [Comment("员工ID")]
        [Required]
        public string EmployeeId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("分管领导ID")]
        [Comment("分管领导ID")]
        [Required]
        public string PEmployeeId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("员工姓名")]
        [Comment("员工姓名")]
        [Required]
        public string EmployeeName { get; set; }

        [Column( TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("员工代码")]
        [Comment("员工代码")]
        [Required]
        public string EmployeeCode { get; set; }

        [Column(TypeName = "varchar(10)")]
        [MaxLength(10)]
        [Description("员工工号")]
        [Comment("员工工号")]
        [Required]
        public string EmployeeNum { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("所在部门ID")]
        [Comment("所在部门ID")]
        [Required]
        public string DepartMentId { get; set; }

        [Column(TypeName = "varchar(40)")]
        [MaxLength(40)]
        [Description("所在部门名称")]
        [Comment("所在部门名称")]
        [Required]
        public string DepartMentName { get; set; }

        [Column(TypeName = "tinyint")]
        [MaxLength(1)]
        [Description("职务等级")]
        [Comment("职务等级")]
        [Required]
        public byte PositionLeve { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("职务名称")]
        [Comment("职务名称")]
        [Required]
        public string Position { get; set; }

        [Column( TypeName = "bool")]
        [Description("部门最高管理人")]
        [Comment("部门最高管理人")]
        [Required]
        public bool DepartMentMaster { get; set; }

        [Column(TypeName = "varchar(2)")]
        [MaxLength(2)]
        [Description("员工性别")]
        [Comment("员工性别")]
        [Required]
        public string Gender { get; set; }

        [Column(TypeName = "datetime")]
        [Description("出生日期")]
        [Comment("出生日期")]
        [Required]
        public DateTime BirthDate { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("员工证件号码")]
        [Comment("员工证件号码")]
        public string? IDCardNumber { get; set; }

        [Column(TypeName = "varchar(20)")]
        [MaxLength(20)]
        [Description("员工联系电话")]
        [Comment("员工联系电话")]
        [Required]
        public string PhoneNum { get; set; }

        [Column(TypeName = "varchar(50)")]
        [MaxLength(50)]
        [Description("员工邮箱")]
        [Comment("员工邮箱")]
        [Required]
        public string EmailNum { get; set; }

        [Column(TypeName = "varchar(80)")]
        [MaxLength(80)]
        [Description("员工通信地址")]
        [Comment("员工通信地址")]
        public string? CommunicationAddress { get; set; }

        [Column(TypeName = "datetime")]
        [Description("入职时间")]
        [Comment("入职时间")]
        [Required]
        public DateTime DateOfEmployment { get; set; }

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
