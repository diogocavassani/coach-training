using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachTraining.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAtletaPlanejamentoBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TreinosPlanejadosPorSemana",
                table: "atletas",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TreinosPlanejadosPorSemana",
                table: "atletas");
        }
    }
}
