using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations.ApplicationDbCode
{
    /// <inheritdoc />
    public partial class AddDataDictionaryCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataDictionary");

            migrationBuilder.CreateTable(
                name: "DataDictionaryCode",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DicCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "字典代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DicPCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "父级字典代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLeaf = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "叶子节点"),
                    DicName = table.Column<string>(type: "varchar(40)", nullable: false, comment: "字典名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DicSpell = table.Column<string>(type: "varchar(40)", nullable: false, comment: "字典简拼")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DicIndex = table.Column<int>(type: "int", maxLength: 3, nullable: false, comment: "序号"),
                    AppId = table.Column<string>(type: "varchar(40)", nullable: true, comment: "应用ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AppName = table.Column<string>(type: "varchar(40)", nullable: true, comment: "应用名称")
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
                    table.PrimaryKey("PK_DataDictionaryCode", x => new { x.Yhid, x.DicCode });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataDictionaryCode");

            migrationBuilder.CreateTable(
                name: "DataDictionary",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DicCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "字典代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AppId = table.Column<string>(type: "varchar(40)", nullable: true, comment: "应用ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AppName = table.Column<string>(type: "varchar(40)", nullable: true, comment: "应用名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DicIndex = table.Column<int>(type: "int", maxLength: 3, nullable: false, comment: "序号"),
                    DicName = table.Column<string>(type: "varchar(40)", nullable: false, comment: "字典名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DicPCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "父级字典代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DicSpell = table.Column<string>(type: "varchar(40)", nullable: false, comment: "字典简拼")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLeaf = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "叶子节点"),
                    IsLock = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "停用"),
                    Isdelete = table.Column<sbyte>(type: "tinyint", maxLength: 1, nullable: false, comment: "删除标志"),
                    LastModified = table.Column<DateTime>(type: "datetime", nullable: false, comment: "末次数据变更时间"),
                    LockReason = table.Column<string>(type: "varchar(40)", nullable: true, comment: "停用原因")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LockTime = table.Column<DateTime>(type: "datetime", nullable: true, comment: "停用时间"),
                    ReMarks = table.Column<string>(type: "varchar(40)", nullable: true, comment: "备注信息")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataDictionary", x => new { x.Yhid, x.DicCode });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
