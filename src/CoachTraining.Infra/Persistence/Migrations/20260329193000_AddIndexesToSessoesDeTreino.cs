using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachTraining.Infra.Persistence.Migrations
{
    public partial class AddIndexesToSessoesDeTreino : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_sessoes_treino_AtletaId",
                table: "sessoes_treino",
                column: "AtletaId");

            migrationBuilder.CreateIndex(
                name: "IX_sessoes_treino_Data",
                table: "sessoes_treino",
                column: "Data");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_sessoes_treino_AtletaId",
                table: "sessoes_treino");

            migrationBuilder.DropIndex(
                name: "IX_sessoes_treino_Data",
                table: "sessoes_treino");
        }
    }
}
