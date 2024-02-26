using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LabinventTestTask.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ModuleData",
                columns: table => new
                {
                    ModuleCategoryID = table.Column<string>(type: "TEXT", nullable: false),
                    ModuleState = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleData", x => x.ModuleCategoryID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleData");
        }
    }
}
