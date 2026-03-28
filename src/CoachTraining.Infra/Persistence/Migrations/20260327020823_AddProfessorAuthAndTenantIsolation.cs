using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachTraining.Infra.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessorAuthAndTenantIsolation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "professores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    SenhaHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professores", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "atletas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfessorId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ObservacoesClinicas = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    NivelEsportivo = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_atletas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_atletas_professores_ProfessorId",
                        column: x => x.ProfessorId,
                        principalTable: "professores",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "provas_alvo",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AtletaId = table.Column<Guid>(type: "uuid", nullable: false),
                    DataProva = table.Column<DateOnly>(type: "date", nullable: false),
                    DistanciaKm = table.Column<double>(type: "double precision", nullable: false),
                    Objetivo = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_provas_alvo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_provas_alvo_atletas_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "atletas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "sessoes_treino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AtletaId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    DuracaoMinutos = table.Column<int>(type: "integer", nullable: false),
                    DistanciaKm = table.Column<double>(type: "double precision", nullable: false),
                    Rpe = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessoes_treino", x => x.Id);
                    table.ForeignKey(
                        name: "FK_sessoes_treino_atletas_AtletaId",
                        column: x => x.AtletaId,
                        principalTable: "atletas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_atletas_ProfessorId",
                table: "atletas",
                column: "ProfessorId");

            migrationBuilder.CreateIndex(
                name: "IX_professores_Email",
                table: "professores",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_provas_alvo_AtletaId",
                table: "provas_alvo",
                column: "AtletaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_sessoes_treino_AtletaId_Data",
                table: "sessoes_treino",
                columns: new[] { "AtletaId", "Data" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "provas_alvo");

            migrationBuilder.DropTable(
                name: "sessoes_treino");

            migrationBuilder.DropTable(
                name: "atletas");

            migrationBuilder.DropTable(
                name: "professores");
        }
    }
}
