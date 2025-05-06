using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations.ApplicationDbCode
{
    /// <inheritdoc />
    public partial class AddObjectOperationTypesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ObjectOperationTypes",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ObjCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "操作对象代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PermissionCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "权限代码代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ObjectOperationTypes", x => new { x.Yhid, x.ObjCode, x.PermissionCode });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ObjectOperationTypes");
        }
    }
}
