using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TractorRental.Frota.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarCamposTrator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosManutencao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescricaoDefeito = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DataEntrada = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataResolucao = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosManutencao", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tratores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Marca = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Modelo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AnoFabricacao = table.Column<int>(type: "int", nullable: false),
                    PotenciaCv = table.Column<int>(type: "int", nullable: false),
                    HorimetroInicial = table.Column<double>(type: "float", nullable: false),
                    NumeroSerie = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TemperaturaAtualMotor = table.Column<double>(type: "float", nullable: false),
                    PressaoAtualPneus = table.Column<double>(type: "float", nullable: false),
                    NivelCombustivel = table.Column<double>(type: "float", nullable: false),
                    NivelOleo = table.Column<double>(type: "float", nullable: false),
                    RotacaoMotor = table.Column<double>(type: "float", nullable: false),
                    Velocidade = table.Column<double>(type: "float", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tratores", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tratores_NumeroSerie",
                table: "Tratores",
                column: "NumeroSerie",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosManutencao");

            migrationBuilder.DropTable(
                name: "Tratores");
        }
    }
}
