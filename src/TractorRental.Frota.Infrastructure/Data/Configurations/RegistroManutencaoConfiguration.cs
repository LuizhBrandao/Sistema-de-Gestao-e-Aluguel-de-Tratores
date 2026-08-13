using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TractorRental.Frota.Domain.Aggregates;

namespace TractorRental.Frota.Infrastructure.Data.Configurations;

public class RegistroManutencaoConfiguration : IEntityTypeConfiguration<RegistroManutencao>
{
    public void Configure(EntityTypeBuilder<RegistroManutencao> builder)
    {
        builder.ToTable("RegistrosManutencao");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.DescricaoDefeito).IsRequired().HasMaxLength(500);
        builder.Property(r => r.DataEntrada).IsRequired();
    }
}
