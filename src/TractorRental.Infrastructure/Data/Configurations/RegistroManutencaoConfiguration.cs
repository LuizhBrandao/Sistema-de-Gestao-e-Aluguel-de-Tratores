using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TractorRental.Domain.Aggregates;

namespace TractorRental.Infrastructure.Data.Configurations;

public class RegistroManutencaoConfiguration : IEntityTypeConfiguration<RegistroManutencao>
{
    public void Configure(EntityTypeBuilder<RegistroManutencao> builder)
    {
        // Define o nome da tabela no SQL Server
        builder.ToTable("RegistrosManutencao");

        // Define a Chave Primária
        builder.HasKey(r => r.Id);

        // Configura as colunas
        builder.Property(r => r.DescricaoDefeito)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(r => r.DataEntrada)
            .IsRequired();
    }
}