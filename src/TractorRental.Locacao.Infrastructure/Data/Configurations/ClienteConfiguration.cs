using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TractorRental.Locacao.Domain.Aggregates;

namespace TractorRental.Locacao.Infrastructure.Data.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.RazaoSocialOuNome).IsRequired().HasMaxLength(200);
        builder.Property(c => c.InscricaoEstadual).HasMaxLength(50);
        builder.Property(c => c.EmailFaturamento).IsRequired().HasMaxLength(150);

        builder.Property(c => c.DocumentoNumero).HasMaxLength(20).IsRequired();
        builder.Property(c => c.DocumentoTipo).IsRequired().HasConversion<string>();

        builder.Property(c => c.ContatoNome).HasMaxLength(200).IsRequired();
        builder.Property(c => c.ContatoTelefone).HasMaxLength(20).IsRequired();
        builder.Property(c => c.ContatoEmail).HasMaxLength(150).IsRequired();

        builder.Property(c => c.EnderecoLogradouro).HasMaxLength(250).IsRequired();
        builder.Property(c => c.EnderecoCidade).HasMaxLength(100).IsRequired();
        builder.Property(c => c.EnderecoEstado).HasMaxLength(2).IsRequired();
    }
}
