using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations.ApplicationDbCode
{
    /// <inheritdoc />
    public partial class AddMaterialsCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialsCode",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "物资ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialType = table.Column<string>(type: "varchar(20)", maxLength: 40, nullable: false, comment: "物资类型")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "物资名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialCode = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "物资代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialBarCode = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false, comment: "物资条码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SettingIndex = table.Column<int>(type: "int", maxLength: 3, nullable: false, comment: "序号"),
                    MaterialForm = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "物资形态")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialSupplies = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, comment: "物资材质")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDanger = table.Column<byte>(type: "bool", maxLength: 1, nullable: false, comment: "危险物"),
                    DangerType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "危险分类")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Specifications = table.Column<string>(type: "varchar(400)", maxLength: 400, nullable: false, comment: "规格")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Unit = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false, comment: "计量单位")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StorageEnvironment = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "存储环境")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OtherParames = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "其他参数")
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
                    table.PrimaryKey("PK_MaterialsCode", x => new { x.Yhid, x.MaterialId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialsCode");
        }
    }
}
