using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations.ApplicationDbCode
{
    /// <inheritdoc />
    public partial class AddOperationObjectsCodeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppId",
                table: "OperationObjectsCode",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                comment: "所属应用")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AppId",
                table: "OperationObjectsCode");
        }
    }
}
