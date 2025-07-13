using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTaskManager.Migrations
{
    /// <inheritdoc />
    public partial class adjust_tarefa_with_status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Prioridade",
                table: "Tarefas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjetoId",
                table: "Tarefas",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_ProjetoId",
                table: "Tarefas",
                column: "ProjetoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_Projetos_ProjetoId",
                table: "Tarefas",
                column: "ProjetoId",
                principalTable: "Projetos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Projetos_ProjetoId",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_ProjetoId",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "Prioridade",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "ProjetoId",
                table: "Tarefas");
        }
    }
}
