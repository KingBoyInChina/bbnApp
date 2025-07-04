using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorRegisterKeysTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorRegisterKeys",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AuthorId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "身份ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SelectedAppId = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "选择使用的应用，后期可能会用到")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SetAppName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户注册时填写应用用途名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SetAppCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户注册时填写应用用途代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SetAppDescription = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户注册时填写应用用途说明")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AppId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "生成的APPID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecriteKey = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "生成的密钥")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OperatorId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, comment: "操作员ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: true, comment: "机构ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLock = table.Column<sbyte>(type: "tinyint", nullable: false, comment: "停用"),
                    LockTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "停用时间"),
                    LockReason = table.Column<string>(type: "varchar(40)", nullable: true, comment: "停用原因")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReMarks = table.Column<string>(type: "varchar(40)", nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorRegisterKeys", x => new { x.Yhid, x.AuthorId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorRegisterKeys");
        }
    }
}
