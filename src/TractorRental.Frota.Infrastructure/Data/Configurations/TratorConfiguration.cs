using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TractorRental.Frota.Domain.Aggregates;

namespace TractorRental.Frota.Infrastructure.Data.Configurations;

public class TratorConfiguration : IEntityTypeConfiguration<Trator>
{
    public void Configure(EntityTypeBuilder<Trator> builder)
    {
        builder.ToTable("Tratores");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Modelo).IsRequired().HasMaxLength(100);
        builder.Property(t => t.Marca).HasConversion<string>().IsRequired().HasMaxLength(50);
        builder.Property(t => t.AnoFabricacao).IsRequired();
        builder.Property(t => t.PotenciaCv).IsRequired();
        builder.Property(t => t.HorimetroInicial).IsRequired();
        builder.Property(t => t.NumeroSerie).IsRequired().HasMaxLength(50);
        builder.HasIndex(t => t.NumeroSerie).IsUnique();
        builder.Property(t => t.Status).HasConversion<string>().IsRequired();
        builder.Property(t => t.NivelOleo).IsRequired();
        builder.Property(t => t.RotacaoMotor).IsRequired();
        builder.Property(t => t.Velocidade).IsRequired();
        builder.Ignore(t => t.DomainEvents);
    }
}
