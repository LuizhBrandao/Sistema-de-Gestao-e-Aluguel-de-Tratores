using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TractorRental.Locacao.Domain.Aggregates;

namespace TractorRental.Locacao.Infrastructure.Data.Configurations;

public class ContratoAluguelConfiguration : IEntityTypeConfiguration<ContratoAluguel>
{
    public void Configure(EntityTypeBuilder<ContratoAluguel> builder)
    {
        builder.ToTable("ContratosAluguel");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.ValorHora).HasPrecision(18, 2).IsRequired();
        builder.Property(c => c.Status).HasConversion<string>().IsRequired();
        builder.Ignore(c => c.DomainEvents);
    }
}
