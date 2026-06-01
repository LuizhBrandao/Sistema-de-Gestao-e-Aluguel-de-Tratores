using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TractorRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRegistroManutencao : Migration
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosManutencao");
        }
    }
}
