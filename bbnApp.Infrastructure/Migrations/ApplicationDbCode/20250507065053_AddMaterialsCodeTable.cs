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
            migrationBuilder.RenameColumn(
                name: "SettingIndex",
                table: "MaterialsCode",
                newName: "MaterialIndex");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MaterialIndex",
                table: "MaterialsCode",
                newName: "SettingIndex");
        }
    }
}
