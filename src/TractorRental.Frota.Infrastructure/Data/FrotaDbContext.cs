using Microsoft.EntityFrameworkCore;
using TractorRental.Frota.Domain.Aggregates;

namespace TractorRental.Frota.Infrastructure.Data;

public class FrotaDbContext(DbContextOptions<FrotaDbContext> options) : DbContext(options)
{
    public DbSet<Trator> Tratores => Set<Trator>();
    public DbSet<RegistroManutencao> RegistrosManutencao => Set<RegistroManutencao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FrotaDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
