using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations.ApplicationDbCode
{
    /// <inheritdoc />
    public partial class AddDeviceCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceCode",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "设备ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "设备代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceType = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "设备分类")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceSpecifications = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, comment: "规格")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceModel = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: true, comment: "型号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceBarCode = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "条码号")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Usage = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "用途")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StorageEnvironment = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "存储环境")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsageEnvironment = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true, comment: "使用环境")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ServiceLife = table.Column<int>(type: "int", maxLength: 3, nullable: false, comment: "使用寿命"),
                    LifeUnit = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "计量单位")
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
                    table.PrimaryKey("PK_DeviceCode", x => new { x.Yhid, x.DeviceId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DeviceStruct",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StructId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "构成ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "设备ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "物资ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MaterialName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "物资名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UtilizeQuantity = table.Column<int>(type: "int", maxLength: 3, nullable: false, comment: "使用数量"),
                    QuantityUnit = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "计量单位")
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
                    table.PrimaryKey("PK_DeviceStruct", x => new { x.Yhid, x.StructId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TopicCodes",
                columns: table => new
                {
                    Yhid = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "用户组ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopicId = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "订阅代码ID")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "订阅代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopicName = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "订阅名称")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopicRoter = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false, comment: "路由地址")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopicType = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "订阅分类,设备/基站等")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceType = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false, comment: "设备分类代码")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DeviceIds = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false, comment: "设备ID,需要指定设备的情况")
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
                    table.PrimaryKey("PK_TopicCodes", x => new { x.Yhid, x.TopicId });
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceCode");

            migrationBuilder.DropTable(
                name: "DeviceStruct");

            migrationBuilder.DropTable(
                name: "TopicCodes");
        }
    }
}
