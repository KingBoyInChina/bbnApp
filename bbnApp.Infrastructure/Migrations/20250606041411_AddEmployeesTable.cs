using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bbnApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEmployeesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "DepartMentMaster",
                table: "Employees",
                type: "bool",
                maxLength: 2,
                nullable: false,
                defaultValue: false,
                comment: "部门最高管理人")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AddColumn<string>(
                name: "PEmployeeId",
                table: "Employees",
                type: "varchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "",
                comment: "分管领导ID")
                .Annotation("MySql:CharSet", "utf8mb4")
                .Annotation("Relational:ColumnOrder", 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DepartMentMaster",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "PEmployeeId",
                table: "Employees");
        }
    }
}
