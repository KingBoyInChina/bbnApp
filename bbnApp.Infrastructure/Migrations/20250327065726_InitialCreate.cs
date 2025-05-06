using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CompanyInfo",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PCompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "上级机构ID,企业/集团适用")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyType = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构类型,个人/个体/企业/集团")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyName = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, comment: "机构名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrganizationCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "组织机构代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyLeve = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "机构级别,1/2/3/4/5"),
                    CompanyLeveName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构级别名称,1级/2级/3级/4级")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AreaCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "所在地行政区划代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AreaName = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, comment: "所在地行政区划名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AreaNameExt = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, comment: "所在地详细信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Location = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, comment: "经纬度坐标")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "联系电话")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Contact = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "联系人")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLock = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "停用"),
                    LockTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "停用时间"),
                    LockReason = table.Column<string>(type: "varchar(40)", nullable: true, comment: "停用原因")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReMarks = table.Column<string>(type: "varchar(40)", nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyInfo", x => new { x.Yhid, x.CompanyId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DepartMents",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartMentId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, comment: "部门ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PDepartMentId = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, comment: "上级部门ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartMentName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "部门名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartMentCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "部门代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartMentIndex = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "部门序号,排序显示"),
                    DepartMentLocation = table.Column<string>(type: "varchar(100)", nullable: true, comment: "部门位置")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartMentDescription = table.Column<string>(type: "varchar(100)", nullable: true, comment: "部门介绍")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLock = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "停用"),
                    LockTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "停用时间"),
                    LockReason = table.Column<string>(type: "varchar(40)", nullable: true, comment: "停用原因")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReMarks = table.Column<string>(type: "varchar(40)", nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartMents", x => new { x.Yhid, x.DepartMentId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "员工ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "员工姓名")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "员工代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeNum = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, comment: "员工工号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartMentId = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "所在部门ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DepartMentName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "所在部门名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PositionLeve = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "职务等级"),
                    Position = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "职务名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Gender = table.Column<string>(type: "varchar(2)", maxLength: 2, nullable: false, comment: "员工性别")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BirthDate = table.Column<DateTime>(type: "datetime", nullable: false, comment: "出生日期"),
                    IDCardNumber = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true, comment: "员工证件号码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PhoneNum = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "员工联系电话")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmailNum = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, comment: "员工邮箱")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommunicationAddress = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, comment: "员工通信地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateOfEmployment = table.Column<DateTime>(type: "datetime", nullable: false, comment: "入职时间"),
                    IsLock = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "停用"),
                    LockTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "停用时间"),
                    LockReason = table.Column<string>(type: "varchar(40)", nullable: true, comment: "停用原因")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReMarks = table.Column<string>(type: "varchar(40)", nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => new { x.Yhid, x.EmployeeId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LimiteRecord",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LimiteId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "限制记录ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LimiteIP = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false, comment: "限制IP地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LimiteTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "限制记录时间"),
                    LimiteReason = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "限制原因")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LimiteExpireTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "限制有效期,到期自动解除"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LimiteRecord", x => new { x.Yhid, x.LimiteId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LoginInfo",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户身份ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginFrom = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, comment: "登录来源，PC/APP/WX/ZFB")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Token = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false, comment: "token信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Exptime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "token过期时间"),
                    Remarks = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构ID,登录成功时记录")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginInfo", x => new { x.Yhid, x.EmployeeId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LoginRecord",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "登录ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "登录用户身份id")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "登录时间"),
                    LoginFrom = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "登录来源，PC/APP/WX/ZFB")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(15)", maxLength: 15, nullable: false, comment: "登录IP")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AreaInfo = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "登录地区")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LoginState = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, comment: "登录状态,成功/失败")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Token = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "token信息,成功登录才需要")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Exptime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "token过期时间"),
                    CompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构ID,登录成功时记录")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginRecord", x => new { x.Yhid, x.LoginId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Operators",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OperatorId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "分配记录ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeeId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "员工ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PassWord = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false, comment: "密码,需加密")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PassWordExpTime = table.Column<DateTime>(type: "datetime", nullable: false, comment: "密码重置时间"),
                    IsLock = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "停用"),
                    LockTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "停用时间"),
                    LockReason = table.Column<string>(type: "varchar(40)", nullable: true, comment: "停用原因")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReMarks = table.Column<string>(type: "varchar(40)", nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operators", x => new { x.Yhid, x.OperatorId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PermissionAssignment",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PermissionId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "分配记录ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OperatorId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "操作员ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "角色ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLock = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "停用"),
                    LockTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "停用时间"),
                    LockReason = table.Column<string>(type: "varchar(40)", nullable: true, comment: "停用原因")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReMarks = table.Column<string>(type: "varchar(40)", nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermissionAssignment", x => new { x.Yhid, x.PermissionId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RoleManagment",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "角色ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "角色名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "角色代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleLeve = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "角色级别"),
                    RoleDescription = table.Column<string>(type: "varchar(100)", nullable: true, comment: "角色介绍")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLock = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "停用"),
                    LockTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "停用时间"),
                    LockReason = table.Column<string>(type: "varchar(40)", nullable: true, comment: "停用原因")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReMarks = table.Column<string>(type: "varchar(40)", nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "机构ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleManagment", x => new { x.Yhid, x.RoleId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyInfo");

            migrationBuilder.DropTable(
                name: "DepartMents");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "LimiteRecord");

            migrationBuilder.DropTable(
                name: "LoginInfo");

            migrationBuilder.DropTable(
                name: "LoginRecord");

            migrationBuilder.DropTable(
                name: "Operators");

            migrationBuilder.DropTable(
                name: "PermissionAssignment");

            migrationBuilder.DropTable(
                name: "RoleManagment");
        }
    }
}
