using TractorRental.Application.Interfaces;
using TractorRental.Domain.Aggregates;

namespace TractorRental.Infrastructure.Data.Repositories;

// Esta classe vive na Infrastructure, logo pode usar o DbContext livremente
public class RegistroManutencaoRepository(TractorRentalDbContext context) : IRegistroManutencaoRepository
{
    public async Task SalvarAsync(RegistroManutencao registro, CancellationToken cancellationToken)
    {
        await context.Set<RegistroManutencao>().AddAsync(registro, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}