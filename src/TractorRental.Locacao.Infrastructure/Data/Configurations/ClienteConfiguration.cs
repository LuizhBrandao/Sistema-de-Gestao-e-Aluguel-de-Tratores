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

        builder.OwnsOne(c => c.Documento, doc => 
        {
            doc.Property(d => d.Numero).HasColumnName("Documento_Numero").IsRequired().HasMaxLength(20);
            doc.Property(d => d.Tipo).HasColumnName("Documento_TipoPessoa").IsRequired().HasConversion<string>();
        });

        builder.OwnsOne(c => c.ContatoOperacional, cont => 
        {
            cont.Property(co => co.Nome).HasColumnName("Contato_Nome").IsRequired().HasMaxLength(200);
            cont.Property(co => co.Telefone).HasColumnName("Contato_Telefone").IsRequired().HasMaxLength(20);
            cont.Property(co => co.Email).HasColumnName("Contato_Email").IsRequired().HasMaxLength(150);
        });

        builder.OwnsOne(c => c.EnderecoOperacao, end => 
        {
            end.Property(e => e.Logradouro).HasColumnName("Endereco_Logradouro").IsRequired().HasMaxLength(250);
            end.Property(e => e.Cidade).HasColumnName("Endereco_Cidade").IsRequired().HasMaxLength(100);
            end.Property(e => e.Estado).HasColumnName("Endereco_Estado").IsRequired().HasMaxLength(2);
        });
    }
}
