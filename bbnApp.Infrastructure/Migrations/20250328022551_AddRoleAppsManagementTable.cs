using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleAppsManagementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RoleAppsManagement",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "角色ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AppId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "应用ID")
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
                    table.PrimaryKey("PK_RoleAppsManagement", x => new { x.Yhid, x.RoleId, x.AppId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RolePermissionManagement",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "角色ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ObjCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "对象代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PermissionCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "操作代码")
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
                    table.PrimaryKey("PK_RolePermissionManagement", x => new { x.Yhid, x.RoleId, x.ObjCode, x.PermissionCode });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RoleAppsManagement");

            migrationBuilder.DropTable(
                name: "RolePermissionManagement");
        }
    }
}
