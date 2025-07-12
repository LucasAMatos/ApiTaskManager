using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTaskManager.Migrations
{
    /// <inheritdoc />
    public partial class updateColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CriadoPor",
                table: "Projetos",
                newName: "AlteradoPor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AlteradoPor",
                table: "Projetos",
                newName: "CriadoPor");
        }
    }
}
