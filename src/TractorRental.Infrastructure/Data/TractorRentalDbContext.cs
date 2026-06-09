using Microsoft.EntityFrameworkCore;
using TractorRental.Domain.Aggregates;

namespace TractorRental.Infrastructure.Data;

public class TractorRentalDbContext(DbContextOptions<TractorRentalDbContext> options) : DbContext(options)
{
    public DbSet<Trator> Tratores => Set<Trator>();
    public DbSet<ContratoAluguel> ContratosAluguel => Set<ContratoAluguel>(); 
    public DbSet<RegistroManutencao> RegistrosManutencao => Set<RegistroManutencao>();
    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TractorRentalDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}