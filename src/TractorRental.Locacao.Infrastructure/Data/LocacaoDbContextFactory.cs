using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TractorRental.Locacao.Infrastructure.Data;

public class LocacaoDbContextFactory : IDesignTimeDbContextFactory<LocacaoDbContext>
{
    public LocacaoDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LocacaoDbContext>();
        // Conexão simulada ou pegue de variáveis de ambiente.
        optionsBuilder.UseSqlServer("Server=localhost;Database=TractorRentalDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

        return new LocacaoDbContext(optionsBuilder.Options);
    }
}
