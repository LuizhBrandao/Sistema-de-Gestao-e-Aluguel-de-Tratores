using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TractorRental.Locacao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddClienteValueObjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Documento_Numero = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Documento_TipoPessoa = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RazaoSocialOuNome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    InscricaoEstadual = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EmailFaturamento = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Contato_Nome = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Contato_Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Contato_Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Endereco_Logradouro = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Endereco_Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Endereco_Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContratosAluguel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataFim = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValorHora = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContratosAluguel", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clientes");

            migrationBuilder.DropTable(
                name: "ContratosAluguel");
        }
    }
}
