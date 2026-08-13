using Microsoft.EntityFrameworkCore;
using TractorRental.Locacao.Domain.Aggregates;

namespace TractorRental.Locacao.Infrastructure.Data;

public class LocacaoDbContext(DbContextOptions<LocacaoDbContext> options) : DbContext(options)
{
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<ContratoAluguel> ContratosAluguel => Set<ContratoAluguel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LocacaoDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
