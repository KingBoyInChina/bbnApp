using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLoginInfoTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "LoginRecord",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                comment: "token信息,成功登录才需要",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "token信息,成功登录才需要")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Exptime",
                table: "LoginRecord",
                type: "datetime",
                nullable: true,
                comment: "token过期时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldComment: "token过期时间");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "LoginRecord",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "机构ID,登录成功时记录",
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldComment: "机构ID,登录成功时记录")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "LoginInfo",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "token信息",
                oldClrType: typeof(string),
                oldType: "varchar(300)",
                oldMaxLength: 300,
                oldComment: "token信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "LoginInfo",
                type: "varchar(40)",
                maxLength: 40,
                nullable: true,
                comment: "备注信息",
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldComment: "备注信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "CompanyInfo",
                keyColumn: "AreaNameExt",
                keyValue: null,
                column: "AreaNameExt",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "AreaNameExt",
                table: "CompanyInfo",
                type: "varchar(80)",
                maxLength: 80,
                nullable: false,
                comment: "所在地详细信息",
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldMaxLength: 80,
                oldNullable: true,
                oldComment: "所在地详细信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LoginRecord",
                keyColumn: "Token",
                keyValue: null,
                column: "Token",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "LoginRecord",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                comment: "token信息,成功登录才需要",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true,
                oldComment: "token信息,成功登录才需要")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Exptime",
                table: "LoginRecord",
                type: "datetime",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "token过期时间",
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true,
                oldComment: "token过期时间");

            migrationBuilder.UpdateData(
                table: "LoginRecord",
                keyColumn: "CompanyId",
                keyValue: null,
                column: "CompanyId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "LoginRecord",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "机构ID,登录成功时记录",
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "机构ID,登录成功时记录")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Token",
                table: "LoginInfo",
                type: "varchar(300)",
                maxLength: 300,
                nullable: false,
                comment: "token信息",
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldComment: "token信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "LoginInfo",
                keyColumn: "Remarks",
                keyValue: null,
                column: "Remarks",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Remarks",
                table: "LoginInfo",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                comment: "备注信息",
                oldClrType: typeof(string),
                oldType: "varchar(40)",
                oldMaxLength: 40,
                oldNullable: true,
                oldComment: "备注信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "AreaNameExt",
                table: "CompanyInfo",
                type: "varchar(80)",
                maxLength: 80,
                nullable: true,
                comment: "所在地详细信息",
                oldClrType: typeof(string),
                oldType: "varchar(80)",
                oldMaxLength: 80,
                oldComment: "所在地详细信息")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
