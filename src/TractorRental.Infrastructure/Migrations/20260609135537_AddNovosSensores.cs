using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TractorRental.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNovosSensores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "NivelOleo",
                table: "Tratores",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "RotacaoMotor",
                table: "Tratores",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Velocidade",
                table: "Tratores",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NivelOleo",
                table: "Tratores");

            migrationBuilder.DropColumn(
                name: "RotacaoMotor",
                table: "Tratores");

            migrationBuilder.DropColumn(
                name: "Velocidade",
                table: "Tratores");
        }
    }
}
